using Cor.CRM.Interfaces;
using Cor.CRM.Models.Entities;
using Cor.CRM.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Repos;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CrmDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IDbRetryHandler _retry;

    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction? _transaction;

    private bool _disposed;
    private bool _hasChanges;

    public UnitOfWork(CrmDbContext context, IDbRetryHandler retry, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _retry = retry;
        _logger = logger;
        _connection = (NpgsqlConnection)_context.Database.GetDbConnection();
    }

    public IDbConnection Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    public async Task Begin(CancellationToken ct = default)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(ct);
            _logger.LogInformation("Connection OPENED. ConnectionId={ConnectionId}", _connection.ProcessID);
        }
    }

    public async Task Commit(CancellationToken ct = default)
    {
        if (!_hasChanges)
        {
            _logger.LogInformation("No changes to commit.");
            return;
        }

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _connection.BeginTransactionAsync(ct);
            try
            {
                await _context.Database.UseTransactionAsync(transaction, ct);
                _transaction = transaction;
                await FlushInternalAsync(ct);
                await transaction.CommitAsync(ct);
                _context.ChangeTracker.Clear();
                _logger.LogInformation("Transaction COMMITTED successfully. ConnectionId={ConnectionId}", _connection.ProcessID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction FAILED. Rolling back. ConnectionId={ConnectionId}", _connection.ProcessID);
                await transaction.RollbackAsync(ct);
                throw;
            }
            finally
            {
                _transaction = null;
            }
        });
    }

    public async Task Rollback(CancellationToken ct = default)
    {
        if (_transaction == null)
        {
            _logger.LogInformation("No Transactions available to Rollback.");
            return;
        }

        await _transaction.RollbackAsync(ct);
        _transaction = null;
        _logger.LogWarning("Transaction rolled back manually.");
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _retry.ExecuteAsync(async () => { return await FlushInternalAsync(ct); }, ct);
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
        // REMOVED: _context.Entry(entity).Property(x => x.xmin).OriginalValue = entity.xmin;
        _context.Entry(entity).State = EntityState.Modified;
        _logger.LogDebug("Entity updated: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        return Task.CompletedTask;
    }

    public Task UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        foreach (var entity in entities)
        {
            _context.Attach(entity);
            // REMOVED: _context.Entry(entity).Property(x => x.xmin).OriginalValue = entity.xmin;
            _context.Entry(entity).State = EntityState.Modified;
        }
        _logger.LogDebug("Entities updated: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        return Task.CompletedTask;
    }

    public async Task Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        var entry = _context.Attach(entity);
        // REMOVED: entry.Property(e => e.xmin).OriginalValue = entity.xmin;
        entry.State = EntityState.Modified;

        _logger.LogDebug("Entity soft deleted: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        await Task.CompletedTask;
    }

    public async Task Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = _context.Attach(entity);
        // REMOVED: entry.Property(e => e.xmin).OriginalValue = entity.xmin;
        entry.State = EntityState.Deleted;

        _logger.LogDebug("Entity hard deleted: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        await Task.CompletedTask;
    }

    public async Task Restore<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = false;
        entity.UpdatedAt = DateTime.UtcNow;

        var entry = _context.Attach(entity);
        // REMOVED: entry.Property(e => e.xmin).OriginalValue = entity.xmin;
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