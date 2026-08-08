using System.ComponentModel.DataAnnotations.Schema;

namespace Leave.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    private DateTime _startDate;
    private DateTime _endDate;

    [Column(TypeName = "timestamp with time zone")]
    public DateTime StartDate
    {
        get => _startDate;
        set => _startDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime EndDate
    {
        get => _endDate;
        set => _endDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public double DaysRequested { get; set; }
    public bool IsHalfDay { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? DateApproved { get; set; }
    public string Comments { get; set; } = default!;
    public int CurrentAppStep { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? ApprovedById { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid? ApprovalChainId { get; set; }  // Link to the approval chain
     public Guid? CurrentStepId { get; set; }    // Current approval step ID


  public double PerApp { get; set; } = 0;
    public DateTime? DateApp { get; set; }

    public Guid BranchId { get; set; } // Cor.Module.Branch
    public Guid DeptId { get; set; } // Cor.Module.Department



// Navigation property
[ForeignKey("ApprovalChainId")]
public virtual LeaveAppChain? ApprovalChain { get; set; }
    public LeaveType LeaveType { get; set; } = null!;
}