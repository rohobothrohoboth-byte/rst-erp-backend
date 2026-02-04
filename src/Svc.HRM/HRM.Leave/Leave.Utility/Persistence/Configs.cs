using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leave.Utility.Persistence;

public class AccrualHistoryConf : IEntityTypeConfiguration<AccrualHistory>
{
    public void Configure(EntityTypeBuilder<AccrualHistory> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyId, x.LeaveLedgerId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.LeaveLedger).WithMany().HasForeignKey(e => e.LeaveLedgerId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.AccruedAmount).HasPrecision(18, 2);
        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.PeriodStart, e.PeriodEnd });
        b.HasIndex(e => e.Frequency);
    }
}

public class AttachmentConf : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.LeaveRequestId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class AttachmentBlobConf : IEntityTypeConfiguration<AttachmentBlob>
{
    public void Configure(EntityTypeBuilder<AttachmentBlob> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.AttachmentId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class EmpLeavePolicyConf : IEntityTypeConfiguration<EmpLeavePolicy>
{
    public void Configure(EntityTypeBuilder<EmpLeavePolicy> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.EmployeeId, x.LeavePolicyId });
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.AssignedEntitlement).HasPrecision(18, 2);
        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.EffectiveFrom });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class EncashmentAppActionConf : IEntityTypeConfiguration<EncashmentAppAction>
{
    public void Configure(EntityTypeBuilder<EncashmentAppAction> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.LeaveEncashmentId, x.ApprovedById });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveEncashment).WithMany().HasForeignKey(e => e.LeaveEncashmentId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(e => new { e.LeaveEncashmentId, e.StepOrder });
    }
}

public class LeaveAppActionConf : IEntityTypeConfiguration<LeaveAppAction>
{
    public void Configure(EntityTypeBuilder<LeaveAppAction> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.LeaveRequestId, x.ApprovedById });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveRequest).WithMany().HasForeignKey(e => e.LeaveRequestId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(e => new { e.LeaveRequestId, e.StepOrder });
    }
}

public class LeaveAppChainConf : IEntityTypeConfiguration<LeaveAppChain>
{
    public void Configure(EntityTypeBuilder<LeaveAppChain> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.LeavePolicyId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(e => e.Steps).WithOne().HasForeignKey("LeaveAppChainId").OnDelete(DeleteBehavior.Cascade);
    }
}

public class LeaveAppStepConf : IEntityTypeConfiguration<LeaveAppStep>
{
    public void Configure(EntityTypeBuilder<LeaveAppStep> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => x.StepName);
        //b.HasIndex(x => new { x.LeaveAppChainId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();

        b.HasKey(x => x.Id);
        b.Property(x => x.StepOrder).IsRequired();
        b.Property(x => x.Role).IsRequired();
        b.HasOne(x => x.LeaveAppChain).WithMany(c => c.Steps).HasForeignKey(x => x.LeaveAppChainId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.LeaveAppChainId, x.StepOrder }).IsUnique();
    }
}

public class LeaveBalanceConf : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeaveLedgerId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.LeaveLedger).WithMany().HasForeignKey(e => e.LeaveLedgerId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.Balance).HasPrecision(18, 2);
        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId }).IsUnique();
    }
}

public class LeaveEncashmentConf : IEntityTypeConfiguration<LeaveEncashment>
{
    public void Configure(EntityTypeBuilder<LeaveEncashment> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.DaysEncashed).HasPrecision(18, 2);
        b.Property(e => e.RatePerDay).HasPrecision(18, 2);
        b.Property(e => e.TotalAmount).HasPrecision(18, 2);
        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.Status });
    }
}

public class LeaveLedgerConf : IEntityTypeConfiguration<LeaveLedger>
{
    public void Configure(EntityTypeBuilder<LeaveLedger> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyId, x.ReferenceId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.Amount).HasPrecision(18, 2);
        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.Date });
        b.HasIndex(e => e.ReferenceId);
        b.HasIndex(e => new { e.SourceType, e.Date });
    }
}

public class LeavePolicyConf : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(EntityTypeBuilder<LeavePolicy> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Code).IsUnique();
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.LeaveTypeId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(e => e.Code).IsUnique();
        b.HasIndex(e => e.Status);
    }
}

public class LeavePolicyConfigConf : IEntityTypeConfiguration<LeavePolicyConfig>
{
    public void Configure(EntityTypeBuilder<LeavePolicyConfig> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.FiscalYearId, x.LeavePolicyId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.AnnualEntitlement).HasPrecision(18, 2);
        b.Property(e => e.AccrualRate).HasPrecision(18, 2);
        b.Property(e => e.MaxDaysPerReq).HasPrecision(18, 2);
        b.Property(e => e.MaxCarryOverDays).HasPrecision(18, 2);
        b.HasIndex(e => new { e.LeavePolicyId, e.FiscalYearId, e.IsActive });
    }
}

public class LeaveRequestConf : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> b)
    {
        //b.HasKey(x => x.Id);
        //b.HasIndex(x => x.Id).IsUnique();
        //b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.ApprovedById, x.Status });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasKey(e => e.Id);
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.DaysRequested).HasPrecision(18, 2);
        b.HasIndex(e => new { e.EmployeeId, e.StartDate, e.EndDate });
        b.HasIndex(e => e.Status);
    }
}

public class LeaveTypeConf : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.HasIndex(x => x.Name).IsUnique();
        b.HasIndex(x => new { x.LeaveCategory, x.IsActive });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class PolicyAssignmentRuleConf : IEntityTypeConfiguration<PolicyAssignmentRule>
{
    public void Configure(EntityTypeBuilder<PolicyAssignmentRule> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => x.Priority);
        b.HasIndex(x => new { x.LeavePolicyId, x.LeaveTypeId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class PolicyRuleConditionConf : IEntityTypeConfiguration<PolicyRuleCondition>
{
    public void Configure(EntityTypeBuilder<PolicyRuleCondition> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.PolicyAssignmentRuleId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}