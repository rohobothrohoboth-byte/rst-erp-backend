using System.Linq.Expressions;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface IAuthMngrRepo<T> where T : BaseEntity
{
    Task<T?> GetById(Guid id);
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetFoD(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
    Task Add(T entity);
    Task<T> Update(T entity);
    Task Delete(Guid id);
    //Task Assign(T entity);
}