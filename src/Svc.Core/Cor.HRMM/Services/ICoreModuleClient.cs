namespace Cor.HRMM.Services;

public interface ICoreModuleClient
{
    Task<LupListDto?> Department(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> DepartmentList(CancellationToken ct = default);


}
