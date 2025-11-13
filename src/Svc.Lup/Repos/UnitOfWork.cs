using Npgsql;
using Svc.Lup.Extensions;
using Svc.Lup.Interfaces;
using System.Collections.Concurrent;
using System.Data;

namespace Svc.Lup.Repos;

public class UnitOfWork(DapperContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory) : IUnitOfWork
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<UnitOfWork> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private NpgsqlTransaction? _transaction;
    private bool _disposed;

    public ILupRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        return (ILupRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity), type =>
        {
            _logger.LogInformation("Creating new LupRepository<{EntityType}> instance for UnitOfWork.", typeof(TEntity).Name);
            var repoLogger = _loggerFactory.CreateLogger<LupRepository<TEntity>>();
            return new LupRepository<TEntity>(_context, repoLogger);
        });
    }

    public async Task Begin()
    {
        if (_transaction != null)
        {
            _logger.LogError("Transaction already started.");
            throw new InvalidOperationException("Transaction already started.");
        }

        var connection = _context.CreateConnection() as NpgsqlConnection;
        if (connection?.State != ConnectionState.Open)
        {
            _logger.LogInformation("Opening database connection for transaction.");
            await connection!.OpenAsync();
        }
        _transaction = await connection.BeginTransactionAsync();
        _logger.LogInformation("Started new database transaction.");
    }

    public async Task Commit()
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No transaction to commit.");
            return;
        }
        try
        {
            _logger.LogInformation("Committing transaction.");
            await _transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction.");
            throw;
        }
        finally
        {
            await DisposeTransaction();
        }
    }

    public async Task Rollback()
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No transaction to rollback.");
            return;
        }
        try
        {
            _logger.LogInformation("Rolling back transaction.");
            await _transaction.RollbackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction.");
            throw;
        }
        finally
        {
            await DisposeTransaction();
        }
    }

    private async Task DisposeTransaction()
    {
        if (_transaction != null)
        {
            _logger.LogInformation("Disposing transaction.");
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _logger.LogInformation("Disposing UnitOfWork and DapperContext.");
        _transaction?.Dispose();
        _context.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
