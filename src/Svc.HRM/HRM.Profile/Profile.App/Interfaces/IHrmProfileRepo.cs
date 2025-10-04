using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using System.Linq.Expressions;

namespace Profile.App.Interfaces;

public interface IHrmProfileRepo<T> where T : BaseEntity
{
    Task<T?> GetById(Guid id);
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetFoD(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
    Task Add(T entity);
    Task<T> Update(T entity);
    Task Delete(Guid id);
}