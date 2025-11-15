using Leave.Domain.Entities;

namespace Leave.App.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IHrmLeaveRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}