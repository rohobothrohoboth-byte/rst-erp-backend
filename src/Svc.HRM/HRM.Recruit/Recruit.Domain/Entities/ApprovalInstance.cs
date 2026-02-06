namespace Recruit.Domain.Entities;

public class ApprovalInstance : BaseEntity
{
    public string EntityType { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.ApprovalStatus(0/1)
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? Comments { get; set; } = default!;
    public int SequenceNumber { get; set; }
    public Guid ApprovalStepId { get; set; } // ApprovalStep
    public Guid EntityId { get; set; }
    public Guid? ApproverId { get; set; } // HRM.Profile.Employee

    //******************************************//

    public ApprovalStep ApprovalStep { get; set; } = null!;
}