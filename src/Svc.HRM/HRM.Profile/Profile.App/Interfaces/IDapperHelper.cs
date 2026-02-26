using System.Data.Common;

namespace Profile.App.Interfaces;

public interface IDapperHelper
{
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default);
    Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, CancellationToken ct = default);
}