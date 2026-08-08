// Leave.Utility/Repos/HrmProDapperHelper.cs
using Dapper;
using Leave.App.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Leave.Utility.Repos;

public sealed class HrmProDapperHelper : IHrmProDapperHelper
{
    private readonly string _connectionString;
    private readonly IDbRetryHandler _retry;
    private readonly ILogger<HrmProDapperHelper> _logger;

    public HrmProDapperHelper(
        IConfiguration configuration,
        IDbRetryHandler retry,
        ILogger<HrmProDapperHelper> logger)
    {
        _connectionString = configuration.GetConnectionString("HRMProDbCon")
            ?? throw new InvalidOperationException("HrmPro connection string not found");
        _retry = retry;
        _logger = logger;
    }

    private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                using var connection = CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<T>(sql, param);
                sw.Stop();
                _logger.LogInformation("HrmPro QueryFirstOrDefault SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "HrmPro QueryFirstOrDefault FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        return await _retry.ExecuteAsync(async () =>
        {
            try
            {
                using var connection = CreateConnection();
                var result = await connection.QueryAsync<T>(sql, param);
                sw.Stop();
                _logger.LogInformation("HrmPro QueryAsync SUCCESS ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "HrmPro QueryAsync FAILED ({ElapsedMs} ms)", sw.ElapsedMilliseconds);
                throw;
            }
        }, ct);
    }
}