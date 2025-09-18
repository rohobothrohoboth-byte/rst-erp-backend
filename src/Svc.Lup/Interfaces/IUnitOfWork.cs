namespace Svc.Lup.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ILupRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task Begin();
    Task Commit();
    Task Rollback();
}