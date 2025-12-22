namespace Cor.HRMM.Services;

public interface ILup
{
    Task<LupListDto?> EducationLevel(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> EducationLevelList(CancellationToken ct = default);




}
