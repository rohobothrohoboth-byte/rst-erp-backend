using Dapper;
using Microsoft.Extensions.Logging;
using Profile.App.Interfaces;
using System.Data.Common;
using System.Diagnostics;

namespace Profile.Utility.Repos;

public sealed class DapperHelper : IDapperHelper
{
    private readonly IUnitOfWorkNew _uow;
    private readonly ILogger<DapperHelper> _logger;

    public DapperHelper(IUnitOfWorkNew uow, ILogger<DapperHelper> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    private CommandDefinition CreateCommand(string sql, object? param, CancellationToken ct) => new(sql, param, _uow.Transaction, cancellationToken: ct);
    public async Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var start = DateTime.UtcNow;
        var sw = Stopwatch.StartNew();
        try
        {
            var cmd = CreateCommand(sql, param, ct);
            var result = await _uow.Connection.ExecuteAsync(cmd);
            sw.Stop();
            _logger.LogInformation("SQL Executed SUCCESSFULLY in {ElapsedMs} ms", sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "SQL Execution FAILED in {ElapsedMs} ms ", sw.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var cmd = CreateCommand(sql, param, ct);
            var result = await _uow.Connection.QueryFirstOrDefaultAsync<T>(cmd);
            sw.Stop();
            _logger.LogInformation("Query executed in {ElapsedMs} ms", sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Query FAILED in {ElapsedMs} ms", sw.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var cmd = CreateCommand(sql, param, ct);
            var result = await _uow.Connection.QueryAsync<T>(cmd);
            sw.Stop();
            _logger.LogInformation("QueryAsync executed in {ElapsedMs} ms", sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "QueryAsync FAILED in {ElapsedMs} ms", sw.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<DbDataReader> ExecuteReaderAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var cmd = CreateCommand(sql, param, ct);
            var reader = (DbDataReader)await _uow.Connection.ExecuteReaderAsync(cmd);
            sw.Stop();
            _logger.LogInformation("ExecuteReader completed in {ElapsedMs} ms", sw.ElapsedMilliseconds);
            return reader;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "ExecuteReader FAILED in {ElapsedMs} ms", sw.ElapsedMilliseconds);
            throw;
        }
    }
}