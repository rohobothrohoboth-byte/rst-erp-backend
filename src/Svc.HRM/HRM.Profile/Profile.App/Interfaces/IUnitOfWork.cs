using Profile.Domain.Entities;

namespace Profile.App.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IHrmProfileRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}