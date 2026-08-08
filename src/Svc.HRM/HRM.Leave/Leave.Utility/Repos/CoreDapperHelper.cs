// Leave.Utility/Repos/CoreDapperHelper.cs
using Dapper;
using Leave.App.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Leave.Utility.Repos;

public sealed class CoreDapperHelper : ICoreDapperHelper
{
    private readonly string _connectionString;
    private readonly IDbRetryHandler _retry;
    private readonly ILogger<CoreDapperHelper> _logger;

    // Leave.Utility/Repos/CoreDapperHelper.cs
    public CoreDapperHelper(
        IConfiguration configuration,
        IDbRetryHandler retry,
        ILogger<CoreDapperHelper> logger)
    {
        // Use "CorModuleDbCon" from your appsettings.json
        _connectionString = configuration.GetConnectionString("CorModuleDbCon")
            ?? throw new InvalidOperationException("Core connection string 'CorModuleDbCon' not found");
        _retry = retry ?? throw new ArgumentNullException(nameof(retry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    private CommandDefinition CreateCommand(string sql, object? param, CancellationToken ct, bool buffered = true)
    {
        return new CommandDefinition(sql, param, cancellationToken: ct, flags: buffered ? CommandFlags.Buffered : CommandFlags.None);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                using var connection = CreateConnection();
                var cmd = CreateCommand(sql, param, ct);
                var result = await connection.QueryFirstOrDefaultAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("Core QueryFirstOrDefault SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core QueryFirstOrDefault FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
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
                using var connection = CreateConnection();
                var cmd = CreateCommand(sql, param, ct);
                var result = await connection.QuerySingleOrDefaultAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("Core QuerySingleOrDefault SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core QuerySingleOrDefault FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
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
                using var connection = CreateConnection();
                var cmd = CreateCommand(sql, param, ct);
                var result = await connection.QuerySingleAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("Core QuerySingle SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core QuerySingle FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
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
                using var connection = CreateConnection();
                var cmd = CreateCommand(sql, param, ct, buffered);
                var result = await connection.QueryAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("Core QueryAsync SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core QueryAsync FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
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
                using var connection = CreateConnection();
                var result = await connection.QueryAsync(sql, map, param, splitOn: splitOn);
                sw.Stop();
                _logger.LogInformation("Core MultiMapping Query SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core MultiMapping Query FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
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
                using var connection = CreateConnection();
                var cmd = CreateCommand(sql, param, ct);
                var result = await connection.ExecuteAsync(cmd);
                sw.Stop();
                _logger.LogInformation("Core Execute SUCCESS ({ElapsedMs} ms), Rows={Rows}", sw.ElapsedMilliseconds, result);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core Execute FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                using var connection = CreateConnection();
                var cmd = CreateCommand(sql, param, ct);
                var result = await connection.ExecuteScalarAsync<T>(cmd);
                sw.Stop();
                _logger.LogInformation("Core ExecuteScalar SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core ExecuteScalar FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                var connection = CreateConnection();
                await connection.OpenAsync(ct);
                var cmd = CreateCommand(sql, param, ct, buffered: false);
                var reader = await connection.ExecuteReaderAsync(cmd);
                sw.Stop();
                _logger.LogInformation("Core ExecuteReader SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return (DbDataReader)reader;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Core ExecuteReader FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }
}