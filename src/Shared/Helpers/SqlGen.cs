using Dapper;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

public static class SqlMetadata
{
    private static readonly ConcurrentDictionary<Type, string> TableCache = new();
    private static readonly ConcurrentDictionary<MemberInfo, string> ColumnCache = new();

    public static string Table(Type type)
    {
        return TableCache.GetOrAdd(type, t =>
        {
            var attr = t.GetCustomAttribute<TableAttribute>();
            return attr?.Name ?? t.Name;
        });
    }

    public static string Column(MemberInfo member)
    {
        return ColumnCache.GetOrAdd(member, m =>
        {
            var attr = m.GetCustomAttribute<ColumnAttribute>();
            return attr?.Name ?? m.Name;
        });
    }

    public static bool HasSoftDelete(Type type)
    {
        return type.GetProperty("IsDeleted")?.PropertyType == typeof(bool);
    }
}

public static class SqlGen
{
    public static string Col<T>(string alias, Expression<Func<T, object>> expr)
    {
        var member = GetMember(expr);
        var name = SqlMetadata.Column(member);

        return $"{alias}.\"{name}\"";
    }

    public static MemberInfo GetMember<T>(Expression<Func<T, object>> expr)
    {
        return expr.Body switch
        {
            MemberExpression m => m.Member,
            UnaryExpression u when u.Operand is MemberExpression um => um.Member,
            _ => throw new InvalidOperationException("Invalid column expression")
        };
    }
}

public sealed class QueryBuilder
{
    private readonly List<string> _select = new();
    private readonly List<string> _joins = new();
    private readonly List<string> _where = new();
    private readonly List<string> _group = new();
    private readonly List<string> _order = new();
    private readonly DynamicParameters _params = new();

    private string? _from;
    private int _paramIndex;
    private int? _limit;
    private int? _offset;
    private bool _distinct;

    public QueryBuilder Select<T>(string alias, params Expression<Func<T, object>>[] cols)
    {
        foreach (var c in cols) { _select.Add(SqlGen.Col(alias, c)); }
        return this;
    }

    public QueryBuilder SelectRaw(string sql)
    {
        _select.Add(sql);
        return this;
    }

    public QueryBuilder SelectAs<TSource, TDest>(string alias, Expression<Func<TSource, object>> source, Expression<Func<TDest, object>> dest)
    {
        var sourceCol = SqlGen.Col(alias, source);
        var destName = SqlMetadata.Column(SqlGen.GetMember(dest));
        _select.Add($"{sourceCol} AS \"{destName}\"");
        return this;
    }

    public QueryBuilder SelectDto<TEntity, TDto>(string alias)
    {
        var entityProps = typeof(TEntity).GetProperties();
        var dtoProps = typeof(TDto).GetProperties().Select(p => SqlMetadata.Column(p)).ToHashSet();

        foreach (var prop in entityProps)
        {
            if (!dtoProps.Contains(prop.Name)) { continue; }
            var col = SqlMetadata.Column(prop);
            _select.Add($"{alias}.\"{col}\"");
        }

        return this;
    }

    public QueryBuilder From<T>(string alias)
    {
        _from = $"\"{SqlMetadata.Table(typeof(T))}\" {alias}";
        if (SqlMetadata.HasSoftDelete(typeof(T))) { _where.Add($"{alias}.\"IsDeleted\" = false"); }
        return this;
    }

    public QueryBuilder Join<TLeft, TRight>(string leftAlias, string rightAlias, Expression<Func<TLeft, object>> leftKey, Expression<Func<TRight, object>> rightKey, bool leftJoin = false)
    {
        var type = leftJoin ? "LEFT JOIN" : "JOIN";
        var sql = $"{type} \"{SqlMetadata.Table(typeof(TRight))}\" {rightAlias} " + $"ON {SqlGen.Col(leftAlias, leftKey)} = {SqlGen.Col(rightAlias, rightKey)}";
        if (SqlMetadata.HasSoftDelete(typeof(TRight))) { sql += $" AND {rightAlias}.\"IsDeleted\" = false"; }

        _joins.Add(sql);
        return this;
    }

    public QueryBuilder LeftJoin<TLeft, TRight>(string leftAlias, string rightAlias, Expression<Func<TLeft, object>> left, Expression<Func<TRight, object>> right)
    {
        return Join(leftAlias, rightAlias, left, right, true);
    }

    public QueryBuilder Where<T>(string alias, Expression<Func<T, bool>> predicate)
    {
        var sql = ParseExpression(alias, predicate.Body);
        AppendWhere(sql);
        return this;
    }

    public QueryBuilder WhereRaw<T>(string alias, Expression<Func<T, object>> column, string sqlOperator, object? value = null)
    {
        var columnName = SqlMetadata.Column(SqlGen.GetMember(column));
        if (value == null)
        {
            AppendWhere($"{alias}.\"{columnName}\" {sqlOperator}");
        }
        else
        {
            var param = AddParam(value);
            AppendWhere($"{alias}.\"{columnName}\" {sqlOperator} {param}");
        }

        return this;
    }

