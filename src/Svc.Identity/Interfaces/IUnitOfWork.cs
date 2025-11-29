using Svc.Identity.Entities;
using Svc.Identity.Repos;

namespace Svc.Identity.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAuthRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}