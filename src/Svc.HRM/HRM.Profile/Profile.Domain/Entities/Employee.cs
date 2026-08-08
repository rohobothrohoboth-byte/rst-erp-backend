namespace Profile.Domain.Entities;

public class Employee : BaseEntity
{
    public string Code { get; private set; } = default!;
    public string EmploymentType { get; set; } = default!; //enum.EmpType
    public string EmploymentNature { get; set; } = default!; //enum.EmpNature
    public string WorkArrangement { get; set; } = default!; //enum.WorkArrangement
    public string EmpState { get; set; } = default!; //enum.EmpState
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
    public Guid PersonId { get; set; } = default!; //Person
    public Guid JobGradeId { get; set; } = default!; //Cor.HRMM.JobGrade
    public Guid PositionId { get; set; } = default!; //Cor.HRMM.Position
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
     public Guid? AppUserId { get; set; }
    //******************************************//

    public Person Person { get; set; } = null!;
    public ICollection<EmpSalary> EmpSalary { get; set; } = [];
}