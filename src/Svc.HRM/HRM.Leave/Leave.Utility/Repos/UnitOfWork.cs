using Leave.App.Interfaces;
using Leave.Domain.Entities;
using Leave.Utility.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using Helpers;
namespace Leave.Utility.Repos;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly HrmLeaveDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IDbRetryHandler _retry;
    private readonly IDbExceptionTranslator _translator;

    private bool _disposed;
    private bool _hasChanges;
    private DbTransaction? _transaction; // Changed to DbTransaction

    public UnitOfWork(
        HrmLeaveDbContext context,
        IDbRetryHandler retry,
        ILogger<UnitOfWork> logger,
        IDbExceptionTranslator translator)
    {
        _context = context;
        _retry = retry;
        _logger = logger;
        _translator = translator;
    }

    public IDbConnection? Connection => _context.Database.GetDbConnection();
    public IDbTransaction? Transaction => _transaction;

    private async Task BeginTran(IsolationLevel isolation, CancellationToken ct)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(ct);
        }
        _transaction = await connection.BeginTransactionAsync(isolation, ct) as DbTransaction;
        await _context.Database.UseTransactionAsync(_transaction, ct);
    }

    private async Task Save(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }

    private async Task CommitTran(CancellationToken ct)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private async Task RollbackTran(CancellationToken ct)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task Begin(CancellationToken ct = default)
    {
        await BeginTran(IsolationLevel.ReadCommitted, ct);
    }

    public async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, ct);
    }

    public async Task Commit(CancellationToken ct = default)
    {
        if (!_hasChanges)
        {
            _logger.LogInformation("No changes to commit.");
            return;
        }

        _logger.LogInformation("Commit called - saving changes to database");
        await SaveChangesAsync(ct);
        await CommitTran(ct);
        _logger.LogInformation("Commit completed successfully");
    }

    public async Task Rollback(CancellationToken ct = default)
    {
        await RollbackTran(ct);
        _logger.LogInformation("Rollback completed");
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _context.SaveChangesAsync(ct);
            _logger.LogInformation($"SaveChangesAsync completed. {result} entities saved.");
            _hasChanges = false;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveChangesAsync failed");
            throw;
        }
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> action, IsolationLevel isolation = IsolationLevel.ReadCommitted, CancellationToken ct = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async token =>
        {
            await BeginTran(isolation, token);
            try
            {
                await action(token);
                await Save(token);
                await CommitTran(token);
            }
            catch (Exception ex)
            {
                await RollbackTran(token);
                if (_translator != null)
                {
                    throw _translator.Translate(ex);
                }
                throw;
            }
        }, ct);
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, IsolationLevel isolation = IsolationLevel.ReadCommitted, CancellationToken ct = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async token =>
        {
            await BeginTran(isolation, token);

            try
            {
                var result = await action(token);
                await Save(token);
                await CommitTran(token);
                return result;
            }
            catch (Exception ex)
            {
                await RollbackTran(token);
                if (_translator != null)
                {
                    throw _translator.Translate(ex);
                }
                throw;
            }
        }, ct);
    }

    public void DetachAllEntities()
    {
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }

    private async Task<int> FlushInternalAsync(CancellationToken ct)
    {
        _context.ChangeTracker.DetectChanges();
        var result = await _context.SaveChangesAsync(ct);
        _hasChanges = false;
        _logger.LogInformation("SaveChanges SUCCESS. Rows={Rows}", result);
        return result;
    }

    public async Task Add<TEntity>(TEntity entity, CancellationToken ct = default) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _context.Set<TEntity>().AddAsync(entity, ct);
        _hasChanges = true;
        _logger.LogDebug("Entity added: {Entity}", typeof(TEntity).Name);
    }

    public async Task AddRange<TEntity>(IEnumerable<TEntity> entities, CancellationToken ct = default) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Set<TEntity>().AddRangeAsync(entities, ct);
        _logger.LogDebug("Entities added: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
    }

    public Task Update<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            var trackedEntity = _context.Set<TEntity>().Local.FirstOrDefault(e => e.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
                _context.Entry(trackedEntity).Property(x => x.xmin).OriginalValue = entity.xmin;
                _context.Entry(trackedEntity).State = EntityState.Modified;
            }
            else
            {
                _context.Set<TEntity>().Attach(entity);
                _context.Entry(entity).Property(x => x.xmin).OriginalValue = entity.xmin;
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
        else
        {
            entry.Property(x => x.xmin).OriginalValue = entity.xmin;
            entry.State = EntityState.Modified;
        }

        _logger.LogDebug("Entity updated: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        return Task.CompletedTask;
    }

    public async Task Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = true;
        entity.DateMod = DateTime.UtcNow;

        await Update(entity);
        _logger.LogDebug("Entity soft deleted: {Entity}", typeof(TEntity).Name);
    }

    public async Task Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var trackedEntity = _context.Set<TEntity>().Local.FirstOrDefault(e => e.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Deleted;
            }
            else
            {
                _context.Set<TEntity>().Attach(entity);
                _context.Entry(entity).State = EntityState.Deleted;
            }
        }
        else
        {
            entry.State = EntityState.Deleted;
        }

        _logger.LogDebug("Entity hard deleted: {Entity}", typeof(TEntity).Name);
        _hasChanges = true;
        await Task.CompletedTask;
    }

    public async Task Restore<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = false;
        entity.DateMod = DateTime.UtcNow;

        await Update(entity);
        _logger.LogDebug("Entity restored: {Entity}", typeof(TEntity).Name);
    }

    public DbSet<TEntity> Set<TEntity>() where TEntity : class => _context.Set<TEntity>();

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        await _context.DisposeAsync();
        _disposed = true;
        _logger.LogDebug("UnitOfWork disposed.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _context.Dispose();
        _disposed = true;
        _logger.LogDebug("UnitOfWork disposed synchronously.");
    }
}