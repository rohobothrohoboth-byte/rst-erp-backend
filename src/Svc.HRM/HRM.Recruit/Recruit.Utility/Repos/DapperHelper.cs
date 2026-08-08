using Dapper;
using Microsoft.Extensions.Logging;
using Recruit.App.Interfaces;
using System.Data.Common;
using System.Diagnostics;

namespace Recruit.Utility.Repos;

public sealed class DapperHelper : IDapperHelper
{
    private readonly IUnitOfWork _uow;
    private readonly IDbRetryHandler _retry;
    private readonly ILogger<DapperHelper> _logger;

    public DapperHelper(IUnitOfWork uow, IDbRetryHandler retry, ILogger<DapperHelper> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _retry = retry ?? throw new ArgumentNullException(nameof(retry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private CommandDefinition CreateCommand(string sql, object? param, CancellationToken ct, bool buffered = true)
    {
        return new CommandDefinition(sql, param, transaction: _uow.Transaction, cancellationToken: ct, flags: buffered ? CommandFlags.Buffered : CommandFlags.None);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var cmd = CreateCommand(sql, param, ct);
                var result = await _uow.Connection.QueryFirstOrDefaultAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("QueryFirstOrDefault SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(sql, "QueryFirstOrDefault FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                _logger.LogError(ex, "QueryFirstOrDefault FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var cmd = CreateCommand(sql, param, ct);
                var result = await _uow.Connection.QuerySingleOrDefaultAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("QuerySingleOrDefault SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "QuerySingleOrDefault FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var cmd = CreateCommand(sql, param, ct);
                var result = await _uow.Connection.QuerySingleAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("QuerySingle SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "QuerySingle FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default, bool buffered = true)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var cmd = CreateCommand(sql, param, ct, buffered);
                var result = await _uow.Connection.QueryAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("QueryAsync SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "QueryAsync FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> map, object? param = null, string splitOn = "Id", CancellationToken ct = default, bool buffered = true)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var result = await _uow.Connection.QueryAsync(sql, map, param, transaction: _uow.Transaction, splitOn: splitOn);

                sw.Stop();
                _logger.LogInformation("MultiMapping Query SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "MultiMapping Query FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var cmd = CreateCommand(sql, param, ct);
                var result = await _uow.Connection.ExecuteAsync(cmd);
                sw.Stop();
                _logger.LogInformation("Execute SUCCESS ({ElapsedMs} ms), Rows={Rows}", sw.ElapsedMilliseconds, result);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Execute FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        return await _retry.ExecuteAsync(async () =>
        {
            var cmd = CreateCommand(sql, param, ct);
            return await _uow.Connection.ExecuteScalarAsync<T>(cmd);
        }, ct);
    }

    public async Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var cmd = CreateCommand(sql, param, ct, buffered: false);
                var reader = await _uow.Connection.ExecuteReaderAsync(cmd);
                sw.Stop();
                _logger.LogInformation("ExecuteReader SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return (DbDataReader)reader;

            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "ExecuteReader FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
    {
        return await _retry.ExecuteAsync(async () => { return await _uow.Connection.QueryAsync<T>(command); });
    }

    public async Task<int> ExecuteAsync(CommandDefinition command)
    {
        return await _retry.ExecuteAsync(async () => { return await _uow.Connection.ExecuteAsync(command); });
    }
}