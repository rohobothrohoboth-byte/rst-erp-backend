using Module.Domain.Entities;

namespace Module.App.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICoreModuleRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}
