namespace Svc.Lup.Interfaces;

public interface ILupRepository<T> where T : class
{
    Task<T?> GetById(Guid id);
    Task<IEnumerable<T>> GetAll();
}