// Leave.App/Interfaces/ICoreDapperHelper.cs
using System.Data.Common;

namespace Leave.App.Interfaces;

public interface ICoreDapperHelper
{
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default, bool buffered = true);
    Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default);
    Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, CancellationToken ct = default);
    Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> map, object? param = null, string splitOn = "Id", CancellationToken ct = default, bool buffered = true);
}