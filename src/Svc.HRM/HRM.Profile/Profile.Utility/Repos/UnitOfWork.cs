using Microsoft.Extensions.Logging;
using Profile.App.Interfaces;
using Profile.Domain.Entities;
using Profile.Utility.Extensions;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace Profile.Utility.Repos;

public class UnitOfWork : IUnitOfWork
{
    private readonly DapperContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(DapperContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task Begin()
    {
        if (_transaction != null) { throw new InvalidOperationException("Transaction already started"); }

        _connection = _context.CreateConnection();
        var dbConnection = (DbConnection)_connection;
        if (dbConnection.State != ConnectionState.Open) { await dbConnection.OpenAsync(); }
        _transaction = await dbConnection.BeginTransactionAsync();
        _logger.LogInformation("Transaction started");
    }

    public IHrmProfileRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        return (IHrmProfileRepo<TEntity>)_repositories.GetOrAdd(typeof(TEntity), _ =>
        {
            var connection = _connection ?? _context.CreateConnection();
            var repoLogger = _loggerFactory.CreateLogger<HrmProfileRepo<TEntity>>();
            return new HrmProfileRepo<TEntity>(connection, _transaction, repoLogger);
        });
    }

    public async Task Commit()
    {
        if (_transaction == null) return;
        if (_transaction is DbTransaction dbTransaction) { await dbTransaction.CommitAsync(); }
        else { _transaction.Commit(); }
        await DisposeTransaction();
        _logger.LogInformation("Transaction committed");
    }

    public async Task Rollback()
    {
        if (_transaction == null) return;
        if (_transaction is DbTransaction dbTransaction) { await dbTransaction.RollbackAsync(); }
        else { _transaction.Rollback(); }
        await DisposeTransaction();
        _logger.LogInformation("Transaction rolled back");
    }

    private async Task DisposeTransaction()
    {
        if (_transaction is IAsyncDisposable asyncTx) { await asyncTx.DisposeAsync(); }
        else { _transaction?.Dispose(); }

        if (_connection is IAsyncDisposable asyncConn) { await asyncConn.DisposeAsync(); }
        else
        {
            _transaction = null;
            _connection = null;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _transaction?.Dispose();
        _connection?.Dispose();

        _disposed = true;
    }
}