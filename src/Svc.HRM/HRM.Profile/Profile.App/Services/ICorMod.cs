namespace Profile.App.Services;

public interface ICorMod
{
    Task<NameAmListDto?> Dept(Guid id, CancellationToken ct = default);
    Task<List<NameAmListDto>?> DeptList(CancellationToken ct = default);
}
