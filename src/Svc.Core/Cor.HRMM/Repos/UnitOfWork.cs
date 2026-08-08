using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Persistence;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;

namespace Cor.HRMM.Repos;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly coreHRMMDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly UnitOfWorkOptions _options;
    private readonly IDbExceptionTranslator _translator;
    private IDbContextTransaction? _transaction;
    private readonly Stack<string> _savepoints = new();
    private int _transactionDepth;
    private bool _disposed;

    public IDbConnection Connection => _context.Database.GetDbConnection();
    public IDbTransaction? Transaction => _transaction?.GetDbTransaction();

    public UnitOfWork(
        coreHRMMDbContext context,
        ILogger<UnitOfWork> logger,
        IOptions<UnitOfWorkOptions> options,
        IDbExceptionTranslator translator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new UnitOfWorkOptions();
        _translator = translator ?? throw new ArgumentNullException(nameof(translator));
    }

    public bool HasChanges => _context.ChangeTracker.HasChanges();
    public bool HasActiveTransaction => _transaction != null;

    public DbSet<TEntity> Set<TEntity>() where TEntity : class => _context.Set<TEntity>();
    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class => _context.Entry(entity);

    #region Save Methods with Retry

   public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
   {
       if (!HasChanges)
           return 0;

       return await SaveInternalAsync(cancellationToken);
   }



    private async Task<int> SaveInternalAsync(CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            sw.Stop();

            if (sw.ElapsedMilliseconds > _options.SlowSaveThresholdMilliseconds)
            {
                LogSlowSave(sw.ElapsedMilliseconds);
            }

            if (_options.ClearChangeTrackerAfterSave)
            {
                _context.ChangeTracker.Clear();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Database exception during save.");
            throw;
        }
    }

    private void LogSlowSave(long ms)
    {
        var stats = _context.ChangeTracker
            .Entries()
            .GroupBy(e => e.State)
            .ToDictionary(g => g.Key, g => g.Count());

        _logger.LogWarning(
            "Slow SaveChanges detected: {Elapsed}ms | Added={Added} Modified={Modified} Deleted={Deleted}",
            ms,
            stats.GetValueOrDefault(EntityState.Added),
            stats.GetValueOrDefault(EntityState.Modified),
            stats.GetValueOrDefault(EntityState.Deleted));
    }

    #endregion

    #region Transaction Management

   public async Task<IDbContextTransaction> BeginTransactionAsync(
       IsolationLevel isolation = IsolationLevel.ReadCommitted,
       CancellationToken cancellationToken = default)
   {
       if (_transaction == null)
       {
           _transaction = await _context.Database.BeginTransactionAsync(isolation, cancellationToken);
           _transactionDepth = 1;
           _savepoints.Clear();
           return _transaction;
       }

       _transactionDepth++;

       var savepoint = $"sp_{_transactionDepth}_{Guid.NewGuid():N}";

       await _transaction.CreateSavepointAsync(savepoint, cancellationToken);

       _savepoints.Push(savepoint);

       return _transaction;
   }

   public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
   {
       if (_transaction == null)
           return;

       _transactionDepth--;

       if (_transactionDepth > 0)
           return;

       try
       {
           await _context.SaveChangesAsync(cancellationToken);

           await _transaction.CommitAsync(cancellationToken);

           if (_options.ClearChangeTrackerAfterSave)
               _context.ChangeTracker.Clear();
       }
       catch
       {
           await _transaction.RollbackAsync(cancellationToken);
           throw;
       }
       finally
       {
           await DisposeTransactionAsync();
       }
   }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No active transaction to rollback");
            return;
        }

        try
        {
            if (_savepoints.Count > 0)
            {
                var savepoint = _savepoints.Pop();
                await _transaction.RollbackToSavepointAsync(savepoint, cancellationToken);
                _transactionDepth--;
                _logger.LogDebug("Rolled back to savepoint: {Savepoint}", savepoint);
                return;
            }

            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogWarning("Transaction rolled back");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to rollback transaction");
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        _transactionDepth = 0;
        _savepoints.Clear();
    }

    #endregion

    #region Execute Methods with Retry

   public async Task ExecuteAsync(
       Func<CancellationToken, Task> action,
       IsolationLevel isolation = IsolationLevel.ReadCommitted,
       CancellationToken ct = default)
   {
       var strategy = _context.Database.CreateExecutionStrategy();

       await strategy.ExecuteAsync(async () =>
       {
           await using var transaction =
               await _context.Database.BeginTransactionAsync(isolation, ct);

           try
           {
               await action(ct);

               await _context.SaveChangesAsync(ct);

               await transaction.CommitAsync(ct);

               if (_options.ClearChangeTrackerAfterSave)
                   _context.ChangeTracker.Clear();
           }
           catch
           {
               await transaction.RollbackAsync(ct);
               throw;
           }
       });
   }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        IsolationLevel isolation = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(isolation, ct);

            try
            {
                var result = await action(ct);

                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                if (_options.ClearChangeTrackerAfterSave)
                    _context.ChangeTracker.Clear();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }


    #endregion

    #region CRUD Operations

    public async Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public void Update<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _context.Attach(entity);
        }
        entry.Property(nameof(BaseEntity.xmin)).OriginalValue = entity.xmin;
        entry.State = EntityState.Modified;
    }

    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        foreach (var entity in entities)
        {
            Update(entity);
        }
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = true;
        entity.DateMod = DateTime.UtcNow;
        var entry = _context.Attach(entity);
        entry.Property(nameof(BaseEntity.xmin)).OriginalValue = entity.xmin;
        entry.State = EntityState.Modified;
    }

    public void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DateMod = now;
            var entry = _context.Attach(entity);
            entry.Property(nameof(BaseEntity.xmin)).OriginalValue = entity.xmin;
            entry.State = EntityState.Modified;
        }
    }

    public void Restore<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = false;
        entity.DateMod = DateTime.UtcNow;
        var entry = _context.Attach(entity);
        entry.Property(nameof(BaseEntity.xmin)).OriginalValue = entity.xmin;
        entry.State = EntityState.Modified;
    }

    public void RestoreRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        var now = DateTime.UtcNow;
        foreach (var entity in entities)
        {
            entity.IsDeleted = false;
            entity.DateMod = now;
            var entry = _context.Attach(entity);
            entry.Property(nameof(BaseEntity.xmin)).OriginalValue = entity.xmin;
            entry.State = EntityState.Modified;
        }
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = _context.Attach(entity);
        entry.Property(nameof(BaseEntity.xmin)).OriginalValue = entity.xmin;
        entry.State = EntityState.Deleted;
    }

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        foreach (var entity in entities)
        {
            var entry = _context.Attach(entity);
            entry.Property(nameof(BaseEntity.xmin)).OriginalValue = entity.xmin;
            entry.State = EntityState.Deleted;
        }
    }

    #endregion

    #region Helpers

    public void Attach<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Attach(entity);
    }

    public void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Entry(entity).State = EntityState.Detached;
    }

    public void ClearChangeTracker()
    {
        _context.ChangeTracker.Clear();
        _logger.LogDebug("Change tracker cleared");
    }

    public int GetTrackedEntityCount()
    {
        return _context.ChangeTracker.Entries().Count();
    }

    #endregion

    #region Dispose Pattern

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during disposal");
        }
        finally
        {
            _transaction = null;
            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            await _context.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during async disposal");
        }
        finally
        {
            _disposed = true;
        }
    }

    #endregion
}

public class UnitOfWorkOptions
{
    public int SlowSaveThresholdMilliseconds { get; set; } = 5000;
    public bool ClearChangeTrackerAfterSave { get; set; } = true;
    public bool EnableDetailedLogging { get; set; } = false;
    public int MaxRetryCount { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 100;
}