using Cor.Module.Models.Entities;

namespace Cor.Module.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICoreModuleRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}
