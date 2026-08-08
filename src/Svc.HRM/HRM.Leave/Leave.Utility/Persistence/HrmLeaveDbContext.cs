using Leave.Domain.Entities;
using Leave.Domain.Entities.Local;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Leave.Utility.Persistence;

public class HrmLeaveDbContext : DbContext
{
    public HrmLeaveDbContext(DbContextOptions<HrmLeaveDbContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var indexes = entityType.GetIndexes().Where(i => i.IsUnique);
            foreach (var index in indexes)
            {
                index.SetFilter("\"IsDeleted\" = false");
            }
        }

        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrmLeaveDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.DateAdd = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.DateMod = now;
                    break;
            }
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DBConcurrencyException("The record was modified by another transaction.", ex);
        }
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
    public DbSet<EmpLeavePolicyHistory> EmpLeavePolicyHistories { get; set; }
     public DbSet<LocalCompany> LocalCompanies { get; set; }
        public DbSet<LocalBranch> LocalBranches { get; set; }
        public DbSet<LocalDepartment> LocalDepartments { get; set; }
        public DbSet<LocalEmployee> LocalEmployees { get; set; }
        public DbSet<LocalPosition> LocalPositions { get; set; }
        public DbSet<LocalJobGrade> LocalJobGrades { get; set; }
        public DbSet<LocalJgStep> LocalJgStep { get; set; }  // Singular
        public DbSet<LocalPositionReq> LocalPositionReq { get; set; }  // Singular

}