    public QueryBuilder WhereIn<T>(string alias, Expression<Func<T, object>> column, IEnumerable values)
    {
        var columnName = SqlMetadata.Column(SqlGen.GetMember(column));
        var param = AddParam(values);
        AppendWhere($"{alias}.\"{columnName}\" = ANY({param})");
        return this;
    }

    public QueryBuilder OrderBy<T>(string alias, Expression<Func<T, object>> col, bool desc = false)
    {
        _order.Add($"{SqlGen.Col(alias, col)} {(desc ? "DESC" : "ASC")}");
        return this;
    }

    public QueryBuilder GroupBy(params string[] cols)
    {
        _group.AddRange(cols);
        return this;
    }

    public QueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    public (string Sql, DynamicParameters Params) Build()
    {
        var sb = new StringBuilder();

        sb.Append(_distinct ? "SELECT DISTINCT " : "SELECT ");
        sb.Append(_select.Count == 0 ? "*" : string.Join(", ", _select));

        sb.AppendLine();
        sb.AppendLine($"FROM {_from}");

        foreach (var j in _joins)
            sb.AppendLine(j);

        if (_where.Count > 0)
            sb.AppendLine("WHERE " + string.Join(" AND ", _where));

        if (_group.Count > 0)
            sb.AppendLine("GROUP BY " + string.Join(", ", _group));

        if (_order.Count > 0)
            sb.AppendLine("ORDER BY " + string.Join(", ", _order));

        if (_limit.HasValue)
            sb.AppendLine($"LIMIT {_limit}");

        if (_offset.HasValue)
            sb.AppendLine($"OFFSET {_offset}");

        return (sb.ToString(), _params);
    }

    private void AppendWhere(string condition)
    {
        if (!string.IsNullOrWhiteSpace(condition)) { _where.Add(condition); }
    }

    private string AddParam(object? value)
    {
        var name = $"@p{_paramIndex++}";
        _params.Add(name, value);
        return name;
    }

    private string ParseExpression(string alias, Expression expr)
    {
        if (expr is UnaryExpression u && expr.NodeType == ExpressionType.Convert)
        {
            return ParseExpression(alias, u.Operand);
        }

        return expr switch
        {
            BinaryExpression b => ParseBinary(alias, b),
            MemberExpression m => ParseMember(alias, m),
            ConstantExpression c => AddParam(c.Value),
            MethodCallExpression mc => ParseMethod(alias, mc),
            _ => throw new NotSupportedException($"Expression {expr.NodeType} not supported")
        };
    }

    private string ParseBinary(string alias, BinaryExpression expr)
    {
        var left = ParseExpression(alias, expr.Left);

        if (expr.Right is ConstantExpression c && c.Value == null)
        {
            return expr.NodeType switch
            {
                ExpressionType.Equal => $"{left} IS NULL",
                ExpressionType.NotEqual => $"{left} IS NOT NULL",
                _ => throw new NotSupportedException()
            };
        }

        var right = ParseExpression(alias, expr.Right);

        var op = expr.NodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            _ => throw new NotSupportedException()
        };

        return expr.NodeType switch
        {
            ExpressionType.AndAlso or ExpressionType.OrElse
                => $"({left} {op} {right})",
            _ => $"{left} {op} {right}"
        };
    }

    private string ParseMember(string alias, MemberExpression expr)
    {
        if (expr.Expression is ParameterExpression)
        {
            var column = SqlMetadata.Column(expr.Member);
            return $"{alias}.\"{column}\"";
        }

        var value = Expression.Lambda(expr).Compile().DynamicInvoke();
        return AddParam(value);
    }

    private string ParseMethod(string alias, MethodCallExpression expr)
    {
        if (expr.Method.Name == "Contains")
        {
            if (expr.Object != null)
            {
                var column = ParseExpression(alias, expr.Object);
                var value = Expression.Lambda(expr.Arguments[0]).Compile().DynamicInvoke();
                var param = AddParam($"%{value}%");

                return $"{column} LIKE {param}";
            }

            var values = Expression.Lambda(expr.Arguments[0]).Compile().DynamicInvoke();
            var columnExpr = expr.Arguments[1] as MemberExpression;

            if (columnExpr != null)
            {
                var column = $"{alias}.\"{SqlMetadata.Column(columnExpr.Member)}\"";
                var param = AddParam(values);

                return $"{column} = ANY({param})";
            }
        }

        if (expr.Method.Name == "StartsWith")
        {
            var column = ParseExpression(alias, expr.Object!);
            var value = Expression.Lambda(expr.Arguments[0]).Compile().DynamicInvoke();
            var param = AddParam($"{value}%");

            return $"{column} LIKE {param}";
        }

        if (expr.Method.Name == "EndsWith")
        {
            var column = ParseExpression(alias, expr.Object!);
            var value = Expression.Lambda(expr.Arguments[0]).Compile().DynamicInvoke();
            var param = AddParam($"%{value}");

            return $"{column} LIKE {param}";
        }

        throw new NotSupportedException($"Method {expr.Method.Name} not supported");
    }
}