namespace Profile.Domain.DTOs;

public class EmpFinanceListDto : BaseDto
{
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string EmpFullName { get; set; } = default!; //Employee
}

public class EmpFinanceAddDto
{
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpFinanceModDto
{
    public Guid Id { get; set; }
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}