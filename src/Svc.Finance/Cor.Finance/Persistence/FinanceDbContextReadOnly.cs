// Persistence/FinanceDbContextReadOnly.cs
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Entities.Local;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Cor.Finance.Models.Entities.Aggregates;
namespace Cor.Finance.Persistence;

public class FinanceDbContextReadOnly : DbContext
{
    public FinanceDbContextReadOnly(DbContextOptions<FinanceDbContextReadOnly> options)
        : base(options) { }

    // ✅ Read-only DbSets (same as main context)
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<ChartOfAccounts> ChartOfAccounts { get; set; }
    public DbSet<FinancialPeriod> FinancialPeriods { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<LocalEmployee> LocalEmployees { get; set; }
    public DbSet<LocalBranch> LocalBranches { get; set; }
    public DbSet<LocalDepartment> LocalDepartments { get; set; }
    public DbSet<InvoiceAggregate> InvoiceAggregates { get; set; }
    public DbSet<PaymentAggregate> PaymentAggregates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
    }
}