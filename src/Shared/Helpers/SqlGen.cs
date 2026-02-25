using Dapper;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

public static class SqlMetadataCache
{
    private static readonly ConcurrentDictionary<Type, string> TableNames = new();
    private static readonly ConcurrentDictionary<MemberInfo, string> ColumnNames = new();
    private static readonly ConcurrentDictionary<Type, bool> SoftDeleteTypes = new();

    public static string GetTableName(Type type)
    {
        return TableNames.GetOrAdd(type, t =>
        {
            var attr = t.GetCustomAttribute<TableAttribute>();
            return attr?.Name ?? t.Name;
        });
    }

    public static string GetColumnName(MemberInfo member)
    {
        return ColumnNames.GetOrAdd(member, m =>
        {
            var attr = m.GetCustomAttribute<ColumnAttribute>();
            return attr?.Name ?? m.Name;
        });
    }

    public static bool HasSoftDelete(Type type)
    {
        return SoftDeleteTypes.GetOrAdd(type, t => t.GetProperty("IsDeleted")?.PropertyType == typeof(bool));
    }
}

public static class SqlGen
{
    public static string Table<T>(string alias) => $"\"{SqlMetadataCache.GetTableName(typeof(T))}\" {alias}";

    public static string Column<T>(Expression<Func<T, object>> expr, string alias)
    {
        var member = GetMember(expr);
        var name = SqlMetadataCache.GetColumnName(member);
        return $"{alias}.\"{name}\"";
    }

    private static MemberInfo GetMember<T>(Expression<Func<T, object>> expr)
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
    private readonly List<string> _select = new(16);
    private readonly List<string> _joins = new(8);
    private readonly List<string> _where = new(8);
    private readonly List<string> _order = new(4);
    private readonly DynamicParameters _params = new();
    private string? _from;
    private int? _limit;
    private int _paramIndex;
    private readonly HashSet<string> _softDeleteWhereAliases = new();

    public QueryBuilder Select<T>(string alias, params Expression<Func<T, object>>[] cols)
    {
        foreach (var c in cols)
            _select.Add(SqlGen.Column(c, alias));
        return this;
    }

    public QueryBuilder SelectAs<T>(string alias, string asName, Expression<Func<T, object>> col)
    {
        _select.Add($"{SqlGen.Column(col, alias)} AS \"{asName}\"");
        return this;
    }

    public QueryBuilder From<T>(string alias)
    {
        _from = SqlGen.Table<T>(alias);
        if (SqlMetadataCache.HasSoftDelete(typeof(T)))
            InjectSoftDeleteWhere(alias);

        return this;
    }

    public QueryBuilder Join<TLeft, TRight>(string leftAlias, string rightAlias, Expression<Func<TLeft, object>> leftKey, Expression<Func<TRight, object>> rightKey, bool leftJoin = false)
    {
        var joinType = leftJoin ? "LEFT JOIN" : "JOIN";
        var sb = new StringBuilder(128);
        sb.Append(joinType).Append(' ').Append(SqlGen.Table<TRight>(rightAlias)).AppendLine().Append("    ON ").Append(SqlGen.Column(rightKey, rightAlias)).Append(" = ").Append(SqlGen.Column(leftKey, leftAlias));

        if (SqlMetadataCache.HasSoftDelete(typeof(TRight)))
        {
            sb.AppendLine().Append("    AND ").Append(rightAlias).Append(".\"IsDeleted\" = false");
        }

        _joins.Add(sb.ToString());
        return this;
    }

    public QueryBuilder LeftJoin<TLeft, TRight>(string leftAlias, string rightAlias, Expression<Func<TLeft, object>> leftKey, Expression<Func<TRight, object>> rightKey) => Join<TLeft, TRight>(leftAlias, rightAlias, leftKey, rightKey, true);

    public QueryBuilder Where<T>(string alias, Expression<Func<T, object>> col, string op, object value)
    {
        var param = AddParam(value);
        AppendWhere($"{SqlGen.Column(col, alias)} {op} {param}");
        return this;
    }

    public QueryBuilder WhereRaw(string sql)
    {
        AppendWhere(sql);
        return this;
    }

    private void InjectSoftDeleteWhere(string alias)
    {
        if (_softDeleteWhereAliases.Contains(alias))
            return;

        AppendWhere($"{alias}.\"IsDeleted\" = false");
        _softDeleteWhereAliases.Add(alias);
    }

    public QueryBuilder OrderBy<T>(string alias, Expression<Func<T, object>> col, bool desc = false)
    {
        _order.Add($"{SqlGen.Column(col, alias)} {(desc ? "DESC" : "ASC")}");
        return this;
    }

    public QueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    public (string Sql, DynamicParameters Params) Build()
    {
        var sb = new StringBuilder(512);
        sb.Append("SELECT ");
        if (_select.Count == 0)
            sb.Append('*');
        else
            sb.AppendJoin(", ", _select);

        sb.AppendLine().Append("FROM ").AppendLine(_from);
        foreach (var j in _joins)
            sb.AppendLine(j);

        if (_where.Count > 0)
        {
            sb.Append("WHERE ");
            sb.AppendJoin(" AND ", _where);
            sb.AppendLine();
        }

        if (_order.Count > 0)
        {
            sb.Append("ORDER BY ");
            sb.AppendJoin(", ", _order);
            sb.AppendLine();
        }

        if (_limit.HasValue)
        {
            sb.Append("LIMIT ");
            sb.Append(_limit.Value);
            sb.AppendLine();
        }

        return (sb.ToString(), _params);
    }

    private void AppendWhere(string condition) => _where.Add(condition);

    private string AddParam(object value)
    {
        var name = $"@p{_paramIndex++}";
        _params.Add(name, value);
        return name;
    }
}