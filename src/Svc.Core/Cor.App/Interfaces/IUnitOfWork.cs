using Cor.Domain.Entities;

namespace Cor.App.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICoreRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}
