using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Cor.HRMM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cor.HRMM.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {

    IDbConnection Connection { get; }  // Add this
        IDbTransaction? Transaction { get; }  // Add this
        // Properties
        bool HasChanges { get; }
        bool HasActiveTransaction { get; }

        // Save Methods
        Task<int> SaveAsync(CancellationToken cancellationToken = default);

        // Transaction Management
        Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolation = IsolationLevel.ReadCommitted, 
            CancellationToken cancellationToken = default);
        
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        // Execute Methods - Transaction Wrappers
        Task ExecuteAsync(
            Func<CancellationToken, Task> action, 
            IsolationLevel isolation = IsolationLevel.ReadCommitted, 
            CancellationToken ct = default);
        
        Task<TResult> ExecuteAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action, 
            IsolationLevel isolation = IsolationLevel.ReadCommitted, 
            CancellationToken ct = default);

        // CRUD Operations
        Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
        Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class;
        
        void Update<TEntity>(TEntity entity) where TEntity : BaseEntity;
        void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;
        
        void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;
        void DeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;
        
        void Restore<TEntity>(TEntity entity) where TEntity : BaseEntity;
        void RestoreRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;
        
        void Remove<TEntity>(TEntity entity) where TEntity : BaseEntity;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        // Helpers
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        
        void Attach<TEntity>(TEntity entity) where TEntity : BaseEntity;
        void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity;
        
        void ClearChangeTracker();
        int GetTrackedEntityCount();
    }
}