// Repositories/IAggregateRepository.cs
namespace Cor.Finance.Repositories;

public interface IAggregateRepository<T> where T : class
{
    Task<T?> GetByPeriodAsync(string period, string aggregateType, CancellationToken ct = default);
    Task<List<T>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task AddAsync(T aggregate, CancellationToken ct = default);
    Task UpdateAsync(T aggregate, CancellationToken ct = default);
    Task DeleteAsync(T aggregate, CancellationToken ct = default);
}

