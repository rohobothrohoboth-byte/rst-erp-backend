namespace Profile.App.Services;

public interface ICorHRMM
{
    Task<NameList?> JobGrade(Guid id, CancellationToken ct = default);
    Task<List<NameList>?> JobGradeList(CancellationToken ct = default);
    Task<NameAmList?> Position(Guid id, CancellationToken ct = default);
    Task<List<NameAmList>?> PositionList(CancellationToken ct = default);

}
