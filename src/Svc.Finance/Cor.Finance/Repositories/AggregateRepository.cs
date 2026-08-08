// Repositories/AggregateRepository.cs
using Cor.Finance.Models.Entities.Aggregates;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Persistence;
namespace Cor.Finance.Repositories;

public class AggregateRepository<T> : IAggregateRepository<T> where T : class
{
    private readonly FinanceDbContext _context;
    private readonly DbSet<T> _dbSet;

    public AggregateRepository(FinanceDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByPeriodAsync(string period, string aggregateType, CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e =>
                EF.Property<string>(e, "Period") == period &&
                EF.Property<string>(e, "AggregateType") == aggregateType, ct);
    }

    public async Task<List<T>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(e => EF.Property<DateTime>(e, "AggregateDate") >= from &&
                        EF.Property<DateTime>(e, "AggregateDate") <= to)
            .ToListAsync(ct);
    }

    public async Task AddAsync(T aggregate, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(aggregate, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T aggregate, CancellationToken ct = default)
    {
        _dbSet.Update(aggregate);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T aggregate, CancellationToken ct = default)
    {
        _dbSet.Remove(aggregate);
        await _context.SaveChangesAsync(ct);
    }
}