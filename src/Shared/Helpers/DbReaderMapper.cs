using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq.Expressions;

namespace Helpers;

public static class DbReaderMapper<T> where T : new()
{
    private static readonly ConcurrentDictionary<string, Func<DbDataReader, T>> Cache = new();

    public static Func<DbDataReader, T> GetMapper(DbDataReader reader)
    {
        var key = string.Join(",", Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)));
        if (Cache.TryGetValue(key, out var mapper))
            return mapper;

        var readerParam = Expression.Parameter(typeof(DbDataReader), "r");
        var objVar = Expression.Variable(typeof(T), "obj");

        var expressions = new List<Expression>
        {
            Expression.Assign(objVar, Expression.New(typeof(T)))
        };

        foreach (var prop in typeof(T).GetProperties().Where(p => p.CanWrite))
        {
            int ordinal;

            try
            {
                ordinal = reader.GetOrdinal(prop.Name);
            }
            catch
            {
                continue;
            }

            var isDbNull = Expression.Call(readerParam, typeof(DbDataReader).GetMethod(nameof(DbDataReader.IsDBNull))!, Expression.Constant(ordinal));
            var getValue = Expression.Call(readerParam, typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetFieldValue))!.MakeGenericMethod(prop.PropertyType), Expression.Constant(ordinal));
            var assign = Expression.IfThenElse(isDbNull, Expression.Assign(Expression.Property(objVar, prop), Expression.Default(prop.PropertyType)), Expression.Assign(Expression.Property(objVar, prop), getValue));
            expressions.Add(assign);
        }

        expressions.Add(objVar);
        var body = Expression.Block(new[] { objVar }, expressions);
        var lambda = Expression.Lambda<Func<DbDataReader, T>>(body, readerParam).Compile();
        Cache[key] = lambda;
        return lambda;
    }
}
