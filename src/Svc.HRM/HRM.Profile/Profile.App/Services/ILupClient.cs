namespace Profile.App.Services;

public interface ILupClient
{
    Task<LupListDto?> Quarter(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> QuarterList(CancellationToken ct = default);

}
