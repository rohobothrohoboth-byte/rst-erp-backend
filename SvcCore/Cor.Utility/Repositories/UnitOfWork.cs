using Cor.App.Interfaces;
using Cor.Domain.Entities;
using Cor.Utility.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Data;

namespace Cor.Utility.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DapperContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private IDbTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(DapperContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _loggerFactory = loggerFactory;
            _repositories = new ConcurrentDictionary<Type, object>();
        }

        public ICoreRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            return (ICoreRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity), (type) =>
            {
                var logger = _loggerFactory.CreateLogger<CoreRepository<TEntity>>();
                return new CoreRepository<TEntity>(_context, logger);
            });
        }
        
        public void Begin()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Transaction already started.");
            }

            var connection = _context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            _transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }
        
        public void Dispose()
        {
            if (_disposed) return;

            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
