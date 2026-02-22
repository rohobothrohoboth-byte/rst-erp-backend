using Dapper;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Helpers;

public class QueryBuilder
{
    private readonly List<string> _select = new();
    private string? _from;
    private readonly List<string> _joins = new();
    private readonly List<string> _where = new();
    private readonly List<string> _orderBy = new();
    private int? _limit;
    private readonly DynamicParameters _parameters = new();
    private int _paramIndex = 0;

    private readonly HashSet<string> _softDeleteInjectedAliases = new();
    private readonly List<string> _multiTenantConditions = new();

    /* ================= BUILD ================= */
    public (string Sql, DynamicParameters Params) Build()
    {
        var sb = new StringBuilder();
        sb.AppendLine("SELECT " + string.Join(", ", _select));
        sb.AppendLine("FROM " + _from);

        if (_joins.Count > 0)
            sb.AppendLine(string.Join("\n", _joins));

        var allWhere = new List<string>(_where);
        allWhere.AddRange(_multiTenantConditions);

        if (allWhere.Count > 0)
            sb.AppendLine("WHERE " + string.Join(" ", allWhere));

        if (_orderBy.Count > 0)
            sb.AppendLine("ORDER BY " + string.Join(", ", _orderBy));

        if (_limit.HasValue)
            sb.AppendLine("LIMIT " + _limit.Value);

        return (sb.ToString(), _parameters);
    }

    /* ================= SELECT ================= */
    public QueryBuilder Select<T>(string alias, params Expression<Func<T, object>>[] cols)
    {
        foreach (var col in cols)
            _select.Add(SqlGen.Column(col, alias));
        return this;
    }

    // For aliasing column
    public QueryBuilder SelectAs<T>(string alias, string asName, Expression<Func<T, object>> col)
    {
        _select.Add($"{SqlGen.Column(col, alias)} AS \"{asName}\"");
        return this;
    }

    /* ================= FROM ================= */
    public QueryBuilder From<T>(string alias)
    {
        _from = SqlGen.Table<T>(alias);
        InjectSoftDelete<T>(alias);
        return this;
    }

    /* ================= JOIN ================= */
    public QueryBuilder Join<TLeft, TRight>(string leftAlias, string rightAlias, Expression<Func<TLeft, object>> leftKey, Expression<Func<TRight, object>> rightKey, string type = "JOIN")
    {
        _joins.Add($"""
        {type} {SqlGen.Table<TRight>(rightAlias)}
            ON {SqlGen.Column(rightKey, rightAlias)} =
               {SqlGen.Column(leftKey, leftAlias)}
        """);

        InjectSoftDelete<TRight>(rightAlias);

        return this;
    }

    /* ================= AND ================= */
    public QueryBuilder And<T>(string alias, Expression<Func<T, object>> col, string op, object value)
    {
        var param = AddParam(value);
        AppendCondition($"{SqlGen.Column(col, alias)} {op} {param}");
        return this;
    }

    /* ================= OR ================= */
    public QueryBuilder Or<T>(string alias, Expression<Func<T, object>> col, string op, object value)
    {
        var param = AddParam(value);
        if (_where.Count == 0)
            throw new InvalidOperationException("OR cannot be first condition.");
        _where.Add("OR " + $"{SqlGen.Column(col, alias)} {op} {param}");
        return this;
    }

    /* ================= ILIKE SEARCH ================= */
    public QueryBuilder SearchILike<T>(string alias, string term, params Expression<Func<T, object>>[] cols)
    {
        if (string.IsNullOrWhiteSpace(term))
            return this;

        var param = AddParam($"%{term}%");
        var conditions = cols.Select(c => $"{SqlGen.Column(c, alias)} ILIKE {param}");
        AppendCondition("(" + string.Join(" OR ", conditions) + ")");
        return this;
    }

    /* ================= FULL-TEXT SEARCH ================= */
    public QueryBuilder SearchFullText(string searchVectorExpression, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return this;

        var param = AddParam(term);
        AppendCondition($""" to_tsvector('simple', {searchVectorExpression}) @@ plainto_tsquery('simple', {param}) """);
        return this;
    }

    /* ================= ORDER BY ================= */
    public QueryBuilder OrderBy<T>(string alias, Expression<Func<T, object>> col, bool desc = false)
    {
        _orderBy.Add($"{SqlGen.Column(col, alias)} {(desc ? "DESC" : "ASC")}");
        return this;
    }

    /* ================= KEYSET PAGINATION ================= */
    public QueryBuilder KeysetAfter<T>(string alias, (Expression<Func<T, object>> col, object value)[] keys, bool desc = false)
    {
        var parts = new List<string>();

        for (int i = 0; i < keys.Length; i++)
        {
            var andParts = new List<string>();

            for (int j = 0; j < i; j++)
            {
                var col = SqlGen.Column(keys[j].col, alias);
                var param = AddParam(keys[j].value);
                andParts.Add($"{col} = {param}");
            }

            var op = desc ? "<" : ">";
            var keyCol = SqlGen.Column(keys[i].col, alias);
            var keyParam = AddParam(keys[i].value);

            andParts.Add($"{keyCol} {op} {keyParam}");

            parts.Add("(" + string.Join(" AND ", andParts) + ")");
        }

        AppendCondition("(" + string.Join(" OR ", parts) + ")");
        return this;
    }

    /* ================= LIMIT ================= */
    public QueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /* ================= MULTI-TENANT FILTER ================= */
    public QueryBuilder MultiTenantFilter<T>(string alias, Expression<Func<T, object>> col, object tenantId)
    {
        var param = AddParam(tenantId);
        _multiTenantConditions.Add($"{SqlGen.Column(col, alias)} = {param}");
        return this;
    }

    /* ================= INTERNAL ================= */
    private void AppendCondition(string condition)
    {
        if (_where.Count == 0)
            _where.Add(condition);
        else
            _where.Add("AND " + condition);
    }

    private string AddParam(object value)
    {
        var name = $"@p{_paramIndex++}";
        _parameters.Add(name, value);
        return name;
    }

    private void InjectSoftDelete<T>(string alias)
    {
        if (_softDeleteInjectedAliases.Contains(alias))
            return;

        var prop = typeof(T).GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(bool))
            AppendCondition($"{alias}.\"IsDeleted\" = false");

        _softDeleteInjectedAliases.Add(alias);
    }
}