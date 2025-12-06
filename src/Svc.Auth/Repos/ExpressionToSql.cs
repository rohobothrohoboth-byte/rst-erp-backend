using System.Linq.Expressions;
using Dapper;

namespace Svc.Auth.Repos;

public static class ExpressionToSql
{
    public static (string Sql, DynamicParameters Params) Parse<T>(Expression<Func<T, bool>> predicate)
    {
        var parameters = new DynamicParameters();
        var sql = VisitExpression(predicate.Body, parameters);
        return (sql, parameters);
    }

    private static string VisitExpression(Expression expr, DynamicParameters parameters)
    {
        switch (expr)
        {
            case BinaryExpression be:
                var left = VisitExpression(be.Left, parameters);
                var right = VisitExpression(be.Right, parameters);
                var op = GetSqlOperator(be.NodeType);
                return $"({left} {op} {right})";

            case MemberExpression me when me.Expression is ParameterExpression:
                // PostgreSQL quoted identifier
                return $"\"{me.Member.Name}\"";

            case ConstantExpression ce:
                var paramName = $"@p{parameters.ParameterNames.Count()}";
                parameters.Add(paramName, ce.Value);
                return paramName;

            case MemberExpression:
                // Handles DateTime.Now etc
                var value = Expression.Lambda(expr).Compile().DynamicInvoke();
                var p = $"@p{parameters.ParameterNames.Count()}";
                parameters.Add(p, value);
                return p;

            case MethodCallExpression mce:
                return HandleMethodCall(mce, parameters);

            default:
                var val = Expression.Lambda(expr).Compile().DynamicInvoke();
                var pname = $"@p{parameters.ParameterNames.Count()}";
                parameters.Add(pname, val);
                return pname;
        }
    }

    private static string HandleMethodCall(MethodCallExpression mce, DynamicParameters parameters)
    {
        // Handle string.Contains / StartsWith / EndsWith
        if (mce.Object != null && mce.Object.Type == typeof(string))
        {
            var column = VisitExpression(mce.Object, parameters);
            var value = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke()?.ToString();
            var paramName = $"@p{parameters.ParameterNames.Count()}";

            parameters.Add(paramName, mce.Method.Name switch
            {
                "Contains" => $"%{value}%",
                "StartsWith" => $"{value}%",
                "EndsWith" => $"%{value}",
                _ => value
            });

            return $"{column} LIKE {paramName}";
        }

        // Handle IN: list.Contains(x.Property)
        if (mce.Method.Name == "Contains" && mce.Arguments.Count == 1)
        {
            Expression collectionExpr;
            Expression propertyExpr;

            if (mce.Object != null)
            {
                collectionExpr = mce.Object;
                propertyExpr = mce.Arguments[0];
            }
            else
            {
                collectionExpr = mce.Arguments[0];
                propertyExpr = mce.Arguments[1];
            }

            var values = ((IEnumerable<object>)Expression.Lambda(collectionExpr).Compile().DynamicInvoke()!).ToList();

            if (!values.Any())
                return "1=0"; // empty IN = no match

            var column = VisitExpression(propertyExpr, parameters);

            var inParams = new List<string>();
            foreach (var v in values)
            {
                var pName = $"@p{parameters.ParameterNames.Count()}";
                parameters.Add(pName, v);
                inParams.Add(pName);
            }

            return $"{column} IN ({string.Join(", ", inParams)})";
        }

        throw new NotSupportedException($"Method {mce.Method.Name} is not supported");
    }

    private static string GetSqlOperator(ExpressionType type)
    {
        return type switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            _ => throw new NotSupportedException($"Operator {type} is not supported")
        };
    }
}