using Recruit.Domain.Entities;

namespace Recruit.App.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IHrmRecruitRepo<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task Begin();
    Task Commit();
    Task Rollback();
}