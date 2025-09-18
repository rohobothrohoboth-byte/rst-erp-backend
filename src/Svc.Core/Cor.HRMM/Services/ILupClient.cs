namespace Cor.HRMM.Services;

public interface ILupClient
{
    Task<LupListDto?> GetAddressType(Guid id);
    Task<LupListDto?> GetRegion(Guid id);
}
