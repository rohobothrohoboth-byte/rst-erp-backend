using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leave.Utility.Persistence;

public class HrmLeaveDbContext : DbContext
{
    public HrmLeaveDbContext(DbContextOptions<HrmLeaveDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrmLeaveDbContext).Assembly);
    }

    public DbSet<AccrualHistory> AccrualHistory { get; set; }
    public DbSet<Attachment> Attachment { get; set; }
    public DbSet<AttachmentBlob> AttachmentBlob { get; set; }
    public DbSet<EmpLeavePolicy> EmpLeavePolicy { get; set; }
    public DbSet<EncashmentAppAction> EncashmentAppAction { get; set; }
    public DbSet<LeaveAppAction> LeaveAppAction { get; set; }
    public DbSet<LeaveAppChain> LeaveAppChain { get; set; }
    public DbSet<LeaveAppStep> LeaveAppStep { get; set; }
    public DbSet<LeaveBalance> LeaveBalance { get; set; }
    public DbSet<LeaveEncashment> LeaveEncashment { get; set; }
    public DbSet<LeaveLedger> LeaveLedger { get; set; }
    public DbSet<LeavePolicy> LeavePolicy { get; set; }
    public DbSet<LeavePolicyConfig> LeavePolicyConfig { get; set; }
    public DbSet<LeaveRequest> LeaveRequest { get; set; }
    public DbSet<LeaveType> LeaveType { get; set; }
    public DbSet<PolicyAssignmentRule> PolicyAssignmentRule { get; set; }
    public DbSet<PolicyRuleCondition> PolicyRuleCondition { get; set; }
}