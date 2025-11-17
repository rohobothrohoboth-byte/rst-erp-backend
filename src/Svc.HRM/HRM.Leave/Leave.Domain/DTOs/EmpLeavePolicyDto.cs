namespace Leave.Domain.DTOs;

public class EmpLeavePolicyListDto : BaseDto
{
    public Guid EmployeeId { get; set; } //HRM.Profile.Employee
    public Guid LeavePolicyId { get; set; } //LeavePolicy
    public string EmployeeName { get; set; } = default!;
    public string LeavePolicy { get; set; } = default!;
}

public class EmpLeavePolicyAddDto
{
    public Guid EmployeeId { get; set; } //HRM.Profile.Employee
    public Guid LeavePolicyId { get; set; } //LeavePolicy
}

public class EmpLeavePolicyModDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; } //HRM.Profile.Employee
    public Guid LeavePolicyId { get; set; } //LeavePolicy
    public string RowVersion { get; set; } = default!;
}