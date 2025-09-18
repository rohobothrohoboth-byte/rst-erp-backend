using Cor.HRMM.Models.Entities;

namespace Cor.HRMM.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICorHRMMRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}
