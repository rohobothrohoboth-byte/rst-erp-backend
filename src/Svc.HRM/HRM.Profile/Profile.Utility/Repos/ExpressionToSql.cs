using Dapper;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Profile.Utility.Repos;

public static class ExpressionToSql
{
    private static readonly ConcurrentDictionary<string, CachedQuery> _cache = new();

    public static (string Sql, DynamicParameters Params) Parse<T>(Expression<Func<T, bool>> predicate)
    {
        var key = GetCacheKey(predicate);
        if (_cache.TryGetValue(key, out var cached))
        {
            return (cached.Sql, BuildParameters(predicate, cached.ParameterExtractors));
        }

        var context = new ParseContext();
        var sql = VisitExpression(predicate.Body, context);
        var cachedQuery = new CachedQuery
        {
            Sql = sql,
            ParameterExtractors = context.ParameterExtractors
        };

        _cache.TryAdd(key, cachedQuery);
        return (sql, BuildParameters(predicate, cachedQuery.ParameterExtractors)
        );
    }

    private static string GetCacheKey<T>(Expression<Func<T, bool>> expression)
    {
        return $"{typeof(T).FullName}:{expression}";
    }

    private static DynamicParameters BuildParameters<T>(Expression<Func<T, bool>> expression, List<Func<object>> extractors)
    {
        var parameters = new DynamicParameters();
        for (int i = 0; i < extractors.Count; i++)
        {
            var value = extractors[i]();
            if (value is Array arr)
            { parameters.Add($"@p{i}", arr); }
            else
            { parameters.Add($"@p{i}", value); }
        }

        return parameters;
    }

    private static string VisitExpression(Expression expr, ParseContext context)
    {
        switch (expr)
        {
            case BinaryExpression be:
                var left = VisitExpression(be.Left, context);
                var right = VisitExpression(be.Right, context);
                var op = GetSqlOperator(be.NodeType);
                return $"({left} {op} {right})";

            case MemberExpression me
                when me.Expression is ParameterExpression:
                return $"\"{me.Member.Name}\"";

            case ConstantExpression ce:
                return context.AddParameter(() => ce.Value);

            case MemberExpression me:
                var getter = Expression.Lambda<Func<object>>(Expression.Convert(me, typeof(object))).Compile();

                return context.AddParameter(getter);

            case MethodCallExpression mce:
                return HandleMethodCall(mce, context);

            default:
                var compiled = Expression.Lambda<Func<object>>(Expression.Convert(expr, typeof(object))).Compile();
                return context.AddParameter(compiled);
        }
    }

    private static string HandleMethodCall(MethodCallExpression mce, ParseContext context)
    {
        if (mce.Object?.Type == typeof(string))
        {
            var column = VisitExpression(mce.Object, context);
            var valueGetter = Expression.Lambda<Func<object>>(Expression.Convert(mce.Arguments[0], typeof(object))).Compile();
            return mce.Method.Name switch
            {
                "Contains" => $"{column} ILIKE {context.AddParameter(() => $"%{valueGetter()}%")}",
                "StartsWith" => $"{column} ILIKE {context.AddParameter(() => $"{valueGetter()}%")}",
                "EndsWith" => $"{column} ILIKE {context.AddParameter(() => $"%{valueGetter()}")}",
                _ => throw new NotSupportedException($"String method {mce.Method.Name} not supported")
            };
        }

        if (mce.Method.Name == "Contains")
        {
            var collectionGetter = Expression.Lambda<Func<object>>(Expression.Convert(mce.Object ?? mce.Arguments[0], typeof(object))).Compile();
            var columnExpr = mce.Object != null ? mce.Arguments[0] : mce.Arguments[1];
            var column = VisitExpression(columnExpr, context);
            var valuesObj = collectionGetter();
            if (valuesObj is not System.Collections.IEnumerable enumerable)
            { throw new NotSupportedException("Contains requires IEnumerable"); }

            var paramNames = new List<string>();
            foreach (var value in enumerable)
            {
                var captured = value;
                paramNames.Add(context.AddParameter(() => captured));
            }

            if (paramNames.Count == 0)
            { return "1=0"; }

            return $"{column} IN ({string.Join(",", paramNames)})";
        }

        throw new NotSupportedException($"Method {mce.Method.Name} not supported");
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
            _ => throw new NotSupportedException()
        };
    }

    private class ParseContext
    {
        public List<Func<object>> ParameterExtractors = new();

        public string AddParameter(Func<object> extractor)
        {
            var index = ParameterExtractors.Count;
            ParameterExtractors.Add(extractor);
            return $"@p{index}";
        }
    }

    private class CachedQuery
    {
        public string Sql { get; set; }
        public List<Func<object>> ParameterExtractors { get; set; }
    }
}