using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAuthMngrRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}
