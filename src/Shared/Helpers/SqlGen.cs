using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

public static class SqlGen
{
    public static string Table<T>(string? alias = null)
    {
        var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
        return alias is null ? Quote(name) : $"{Quote(name)} {alias}";
    }

    public static string Column<T>(Expression<Func<T, object>> expr, string alias)
    {
        var member = GetMember(expr);
        var name = member.GetCustomAttribute<ColumnAttribute>()?.Name ?? member.Name;
        return $"{alias}.{Quote(name)}";
    }

    private static MemberInfo GetMember<T>(Expression<Func<T, object>> expr)
    {
        if (expr.Body is MemberExpression m)
            return m.Member;

        if (expr.Body is UnaryExpression u &&
            u.Operand is MemberExpression um)
            return um.Member;

        throw new InvalidOperationException("Invalid expression");
    }

    private static string Quote(string name) => $"\"{name}\"";
}