using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leave.Utility.Persistence;

public class AccrualHistoryConf : IEntityTypeConfiguration<AccrualHistory>
{
    public void Configure(EntityTypeBuilder<AccrualHistory> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyId, x.LeaveLedgerId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
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
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyConfigId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class EncashmentAppActionConf : IEntityTypeConfiguration<EncashmentAppAction>
{
    public void Configure(EntityTypeBuilder<EncashmentAppAction> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.LeaveEncashmentId, x.ApprovedById });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeaveAppActionConf : IEntityTypeConfiguration<LeaveAppAction>
{
    public void Configure(EntityTypeBuilder<LeaveAppAction> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.LeaveRequestId, x.ApprovedById });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeaveAppChainConf : IEntityTypeConfiguration<LeaveAppChain>
{
    public void Configure(EntityTypeBuilder<LeaveAppChain> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.LeavePolicyId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeaveAppStepConf : IEntityTypeConfiguration<LeaveAppStep>
{
    public void Configure(EntityTypeBuilder<LeaveAppStep> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.LeaveAppChainId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeaveBalanceConf : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeaveLedgerId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeaveEncashmentConf : IEntityTypeConfiguration<LeaveEncashment>
{
    public void Configure(EntityTypeBuilder<LeaveEncashment> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeaveLedgerConf: IEntityTypeConfiguration<LeaveLedger>
{
    public void Configure(EntityTypeBuilder<LeaveLedger> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyId, x.ReferenceId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeavePolicyConf : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(EntityTypeBuilder<LeavePolicy> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.LeaveTypeId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeavePolicyConfigConf : IEntityTypeConfiguration<LeavePolicyConfig>
{
    public void Configure(EntityTypeBuilder<LeavePolicyConfig> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.FiscalYearId, x.LeavePolicyId, x.LeaveAppChainId });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class LeaveRequestConf : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.Id).IsUnique();
        b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.ApprovedById, x.Status });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
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
        b.HasIndex(x => new { x.LeavePolicyId, x.LeaveTypeId});
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