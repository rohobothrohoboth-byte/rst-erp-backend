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

        modelBuilder.Entity<AccrualHistory>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<Attachment>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<AttachmentBlob>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EmpLeavePolicy>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<EncashmentAppAction>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeaveAppAction>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeaveAppChain>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeaveBalance>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeaveEncashment>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeaveLedger>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeavePolicy>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeavePolicyConfig>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeaveRequest>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        modelBuilder.Entity<LeaveType>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
        //modelBuilder.Entity<Address>(entity => { entity.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken(); });
    }

    public DbSet<AccrualHistory> AccrualHistory { get; set; }
    public DbSet<Attachment> Attachment { get; set; }
    public DbSet<AttachmentBlob> AttachmentBlob { get; set; }
    public DbSet<EmpLeavePolicy> EmpLeavePolicy { get; set; }
    public DbSet<EncashmentAppAction> EncashmentAppAction { get; set; }
    public DbSet<LeaveAppAction> LeaveAppAction { get; set; }
    public DbSet<LeaveAppChain> LeaveAppChain { get; set; }
    public DbSet<LeaveBalance> LeaveBalance { get; set; }
    public DbSet<LeaveEncashment> LeaveEncashment { get; set; }
    public DbSet<LeaveLedger> LeaveLedger { get; set; }
    public DbSet<LeavePolicy> LeavePolicy { get; set; }
    public DbSet<LeavePolicyConfig> LeavePolicyConfig { get; set; }
    public DbSet<LeaveRequest> LeaveRequest { get; set; }
    public DbSet<LeaveType> LeaveType { get; set; }
}