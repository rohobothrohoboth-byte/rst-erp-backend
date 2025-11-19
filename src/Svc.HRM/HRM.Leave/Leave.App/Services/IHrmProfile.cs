using Leave.Domain.DTOs;

namespace Leave.App.Services;

public interface IHrmProfile
{
    Task<NameList?> Emp(Guid id, CancellationToken ct = default);
    Task<List<NameList>?> EmpList(CancellationToken ct = default);
}