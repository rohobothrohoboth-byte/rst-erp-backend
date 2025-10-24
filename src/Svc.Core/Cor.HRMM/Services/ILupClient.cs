namespace Cor.HRMM.Services;

public interface ILupClient
{
    Task<LupListDto?> EducationLevel(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> EducationLevelList(CancellationToken ct = default);




}
