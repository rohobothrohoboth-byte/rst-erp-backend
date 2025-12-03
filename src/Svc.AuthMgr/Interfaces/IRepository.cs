using System.Linq.Expressions;

namespace Svc.AuthMgr.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T?>> GetAll();
    Task<T?> GetById(Guid id);
    T? GetFoD(Expression<Func<T?, bool>> predicate);
    Task Add(T? entity);
    Task Remove(T? entity);
    Task Update(T entity);
    Task<IEnumerable<T?>> Find(Expression<Func<T?, bool>> predicate);
}