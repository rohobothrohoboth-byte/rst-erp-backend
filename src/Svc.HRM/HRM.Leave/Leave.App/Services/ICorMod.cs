using Leave.Domain.DTOs;

namespace Leave.App.Services;

public interface ICorMod
{
    Task<NameList?> FiscalYear(Guid id, CancellationToken ct = default);
    Task<List<NameList>?> FiscalYearList(CancellationToken ct = default);
}