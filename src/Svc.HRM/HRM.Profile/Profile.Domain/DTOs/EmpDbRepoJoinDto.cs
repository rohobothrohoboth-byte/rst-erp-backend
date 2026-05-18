namespace Profile.Domain.DTOs;

public class EmpStateList
{
    public string EmpState { get; set; } = default!;
}

public class EmpDbPendJoin
{
    public Guid DepartmentId { get; init; }
    public Guid JobGradeId { get; init; }
    public Guid PositionId { get; init; }
    public string FirstName { get; init; } = default!;
    public string MiddleName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string FirstNameAm { get; init; } = default!;
    public string MiddleNameAm { get; init; } = default!;
    public string LastNameAm { get; init; } = default!;
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
}