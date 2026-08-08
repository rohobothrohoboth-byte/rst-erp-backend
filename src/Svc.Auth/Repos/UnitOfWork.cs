using Microsoft.EntityFrameworkCore;
using Npgsql;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;
using Svc.Auth.Persistence;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
namespace Svc.Auth.Repos;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IDbRetryHandler _retry;

    private readonly NpgsqlConnection _connection;
    private IDbContextTransaction? _transaction;

    private bool _disposed;
    private bool _hasChanges;

    public UnitOfWork(AuthDbContext context, IDbRetryHandler retry, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _retry = retry;
        _logger = logger;
        _connection = (NpgsqlConnection)_context.Database.GetDbConnection();
    }

    public IDbConnection Connection => _connection;
    public IDbTransaction? Transaction => _transaction?.GetDbTransaction();

    public async Task Begin(CancellationToken ct = default)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(ct);
            _logger.LogInformation("Connection OPENED. ConnectionId={ConnectionId}", _connection.ProcessID);
        }

        if (_transaction == null)
        {
            // ? Use EF Core's transaction API instead of raw connection
            _transaction = await _context.Database.BeginTransactionAsync(ct);
            _logger.LogInformation("Transaction STARTED. ConnectionId={ConnectionId}", _connection.ProcessID);
        }
    }

    public async Task Commit(CancellationToken ct = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No transaction to commit.");
            return;
        }

        try
        {
            // ? We should NOT call SaveChangesAsync here - it's handled by ExecuteInTransactionAsync
            // The changes are already saved by the strategy

            // ? Commit the transaction
            await _transaction.CommitAsync(ct);
            _logger.LogInformation("Transaction COMMITTED. ConnectionId={ConnectionId}", _connection.ProcessID);

            // ? Dispose the transaction
            await _transaction.DisposeAsync();
            _transaction = null;

            // ? Clear EF change tracker
            _context.ChangeTracker.Clear();
            _hasChanges = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit transaction. ConnectionId={ConnectionId}", _connection.ProcessID);
            throw;
        }
    }

    public async Task Rollback(CancellationToken ct = default)
    {
        if (_transaction == null)
        {
            _logger.LogInformation("No transaction to rollback.");
            return;
        }

        try
        {
            await _transaction.RollbackAsync(ct);
            _logger.LogWarning("Transaction ROLLED BACK. ConnectionId={ConnectionId}", _connection.ProcessID);

            await _transaction.DisposeAsync();
            _transaction = null;

            _context.ChangeTracker.Clear();
            _hasChanges = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rollback transaction. ConnectionId={ConnectionId}", _connection.ProcessID);
            throw;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _retry.ExecuteAsync(async () =>
        {
            return await FlushInternalAsync(ct);
        }, ct);
    }

    private async Task<int> FlushInternalAsync(CancellationToken ct)
    {
        if (!_hasChanges)
        {
            _logger.LogWarning("No changes available to Save.");
            return 0;
        }

        _context.ChangeTracker.DetectChanges();
        var result = await _context.SaveChangesAsync(ct);
        _hasChanges = false;
        _logger.LogInformation("SaveChanges SUCCESS. Rows={Rows}, ConnectionId={ConnectionId}", result, _connection.ProcessID);
        return result;
    }

    public async Task Add<TEntity>(TEntity entity, CancellationToken ct = default) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _retry.ExecuteAsync(async () =>
        {
            await _context.Set<TEntity>().AddAsync(entity, ct);
            _hasChanges = true;
            _logger.LogDebug("Entity added: {Entity}", typeof(TEntity).Name);
        }, ct);
    }

    public async Task AddRange<TEntity>(IEnumerable<TEntity> entities, CancellationToken ct = default) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _retry.ExecuteAsync(async () =>
        {
            await _context.Set<TEntity>().AddRangeAsync(entities, ct);
            _logger.LogDebug("Entities added: {Entity}", typeof(TEntity).Name);
            _hasChanges = true;
        }, ct);
    }

    public Task Update<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        _logger.LogDebug("Entity updated: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        return Task.CompletedTask;
    }

    public async Task Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = true;
        entity.DateMod = DateTime.UtcNow;

        var entry = _context.Attach(entity);
        entry.State = EntityState.Modified;

        _logger.LogDebug("Entity soft deleted: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        await Task.CompletedTask;
    }

    public async Task Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = _context.Attach(entity);
        entry.State = EntityState.Deleted;

        _logger.LogDebug("Entity hard deleted: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        await Task.CompletedTask;
    }

public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
{
    var strategy = _context.Database.CreateExecutionStrategy();
    await strategy.ExecuteAsync(async () =>
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        var connectionId = _connection.ProcessID; // Store connection ID before it might close
        _logger.LogInformation("Transaction STARTED. ConnectionId={ConnectionId}", connectionId);

        bool committed = false;
        try
        {
            _transaction = transaction;
            _hasChanges = false;

            await action();

            if (_hasChanges)
            {
                _context.ChangeTracker.DetectChanges();
                var rowsAffected = await _context.SaveChangesAsync(ct);
                _logger.LogInformation("SaveChanges SUCCESS. Rows={Rows}", rowsAffected);
                _hasChanges = false;
            }

            await transaction.CommitAsync(ct);
            committed = true;
            _logger.LogInformation("Transaction COMMITTED. ConnectionId={ConnectionId}", connectionId);

            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during transaction");
            if (!committed)
            {
                try
                {
                    await transaction.RollbackAsync(ct);
                    _logger.LogWarning("Transaction ROLLED BACK. ConnectionId={ConnectionId}", connectionId);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Error during rollback (transaction may already be completed)");
                }
            }
            else
            {
                _logger.LogWarning("Transaction already committed, no rollback attempted.");
            }
            throw;
        }
        finally
        {
            _transaction = null;
        }
    });
}
    public async Task Restore<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = false;
        entity.DateMod = DateTime.UtcNow;

        var entry = _context.Attach(entity);
        entry.State = EntityState.Modified;

        _logger.LogDebug("Entity restored: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        await Task.CompletedTask;
    }

    public DbSet<TEntity> Set<TEntity>() where TEntity : class => _context.Set<TEntity>();

    private async Task DisposeTransactionAsync()
    {
        if (_transaction == null) { return; }
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) { return; }
        await DisposeTransactionAsync();
        if (_connection.State == ConnectionState.Open) { await _connection.CloseAsync(); }
        await _context.DisposeAsync();
        _disposed = true;
        _logger.LogDebug("UnitOfWork disposed.");
    }

    public void Dispose()
    {
        if (_disposed) { return; }
        _transaction?.Dispose();
        if (_connection.State == ConnectionState.Open) { _connection.Close(); }
        _context.Dispose();
        _disposed = true;
        _logger.LogDebug("UnitOfWork disposed synchronously.");
    }
}