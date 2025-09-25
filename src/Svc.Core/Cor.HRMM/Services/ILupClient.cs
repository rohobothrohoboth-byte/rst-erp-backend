namespace Cor.HRMM.Services;

public interface ILupClient
{
    Task<LupListDto?> AddressType(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> AddressTypeList(CancellationToken ct = default);
    Task<LupListDto?> Region(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> RegionList(CancellationToken ct = default);
}
