namespace Cor.HRMM.Services;

public interface ILupClient
{
    Task<LupListDto?> AddressType(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> AddressTypeList(CancellationToken ct = default);
    Task<LupListDto?> Region(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> RegionList(CancellationToken ct = default);
    Task<LupListDto?> EducationLevel(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> EducationLevelList(CancellationToken ct = default);
    Task<LupListDto?> ProfessionType(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> ProfessionTypeList(CancellationToken ct = default);




}
