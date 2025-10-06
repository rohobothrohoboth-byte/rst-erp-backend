namespace Profile.App.Services;

public interface ILup
{
    Task<LupListDto?> Quarter(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> QuarterList(CancellationToken ct = default);
    Task<LupListDto?> Relation(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> RelationList(CancellationToken ct = default);
    Task<LupListDto?> MaritalStatus(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> MaritalStatusList(CancellationToken ct = default);



}
