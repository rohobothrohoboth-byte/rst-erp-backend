namespace Profile.Domain.Entities;

public class EmpContract : BaseEntity
{
    public string ContractNumber { get; set; } = default!;
    public string Status { get; set; } = default!; // ContractStatus
    public string ContractType { get; set; } = default!; // EmpNature or free text
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public DateTime? TerminatedDate { get; set; }
    public string? TerminationReason { get; set; }
    public string? DocumentRef { get; set; }
    public string? Notes { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? RenewedFromId { get; set; }

    public Employee Employee { get; set; } = null!;
}
