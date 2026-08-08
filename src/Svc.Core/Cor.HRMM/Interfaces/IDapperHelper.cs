using Dapper;
using System.Data;
using System.Data.Common;
using static Dapper.SqlMapper;

namespace Cor.HRMM.Interfaces;

public interface IDapperHelper
{
    #region Query Methods

    /// <summary>
    /// Executes a query and returns the first result, or default if no result
    /// </summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);

    /// <summary>
    /// Executes a query and returns the first result, throws if no result
    /// </summary>
    Task<T> QueryFirstAsync<T>(string sql, object? param = null, CancellationToken ct = default);

    /// <summary>
    /// Executes a query and returns a single result, throws if more than one or no result
    /// </summary>
    Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default);

    /// <summary>
    /// Executes a query and returns a single result, or default if no result
    /// </summary>
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);

    /// <summary>
    /// Executes a query and returns a list of results
    /// </summary>
    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default, bool buffered = true);

    /// <summary>
    /// Executes a query and streams results asynchronously (for large datasets)
    /// </summary>
    IAsyncEnumerable<T> QueryStreamAsync<T>(string sql, object? param = null, CancellationToken ct = default);

    #endregion

    #region Multi-Mapping Methods

    /// <summary>
    /// Executes a query with multi-mapping (2 types)
    /// </summary>
    Task<IReadOnlyList<TResult>> QueryAsync<TFirst, TSecond, TResult>(
        string sql,
        Func<TFirst, TSecond, TResult> map,
        object? param = null,
        string splitOn = "Id",
        CancellationToken ct = default,
        bool buffered = true);

    /// <summary>
    /// Executes a query with multi-mapping (3 types)
    /// </summary>
    Task<IReadOnlyList<TResult>> QueryAsync<T1, T2, T3, TResult>(
        string sql,
        Func<T1, T2, T3, TResult> map,
        object? param = null,
        string splitOn = "Id",
        CancellationToken ct = default,
        bool buffered = true);

    /// <summary>
    /// Executes a query returning multiple result sets
    /// </summary>
    Task<GridReader> QueryMultipleAsync(string sql, object? param = null, CancellationToken ct = default);

    #endregion

    #region Command Methods

    /// <summary>
    /// Executes a non-query command (INSERT, UPDATE, DELETE)
    /// </summary>
    Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default);

    /// <summary>
    /// Executes a scalar query returning a single value
    /// </summary>
    Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default);

    /// <summary>
    /// Executes a query and returns a data reader
    /// </summary>
    Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, CancellationToken ct = default);

    #endregion

    #region CommandDefinition Methods

    /// <summary>
    /// Executes a query using a CommandDefinition
    /// </summary>
    Task<IReadOnlyList<T>> QueryAsync<T>(CommandDefinition command);

    /// <summary>
    /// Executes a non-query using a CommandDefinition
    /// </summary>
    Task<int> ExecuteAsync(CommandDefinition command);

    /// <summary>
    /// Executes a scalar query using a CommandDefinition
    /// </summary>
    Task<T?> ExecuteScalarAsync<T>(CommandDefinition command);

    #endregion

    #region Transaction Methods

    /// <summary>
    /// Executes an action within a database transaction
    /// </summary>
    Task ExecuteInTransactionAsync(
        Func<IDbTransaction, Task> action,
        IsolationLevel isolation = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);

    /// <summary>
    /// Executes a function within a database transaction and returns a result
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<IDbTransaction, Task<T>> action,
        IsolationLevel isolation = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);

    #endregion
}