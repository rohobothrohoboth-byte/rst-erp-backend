using Microsoft.EntityFrameworkCore;
using Leave.Domain.Entities;
using System.Data;

namespace Leave.App.Interfaces;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{

     IDbConnection? Connection { get; }
    IDbTransaction? Transaction { get; }

    Task Begin(CancellationToken ct = default);
    Task Commit(CancellationToken ct = default);
    Task Rollback(CancellationToken ct = default);
    void DetachAllEntities();

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task Add<TEntity>(TEntity entity, CancellationToken ct = default) where TEntity : class;
    Task AddRange<TEntity>(IEnumerable<TEntity> entities, CancellationToken ct = default) where TEntity : class;
    Task Update<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task Remove<TEntity>(TEntity entity) where TEntity : BaseEntity; // Hard Delete
    Task Restore<TEntity>(TEntity entity) where TEntity : BaseEntity;
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default);
     Task ExecuteAsync(Func<CancellationToken, Task> action, IsolationLevel isolation = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
     Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, IsolationLevel isolation = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

}