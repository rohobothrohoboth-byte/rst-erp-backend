using Cor.HRMM.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static Dapper.SqlMapper;

namespace Cor.HRMM.Repos;

public sealed class DapperHelper : IDapperHelper
{
    private readonly IUnitOfWork _uow;
    private readonly IDbRetryHandler _retry;
    private readonly ILogger<DapperHelper> _logger;
    private readonly DatabaseOptions _options;

    public DapperHelper(
        IUnitOfWork uow,
        IDbRetryHandler retry,
        IOptions<DatabaseOptions> options,
        ILogger<DapperHelper> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _retry = retry ?? throw new ArgumentNullException(nameof(retry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new DatabaseOptions();
    }

    #region Private Helpers

    private CommandDefinition CreateCommand(string sql, object? param, CancellationToken ct, bool buffered = true)
    {
        return new CommandDefinition(
            commandText: sql,
            parameters: param,
            transaction: _uow.Transaction,  // ✅ Uses UnitOfWork's Transaction
            commandTimeout: _options.CommandTimeout,
            cancellationToken: ct,
            flags: buffered ? CommandFlags.Buffered : CommandFlags.None);
    }

    private async Task<TResult> ExecuteAsync<TResult>(
        string operation,
        string sql,
        object? param,
        Func<IDbConnection, CommandDefinition, Task<TResult>> action,
        CancellationToken ct,
        bool buffered = true)
    {
        return await _retry.ExecuteAsync(async () =>
        {
            var command = CreateCommand(sql, param, ct, buffered: buffered);
            var watch = Stopwatch.StartNew();

            try
            {
                var result = await action(_uow.Connection, command);  // ✅ Uses UnitOfWork's Connection
                watch.Stop();
                LogPerformance(operation, sql, param, watch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                watch.Stop();
                LogFailure(operation, sql, param, watch.ElapsedMilliseconds, ex);
                throw;
            }
        }, ct);
    }

    private void LogPerformance(string operation, string sql, object? parameters, long elapsed)
    {
        if (elapsed < _options.SlowQueryMilliseconds)
            return;

        _logger.LogWarning(
            "Slow SQL detected | Operation: {Operation} | Elapsed: {Elapsed}ms | SQL: {Sql} | Parameters: {@Parameters}",
            operation, elapsed, sql, parameters);
    }

    private void LogFailure(string operation, string sql, object? parameters, long elapsed, Exception ex)
    {
        _logger.LogError(ex,
            "Database command failed | Operation: {Operation} | Elapsed: {Elapsed}ms | SQL: {Sql} | Parameters: {@Parameters}",
            operation, elapsed, sql, parameters);
    }

    #endregion

    #region Query Methods

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await ExecuteAsync(nameof(QueryFirstOrDefaultAsync), sql, param,
            (conn, cmd) => conn.QueryFirstOrDefaultAsync<T>(cmd), ct);
    }

    public async Task<T> QueryFirstAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await ExecuteAsync(nameof(QueryFirstAsync), sql, param,
            (conn, cmd) => conn.QueryFirstAsync<T>(cmd), ct);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await ExecuteAsync(nameof(QuerySingleAsync), sql, param,
            (conn, cmd) => conn.QuerySingleAsync<T>(cmd), ct);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await ExecuteAsync(nameof(QuerySingleOrDefaultAsync), sql, param,
            (conn, cmd) => conn.QuerySingleOrDefaultAsync<T>(cmd), ct);
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default, bool buffered = true)
    {
        return await ExecuteAsync(nameof(QueryAsync), sql, param,
            async (conn, cmd) =>
            {
                var result = await conn.QueryAsync<T>(cmd);
                return result.AsList();
            }, ct, buffered);
    }

    public async IAsyncEnumerable<T> QueryStreamAsync<T>(string sql, object? param = null, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var command = CreateCommand(sql, param, ct, buffered: false);

        // ✅ Use DbDataReader for async methods (cast from IDbConnection)
        await using var reader = (DbDataReader)await _uow.Connection.ExecuteReaderAsync(command);
        var parser = reader.GetRowParser<T>();

        while (await reader.ReadAsync(ct))
        {
            yield return parser(reader);
        }
    }

    #endregion

    #region Multi-Mapping

    public async Task<IReadOnlyList<TResult>> QueryAsync<TFirst, TSecond, TResult>(
        string sql,
        Func<TFirst, TSecond, TResult> map,
        object? param = null,
        string splitOn = "Id",
        CancellationToken ct = default,
        bool buffered = true)
    {
        return await ExecuteAsync(nameof(QueryAsync), sql, param,
            async (conn, cmd) =>
            {
                var result = await conn.QueryAsync(sql, map, param, transaction: _uow.Transaction, splitOn: splitOn);
                return result.AsList();
            }, ct, buffered);
    }

    public async Task<IReadOnlyList<TResult>> QueryAsync<T1, T2, T3, TResult>(
        string sql,
        Func<T1, T2, T3, TResult> map,
        object? param = null,
        string splitOn = "Id",
        CancellationToken ct = default,
        bool buffered = true)
    {
        return await ExecuteAsync(nameof(QueryAsync), sql, param,
            async (conn, cmd) =>
            {
                var result = await conn.QueryAsync(sql, map, param, transaction: _uow.Transaction, splitOn: splitOn);
                return result.AsList();
            }, ct, buffered);
    }

    public async Task<GridReader> QueryMultipleAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var command = CreateCommand(sql, param, ct);
        return await _uow.Connection.QueryMultipleAsync(command);
    }

