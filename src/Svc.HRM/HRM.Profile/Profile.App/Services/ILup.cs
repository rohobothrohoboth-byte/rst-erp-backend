namespace Profile.App.Services;

public interface ILup
{
    Task<LupListDto?> Relation(Guid id, CancellationToken ct = default);
    Task<List<LupListDto>?> RelationList(CancellationToken ct = default);



}
