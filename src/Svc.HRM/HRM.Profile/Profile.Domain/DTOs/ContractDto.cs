namespace Profile.Domain.DTOs;

public sealed class ContractListDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string ContractType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Salary { get; set; }
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }
}

public sealed class ContractAddDto
{
    public Guid EmployeeId { get; set; }
    public string ContractType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Salary { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
}

public sealed class ContractModDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string ContractType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Salary { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
}
