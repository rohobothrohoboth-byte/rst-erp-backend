using Microsoft.EntityFrameworkCore;
using Svc.HRM.Attendance.Models.Entities;
using Svc.HRM.Attendance.Models.Entities.Local;

namespace Svc.HRM.Attendance.Persistence;

public class AttendanceDbContext : DbContext
{
    public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options)
    {
    }

    public DbSet<LocalAttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<LocalShift> Shifts { get; set; }
    public DbSet<LocalShiftAssignment> ShiftAssignments { get; set; }
    public DbSet<LocalOvertimeRequest> OvertimeRequests { get; set; }
    public DbSet<LocalLeaveRequest> LeaveRequests { get; set; }
    public DbSet<LocalLeaveBalance> LeaveBalances { get; set; }
    public DbSet<LocalHoliday> Holidays { get; set; }

    public DbSet<LocalCompany> LocalCompanies { get; set; }
    public DbSet<LocalBranch> LocalBranches { get; set; }
    public DbSet<LocalDepartment> LocalDepartments { get; set; }
    public DbSet<LocalEmployee> LocalEmployees { get; set; }
    public DbSet<LocalPosition> LocalPositions { get; set; }
    public DbSet<LocalJobGrade> LocalJobGrades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================================
        // ATTENDANCE RECORD
        // ============================================================
        modelBuilder.Entity<LocalAttendanceRecord>(entity =>
        {
            entity.ToTable("AttendanceRecords");
            entity.HasKey(e => e.Id);

            // ✅ Indexes
            entity.HasIndex(e => new { e.EmployeeId, e.Date })
                .IsUnique()
                .HasDatabaseName("IX_AttendanceRecords_EmployeeId_Date");

            entity.HasIndex(e => e.Date)
                .HasDatabaseName("IX_AttendanceRecords_Date");

            entity.HasIndex(e => e.EmployeeId)
                .HasDatabaseName("IX_AttendanceRecords_EmployeeId");

            entity.HasIndex(e => e.LocalEmployeeId)
                .HasDatabaseName("IX_AttendanceRecords_LocalEmployeeId");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_AttendanceRecords_Status");

            // ✅ FOREIGN KEY: LocalEmployee (CRITICAL FIX!)
            entity.HasOne(e => e.LocalEmployee)
                .WithMany()  // or .WithMany(e => e.AttendanceRecords) if you add the navigation
                .HasForeignKey(e => e.LocalEmployeeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);  // ✅ Required - cannot be null

            // ✅ Foreign key: Shift
            entity.HasOne(e => e.Shift)
                .WithMany()
                .HasForeignKey(e => e.ShiftId)
                .OnDelete(DeleteBehavior.SetNull);

            // ✅ Query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // SHIFT
        // ============================================================
        modelBuilder.Entity<LocalShift>(entity =>
        {
            entity.ToTable("Shifts");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("IX_Shifts_Name");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Shifts_IsActive");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // SHIFT ASSIGNMENT
        // ============================================================
        modelBuilder.Entity<LocalShiftAssignment>(entity =>
        {
            entity.ToTable("ShiftAssignments");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.EmployeeId, e.IsActive })
                .HasDatabaseName("IX_ShiftAssignments_EmployeeId_IsActive");

            entity.HasIndex(e => e.ShiftId)
                .HasDatabaseName("IX_ShiftAssignments_ShiftId");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_ShiftAssignments_IsActive");

            entity.HasOne(e => e.Shift)
                .WithMany(e => e.ShiftAssignments)
                .HasForeignKey(e => e.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // OVERTIME REQUEST
        // ============================================================
        modelBuilder.Entity<LocalOvertimeRequest>(entity =>
        {
            entity.ToTable("OvertimeRequests");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.EmployeeId, e.Date })
                .HasDatabaseName("IX_OvertimeRequests_EmployeeId_Date");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_OvertimeRequests_Status");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LEAVE REQUEST
        // ============================================================
        modelBuilder.Entity<LocalLeaveRequest>(entity =>
        {
            entity.ToTable("LeaveRequests");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.EmployeeId, e.Status })
                .HasDatabaseName("IX_LeaveRequests_EmployeeId_Status");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_LeaveRequests_Status");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LEAVE BALANCE
        // ============================================================
        modelBuilder.Entity<LocalLeaveBalance>(entity =>
        {
            entity.ToTable("LeaveBalances");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.EmployeeId, e.LeaveType, e.Year })
                .IsUnique()
                .HasDatabaseName("IX_LeaveBalances_EmployeeId_LeaveType_Year");

            entity.HasIndex(e => e.EmployeeId)
                .HasDatabaseName("IX_LeaveBalances_EmployeeId");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // HOLIDAY
        // ============================================================
        modelBuilder.Entity<LocalHoliday>(entity =>
        {
            entity.ToTable("Holidays");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Date)
                .HasDatabaseName("IX_Holidays_Date");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Holidays_IsActive");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LOCAL COMPANY
        // ============================================================
        modelBuilder.Entity<LocalCompany>(entity =>
        {
            entity.ToTable("LocalCompanies");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_LocalCompanies_IsDeleted");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LOCAL BRANCH
        // ============================================================
        modelBuilder.Entity<LocalBranch>(entity =>
        {
            entity.ToTable("LocalBranches");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_LocalBranches_IsDeleted");



            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LOCAL DEPARTMENT
        // ============================================================
        modelBuilder.Entity<LocalDepartment>(entity =>
        {
            entity.ToTable("LocalDepartments");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_LocalDepartments_IsDeleted");



            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LOCAL EMPLOYEE
        // ============================================================
        modelBuilder.Entity<LocalEmployee>(entity =>
        {
            entity.ToTable("LocalEmployees");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_LocalEmployees_Code");

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_LocalEmployees_IsDeleted");

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.JobGrade)
                .WithMany()
                .HasForeignKey(e => e.JobGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LOCAL POSITION
        // ============================================================
        modelBuilder.Entity<LocalPosition>(entity =>
        {
            entity.ToTable("LocalPositions");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_LocalPositions_IsDeleted");

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.JobGrade)
                .WithMany()
                .HasForeignKey(e => e.JobGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // LOCAL JOB GRADE
        // ============================================================
        modelBuilder.Entity<LocalJobGrade>(entity =>
        {
            entity.ToTable("LocalJobGrades");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_LocalJobGrades_IsDeleted");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ============================================================
        // UTC DateTime Converter
        // ============================================================
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    ));
                }
            }
        }
    }
}