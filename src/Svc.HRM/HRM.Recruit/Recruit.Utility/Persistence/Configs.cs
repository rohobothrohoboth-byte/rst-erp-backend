namespace Recruit.Utility.Persistence;

//public class AccrualHistoryConf : IEntityTypeConfiguration<AccrualHistory>
//{
//    public void Configure(EntityTypeBuilder<AccrualHistory> b)
//    {
//        //b.HasKey(x => x.Id);
//        //b.HasIndex(x => x.Id).IsUnique();
//        //b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.LeavePolicyId, x.LeaveLedgerId });
//        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
//        b.HasKey(e => e.Id);
//        b.HasOne(e => e.LeaveType).WithMany().HasForeignKey(e => e.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
//        b.HasOne(e => e.LeavePolicy).WithMany().HasForeignKey(e => e.LeavePolicyId).OnDelete(DeleteBehavior.Restrict);
//        b.HasOne(e => e.LeaveLedger).WithMany().HasForeignKey(e => e.LeaveLedgerId).OnDelete(DeleteBehavior.Restrict);
//        b.Property(e => e.AccruedAmount).HasPrecision(18, 2);
//        b.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId, e.PeriodStart, e.PeriodEnd });
//        b.HasIndex(e => e.Frequency);
//    }
//}

