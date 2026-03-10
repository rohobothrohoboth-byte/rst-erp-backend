using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leave.Utility.Persistence;

public abstract class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.DateAdd).IsRequired().HasColumnType("timestamp with time zone");
        b.Property(x => x.DateMod).HasColumnType("timestamp with time zone");
        b.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        b.Property(x => x.xmin).HasColumnName("xmin").HasColumnType("xid").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
        b.HasIndex(x => x.IsDeleted);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class AccrualHistoryConfig : BaseEntityConfig<AccrualHistory>
{
    public override void Configure(EntityTypeBuilder<AccrualHistory> b)
    {
        base.Configure(b);
        b.Property(a => a.Frequency).IsRequired().HasMaxLength(20);
        b.Property(a => a.AccruedAmount).IsRequired().HasColumnType("decimal(18,2)");
        b.Property(a => a.PeriodStart).IsRequired();
        b.Property(a => a.PeriodEnd).IsRequired();
        b.Property(a => a.Source).IsRequired().HasMaxLength(20);
        b.Property(a => a.EmployeeId).IsRequired();
        b.Property(a => a.LeaveTypeId).IsRequired();
        b.Property(a => a.LeavePolicyId).IsRequired();
        b.Property(a => a.LeaveLedgerId).IsRequired();
        b.HasOne(a => a.LeaveType).WithMany().HasForeignKey(a => a.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.LeavePolicy).WithMany().HasForeignKey(a => a.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.LeaveLedger).WithMany().HasForeignKey(a => a.LeaveLedgerId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(a => a.EmployeeId);
        b.HasIndex(a => a.LeaveTypeId);
        b.HasIndex(a => new { a.EmployeeId, a.LeaveTypeId, a.PeriodStart }).IsUnique();
    }
}

public class AttachmentConfig : BaseEntityConfig<Attachment>
{
    public override void Configure(EntityTypeBuilder<Attachment> b)
    {
        base.Configure(b);
        b.Property(a => a.FileName).IsRequired().HasMaxLength(250);
        b.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
        b.Property(a => a.FileSize).IsRequired();
        b.Property(a => a.DateUpload).IsRequired();
        b.Property(a => a.LeaveRequestId).IsRequired();
        b.HasOne(a => a.LeaveRequest).WithMany().HasForeignKey(a => a.LeaveRequestId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(a => a.LeaveRequestId);
    }
}

public class AttachmentBlobConfig : BaseEntityConfig<AttachmentBlob>
{
    public override void Configure(EntityTypeBuilder<AttachmentBlob> b)
    {
        base.Configure(b);
        b.Property(ab => ab.Data).IsRequired();
        b.Property(ab => ab.AttachmentId).IsRequired();
        b.HasOne(ab => ab.Attachment).WithMany().HasForeignKey(ab => ab.AttachmentId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(ab => ab.AttachmentId).IsUnique();
    }
}

public class EmpLeavePolicyConfig : BaseEntityConfig<EmpLeavePolicy>
{
    public override void Configure(EntityTypeBuilder<EmpLeavePolicy> b)
    {
        base.Configure(b);
        b.Property(e => e.EffectiveFrom).IsRequired();
        b.Property(e => e.EffectiveTo);
        b.Property(e => e.AssignedEntitlement).IsRequired();
        b.Property(e => e.Reason).IsRequired().HasMaxLength(50);
        b.Property(e => e.EmployeeId).IsRequired();
        b.Property(e => e.LeaveTypeId).IsRequired();
        b.Property(e => e.LeavePolicyId).IsRequired();
        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(e => e.EmployeeId);
        b.HasIndex(e => e.LeavePolicyId);
        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.LeavePolicyId }).IsUnique();
    }
}

public class EncashmentAppActionConfig : BaseEntityConfig<EncashmentAppAction>
{
    public override void Configure(EntityTypeBuilder<EncashmentAppAction> b)
    {
        base.Configure(b);
        b.Property(e => e.StepOrder).IsRequired();
        b.Property(e => e.Role).IsRequired().HasMaxLength(50);
        b.Property(e => e.Action).IsRequired().HasMaxLength(50);
        b.Property(e => e.Comment).HasMaxLength(500);
        b.Property(e => e.ActionAt).IsRequired();
        b.Property(e => e.LeaveEncashmentId).IsRequired();
        b.Property(e => e.ApprovedById).IsRequired();
        b.HasOne(e => e.LeaveEncashment).WithMany().HasForeignKey(e => e.LeaveEncashmentId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(e => e.LeaveEncashmentId);
        b.HasIndex(e => e.ApprovedById);
        b.HasIndex(e => new { e.LeaveEncashmentId, e.StepOrder }).IsUnique();
    }
}

public class LeaveAppActionConfig : BaseEntityConfig<LeaveAppAction>
{
    public override void Configure(EntityTypeBuilder<LeaveAppAction> b)
    {
        base.Configure(b);
        b.Property(l => l.StepOrder).IsRequired();
        b.Property(l => l.Role).IsRequired().HasMaxLength(50);
        b.Property(l => l.Action).IsRequired().HasMaxLength(50);
        b.Property(l => l.Comment).HasMaxLength(500);
        b.Property(l => l.ActionAt).IsRequired();
        b.Property(l => l.LeaveRequestId).IsRequired();
        b.Property(l => l.ApprovedById).IsRequired();
        b.HasOne(l => l.LeaveRequest).WithMany().HasForeignKey(l => l.LeaveRequestId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(l => l.LeaveRequestId);
        b.HasIndex(l => l.ApprovedById);
        b.HasIndex(l => new { l.LeaveRequestId, l.StepOrder }).IsUnique();
    }
}

public class LeaveAppChainConfig : BaseEntityConfig<LeaveAppChain>
{
    public override void Configure(EntityTypeBuilder<LeaveAppChain> b)
    {
        base.Configure(b);
        b.Property(l => l.LeavePolicyId).IsRequired();
        b.Property(l => l.EffectiveFrom).IsRequired();
        b.Property(l => l.EffectiveTo);
        b.Property(l => l.IsActive).IsRequired();
        b.HasOne(l => l.LeavePolicy).WithMany().HasForeignKey(l => l.LeavePolicyId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(l => l.LeavePolicyId);
        b.HasIndex(l => new { l.LeavePolicyId, l.EffectiveFrom }).IsUnique();
    }
}

public class LeaveAppStepConfig : BaseEntityConfig<LeaveAppStep>
{
    public override void Configure(EntityTypeBuilder<LeaveAppStep> b)
    {
        base.Configure(b);
        b.Property(l => l.StepName).IsRequired().HasMaxLength(100);
        b.Property(l => l.StepOrder).IsRequired();
        b.Property(l => l.Role).IsRequired().HasMaxLength(50);
        b.Property(l => l.EmployeeId);
        b.Property(l => l.IsFinal).IsRequired();
        b.Property(l => l.LeaveAppChainId).IsRequired();
        b.HasOne(l => l.LeaveAppChain).WithMany(c => c.Steps).HasForeignKey(l => l.LeaveAppChainId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(l => l.LeaveAppChainId);
        b.HasIndex(l => new { l.LeaveAppChainId, l.StepOrder }).IsUnique();
    }
}

public class LeaveBalanceConfig : BaseEntityConfig<LeaveBalance>
{
    public override void Configure(EntityTypeBuilder<LeaveBalance> b)
    {
        base.Configure(b);
        b.Property(l => l.Balance).IsRequired();
        b.Property(l => l.AsOf).IsRequired();
        b.Property(l => l.EmployeeId).IsRequired();
        b.Property(l => l.LeaveTypeId).IsRequired();
        b.Property(l => l.LeaveLedgerId);
        b.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(l => l.LeaveLedger).WithMany().HasForeignKey(l => l.LeaveLedgerId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(l => l.EmployeeId);
        b.HasIndex(l => l.LeaveTypeId);
        b.HasIndex(l => new { l.EmployeeId, l.LeaveTypeId }).IsUnique();
    }
}

public class LeaveEncashmentConfig : BaseEntityConfig<LeaveEncashment>
{
    public override void Configure(EntityTypeBuilder<LeaveEncashment> b)
    {
        base.Configure(b);
        b.Property(l => l.DaysEncashed).IsRequired();
        b.Property(l => l.RatePerDay).IsRequired();
        b.Property(l => l.TotalAmount).IsRequired();
        b.Property(l => l.Status).IsRequired().HasMaxLength(50);
        b.Property(l => l.CurrentAppStep).IsRequired();
        b.Property(l => l.EmployeeId).IsRequired();
        b.Property(l => l.LeaveTypeId).IsRequired();
        b.Property(l => l.LeavePolicyId);
        b.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(l => l.LeavePolicy).WithMany().HasForeignKey(l => l.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(l => l.EmployeeId);
        b.HasIndex(l => l.LeaveTypeId);
        b.HasIndex(l => l.LeavePolicyId);
    }
}

public class LeaveLedgerConfig : BaseEntityConfig<LeaveLedger>
{
    public override void Configure(EntityTypeBuilder<LeaveLedger> b)
    {
        base.Configure(b);
        b.Property(l => l.Date).IsRequired();
        b.Property(l => l.Amount).IsRequired();
        b.Property(l => l.EntryType).IsRequired().HasMaxLength(50);
        b.Property(l => l.SourceType).IsRequired().HasMaxLength(50);
        b.Property(l => l.EmployeeId).IsRequired();
        b.Property(l => l.LeaveTypeId).IsRequired();
        b.Property(l => l.LeavePolicyId);
        b.Property(l => l.ReferenceId);
        b.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(l => l.LeavePolicy).WithMany().HasForeignKey(l => l.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(l => l.EmployeeId);
        b.HasIndex(l => l.LeaveTypeId);
        b.HasIndex(l => new { l.EmployeeId, l.LeaveTypeId, l.Date }).IsUnique();
    }
}

public class LeavePolicyConfigu : BaseEntityConfig<LeavePolicy>
{
    public override void Configure(EntityTypeBuilder<LeavePolicy> b)
    {
        base.Configure(b);
        b.Property(l => l.Code).IsRequired().HasMaxLength(50);
        b.Property(l => l.Name).IsRequired().HasMaxLength(150);
        b.Property(l => l.AllowEncashment).IsRequired();
        b.Property(l => l.RequiresAttachment).IsRequired();
        b.Property(l => l.Status).IsRequired().HasMaxLength(20);
        b.Property(l => l.LeaveTypeId).IsRequired();
        b.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(l => l.LeaveTypeId).HasFilter("\"IsDeleted\" = false");
        b.HasIndex(l => l.Code).IsUnique();
        b.HasIndex(l => l.Name).IsUnique();
    }
}

public class LeavePolicyConfigConfigu : BaseEntityConfig<LeavePolicyConfig>
{
    public override void Configure(EntityTypeBuilder<LeavePolicyConfig> b)
    {
        base.Configure(b);
        b.Property(l => l.AnnualEntitlement).IsRequired();
        b.Property(l => l.AccrualFrequency).IsRequired().HasMaxLength(20);
        b.Property(l => l.AccrualRate).IsRequired();
        b.Property(l => l.MaxDaysPerReq).IsRequired();
        b.Property(l => l.MaxCarryOverDays).IsRequired();
        b.Property(l => l.MinServiceMonths).IsRequired();
        b.Property(l => l.IsActive).IsRequired();
        b.Property(l => l.FiscalYearId).IsRequired();
        b.Property(l => l.LeavePolicyId).IsRequired();
        b.HasOne(l => l.LeavePolicy).WithMany().HasForeignKey(l => l.LeavePolicyId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(l => l.FiscalYearId);
        b.HasIndex(l => l.LeavePolicyId).IsUnique();
    }
}

public class LeaveRequestConfig : BaseEntityConfig<LeaveRequest>
{
    public override void Configure(EntityTypeBuilder<LeaveRequest> b)
    {
        base.Configure(b);
        b.Property(l => l.StartDate).IsRequired();
        b.Property(l => l.EndDate).IsRequired();
        b.Property(l => l.DaysRequested).IsRequired();
        b.Property(l => l.IsHalfDay).IsRequired();
        b.Property(l => l.Status).IsRequired().HasMaxLength(20);
        b.Property(l => l.DateApproved);
        b.Property(l => l.Comments).HasMaxLength(500);
        b.Property(l => l.CurrentAppStep).IsRequired();
        b.Property(l => l.EmployeeId).IsRequired();
        b.Property(l => l.ApprovedById);
        b.Property(l => l.LeaveTypeId).IsRequired();
        b.HasOne(l => l.LeaveType).WithMany().HasForeignKey(l => l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(l => l.EmployeeId);
        b.HasIndex(l => l.LeaveTypeId);
        b.HasIndex(l => new { l.EmployeeId, l.LeaveTypeId, l.StartDate }).IsUnique();
    }
}

public class LeaveTypeConfig : BaseEntityConfig<LeaveType>
{
    public override void Configure(EntityTypeBuilder<LeaveType> b)
    {
        base.Configure(b);
        b.Property(l => l.Name).IsRequired().HasMaxLength(150);
        b.Property(l => l.LeaveCategory).IsRequired().HasMaxLength(50);
        b.Property(l => l.RequiresApproval).IsRequired();
        b.Property(l => l.AllowHalfDay).IsRequired();
        b.Property(l => l.HolidaysAsLeave).IsRequired();
        b.Property(l => l.IsActive).IsRequired();
        b.HasIndex(l => l.Name).IsUnique();
        b.HasIndex(l => l.LeaveCategory);
        b.HasIndex(l => l.IsActive);
    }
}

public class PolicyAssRuleConfig : BaseEntityConfig<PolicyAssignmentRule>
{
    public override void Configure(EntityTypeBuilder<PolicyAssignmentRule> b)
    {
        base.Configure(b);
        b.Property(p => p.Code).IsRequired().HasMaxLength(50);
        b.Property(p => p.Name).IsRequired().HasMaxLength(150);
        b.Property(p => p.Priority).IsRequired().HasMaxLength(20);
        b.Property(p => p.IsActive).IsRequired();
        b.Property(p => p.EffectiveFrom).IsRequired();
        b.Property(p => p.EffectiveTo);
        b.Property(p => p.LeavePolicyId).IsRequired();
        b.Property(p => p.LeaveTypeId).IsRequired();
        b.HasOne(p => p.LeaveType).WithMany().HasForeignKey(p => p.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.LeavePolicy).WithMany().HasForeignKey(p => p.LeavePolicyId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(p => p.LeavePolicyId);
        b.HasIndex(p => p.LeaveTypeId);
        b.HasIndex(p => new { p.LeaveTypeId, p.LeavePolicyId, p.Code }).IsUnique();
    }
}

public class PolicyRuleCondConfi : BaseEntityConfig<PolicyRuleCondition>
{
    public override void Configure(EntityTypeBuilder<PolicyRuleCondition> b)
    {
        base.Configure(b);
        b.Property(p => p.Field).IsRequired().HasMaxLength(50);
        b.Property(p => p.Operator).IsRequired().HasMaxLength(20);
        b.Property(p => p.Value).IsRequired().HasMaxLength(100);
        b.Property(p => p.PolicyAssignmentRuleId).IsRequired();
        b.HasOne(p => p.PolicyAssignmentRule).WithMany().HasForeignKey(p => p.PolicyAssignmentRuleId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(p => p.PolicyAssignmentRuleId);
    }
}