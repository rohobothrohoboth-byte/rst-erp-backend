using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Svc.AuthMgr.Interfaces;
using Svc.AuthMgr.Persistence;

namespace Svc.AuthMgr.Repos;

public class Repository<T> : IRepository<T> where T : class
{
    protected AuthDbContext _context;
    protected DbSet<T> _dbSet;

    public Repository(AuthDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T?>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetById(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    async Task IRepository<T>.Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T?>> Find(Expression<Func<T?, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    async Task IRepository<T>.Add(T? entity)
    {
        if (entity != null) _dbSet.Add(entity);
        await _context.SaveChangesAsync();
    }

    async Task IRepository<T>.Remove(T? entity)
    {
        if (entity != null)
        {
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        await _context.SaveChangesAsync();
    }

    T? IRepository<T>.GetFoD(Expression<Func<T?, bool>> predicate)
    {
        return _dbSet.FirstOrDefault(predicate);
    }
}