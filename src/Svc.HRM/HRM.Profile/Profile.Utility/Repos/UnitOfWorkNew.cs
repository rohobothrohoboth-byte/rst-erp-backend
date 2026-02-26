using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Profile.App.Interfaces;
using System.Data;

namespace Profile.Utility.Repos;

public sealed class UnitOfWorkNew : IUnitOfWorkNew
{
    private readonly string _connectionString;
    private readonly ILogger<UnitOfWorkNew> _logger;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;

    public UnitOfWorkNew(IConfiguration config, ILogger<UnitOfWorkNew> logger)
    {
        _connectionString = config.GetConnectionString("HRMProDbCon") ?? throw new InvalidOperationException("Connection string not configured.");
        _logger = logger;
    }

    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection not initialized.");
    public IDbTransaction? Transaction => _transaction;

    public async Task BeginAsync(CancellationToken ct = default)
    {
        if (_connection != null) { return; }
        _connection = new NpgsqlConnection(_connectionString);
        var start = DateTime.UtcNow;
        await _connection.OpenAsync(ct);
        _transaction = await _connection.BeginTransactionAsync(ct);
        _logger.LogInformation("DB Transaction STARTED at {StartTime}, ConnectionId={ConnectionId}", start, _connection?.ProcessID);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction == null) { return; }
        await _transaction.CommitAsync(ct);
        _logger.LogInformation("DB Transaction COMMITTED at {Time}, ConnectionId={ConnectionId}", DateTime.UtcNow, _connection?.ProcessID);
        await DisposeTransactionAsync();
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction == null) { return; }
        await _transaction.RollbackAsync(ct);
        _logger.LogWarning("DB Transaction ROLLED BACK at {Time}, ConnectionId={ConnectionId}", DateTime.UtcNow, _connection?.ProcessID);
        await DisposeTransactionAsync();
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) { return; }
        await DisposeTransactionAsync();
        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
        _disposed = true;
        _logger.LogDebug("UnitOfWork disposed.");
    }

    public void Dispose()
    {
        if (_disposed) { return; }
        _transaction?.Dispose();
        _connection?.Dispose();
        _disposed = true;
        _logger.LogDebug("UnitOfWork disposed.");
    }
}