using System.Data;

namespace Svc.Task.Interfaces;

public interface IDapperHelper
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken ct = default);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, CancellationToken ct = default);
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CancellationToken ct = default);
    Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken ct = default);
    Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, CancellationToken ct = default);
}