    #endregion

    #region Command Methods

    public async Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        return await ExecuteAsync(nameof(ExecuteAsync), sql, param,
            (conn, cmd) => conn.ExecuteAsync(cmd), ct);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await ExecuteAsync(nameof(ExecuteScalarAsync), sql, param,
            (conn, cmd) => conn.ExecuteScalarAsync<T>(cmd), ct);
    }

    public async Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var command = CreateCommand(sql, param, ct, buffered: false);
        return (DbDataReader)await _uow.Connection.ExecuteReaderAsync(command);
    }

    #endregion

    #region CommandDefinition Support

    public async Task<IReadOnlyList<T>> QueryAsync<T>(CommandDefinition command)
    {
        return await _retry.ExecuteAsync(async () =>
        {
            var result = await _uow.Connection.QueryAsync<T>(command);
            return result.AsList();
        }, command.CancellationToken);
    }

    public async Task<int> ExecuteAsync(CommandDefinition command)
    {
        return await _retry.ExecuteAsync(async () =>
        {
            return await _uow.Connection.ExecuteAsync(command);
        }, command.CancellationToken);
    }

    public async Task<T?> ExecuteScalarAsync<T>(CommandDefinition command)
    {
        return await _retry.ExecuteAsync(async () =>
        {
            return await _uow.Connection.ExecuteScalarAsync<T>(command);
        }, command.CancellationToken);
    }

    #endregion

    #region Transaction Methods

    public async Task ExecuteInTransactionAsync(
        Func<IDbTransaction, Task> action,
        IsolationLevel isolation = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        // ✅ Use UnitOfWork's transaction management
        await _uow.BeginTransactionAsync(isolation, ct);
        try
        {
            await action(_uow.Transaction!);
            await _uow.CommitTransactionAsync(ct);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<IDbTransaction, Task<T>> action,
        IsolationLevel isolation = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        // ✅ Use UnitOfWork's transaction management
        await _uow.BeginTransactionAsync(isolation, ct);
        try
        {
            var result = await action(_uow.Transaction!);
            await _uow.CommitTransactionAsync(ct);
            return result;
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    #endregion
}

public class DatabaseOptions
{
    public int CommandTimeout { get; set; } = 60;
    public int SlowQueryMilliseconds { get; set; } = 500;
}