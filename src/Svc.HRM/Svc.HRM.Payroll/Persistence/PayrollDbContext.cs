using Microsoft.EntityFrameworkCore;
using Svc.HRM.Payroll.Models.Entities;

namespace Svc.HRM.Payroll.Persistence;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options)
    {
    }

    public DbSet<LocalSalaryStructure> SalaryStructures { get; set; }
    public DbSet<LocalEmployeeSalary> EmployeeSalaries { get; set; }
    public DbSet<LocalPayrollRun> PayrollRuns { get; set; }
    public DbSet<LocalPayrollEmployee> PayrollEmployees { get; set; }
    public DbSet<LocalTaxRate> TaxRates { get; set; }
    public DbSet<LocalPayslip> Payslips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SalaryStructure
        modelBuilder.Entity<LocalSalaryStructure>()
            .HasIndex(x => x.Name)
            .IsUnique();

        // EmployeeSalary
        modelBuilder.Entity<LocalEmployeeSalary>()
            .HasIndex(x => new { x.EmployeeId, x.IsActive });

        // PayrollRun - Fixed: Use Entity<LocalPayrollRun>() directly
        modelBuilder.Entity<LocalPayrollRun>()
            .HasIndex(x => x.PayPeriodStart);
        modelBuilder.Entity<LocalPayrollRun>()
            .HasIndex(x => x.PayPeriodEnd);
        modelBuilder.Entity<LocalPayrollRun>()
            .HasIndex(x => x.PayrollStatus);

        // PayrollEmployee - Fixed: Use Entity<LocalPayrollEmployee>() directly
        modelBuilder.Entity<LocalPayrollEmployee>()
            .HasIndex(x => new { x.PayrollRunId, x.EmployeeId })
            .IsUnique();

        // TaxRate
        modelBuilder.Entity<LocalTaxRate>()
            .HasIndex(x => new { x.TaxYear, x.IsActive });

        // Payslip
        modelBuilder.Entity<LocalPayslip>()
            .HasIndex(x => x.PayslipNumber)
            .IsUnique();

        // Relationships
        modelBuilder.Entity<LocalEmployeeSalary>()
            .HasOne(x => x.SalaryStructure)
            .WithMany()
            .HasForeignKey(x => x.SalaryStructureId);

        modelBuilder.Entity<LocalPayrollEmployee>()
            .HasOne(x => x.PayrollRun)
            .WithMany(x => x.PayrollEmployees)
            .HasForeignKey(x => x.PayrollRunId);

        modelBuilder.Entity<LocalPayslip>()
            .HasOne(x => x.PayrollEmployee)
            .WithMany()
            .HasForeignKey(x => x.PayrollEmployeeId);
    }
}