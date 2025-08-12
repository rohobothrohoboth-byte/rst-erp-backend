using Cor.Domain.Entities;

namespace Cor.App.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICoreRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        void Begin();
        void Commit();
        void Rollback();
    }
}
