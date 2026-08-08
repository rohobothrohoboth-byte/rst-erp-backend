// Leave.Domain/Entities/LeaveAppChain.cs
using System;
using System.Collections.Generic;

namespace Leave.Domain.Entities;

public class LeaveAppChain : BaseEntity  // <-- ADD THIS INHERITANCE
{
    public Guid? LeavePolicyId { get; set; }  // Make nullable
    public Guid? LeaveTypeId { get; set; }    // Add for direct link
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual LeavePolicy? LeavePolicy { get; set; }
    public virtual LeaveType? LeaveType { get; set; }
    public virtual ICollection<LeaveAppStep> Steps { get; set; } = new List<LeaveAppStep>();
}