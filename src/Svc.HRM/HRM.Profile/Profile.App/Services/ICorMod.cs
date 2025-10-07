namespace Profile.App.Services;

public interface ICorMod
{
    Task<NameAmList?> Dept(Guid id, CancellationToken ct = default);
    Task<List<NameAmList>?> DeptList(CancellationToken ct = default);
}
