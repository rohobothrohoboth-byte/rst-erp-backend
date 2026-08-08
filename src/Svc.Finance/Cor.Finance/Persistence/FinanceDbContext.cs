using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Entities.Local;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Cor.Finance.Models.Entities.Aggregates;

namespace Cor.Finance.Persistence;

public class FinanceDbContext : DbContext
{
    // ✅ ONLY this constructor - nothing else!
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    {
    }

    // ============================================================
    // CORE FINANCE ENTITIES
    // ============================================================
    public DbSet<ChartOfAccounts> ChartOfAccounts { get; set; }
    public DbSet<FinancialPeriod> FinancialPeriods { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalLine> JournalLines { get; set; }
    public DbSet<AccountCategory> AccountCategories { get; set; }

    // ============================================================
    // INVOICES (AP & AR)
    // ============================================================
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<InvoiceAmendment> InvoiceAmendments { get; set; }

    // ============================================================
    // PAYMENTS & RECEIPTS
    // ============================================================
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentApprovalChain> PaymentApprovalChains { get; set; }
    public DbSet<ApprovalStep> ApprovalSteps { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ExternalSystem> ExternalSystems { get; set; }

    // ============================================================
    // VOUCHERS
    // ============================================================
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<VoucherLine> VoucherLines { get; set; }

    // ============================================================
    // BUDGET MANAGEMENT
    // ============================================================
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetLine> BudgetLines { get; set; }
    public DbSet<BudgetCategory> BudgetCategories { get; set; }
    public DbSet<BudgetCode> BudgetCodes { get; set; }
    public DbSet<BudgetControl> BudgetControls { get; set; }

    // ============================================================
    // BANK & CASH
    // ============================================================
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<BankTransaction> BankTransactions { get; set; }

    // ============================================================
    // EXPENSES
    // ============================================================
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<Asset> Assets { get; set; }

    // ============================================================
    // PURCHASE & SALES ORDERS
    // ============================================================
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SalesOrderLine> SalesOrderLines { get; set; }

    // ============================================================
    // TAX MANAGEMENT
    // ============================================================
    public DbSet<TaxRate> TaxRates { get; set; }
    public DbSet<TaxReturn> TaxReturns { get; set; }

    // ============================================================
    // COST & PROFIT CENTERS
    // ============================================================
    public DbSet<CostCenter> CostCenters { get; set; }
    public DbSet<ProfitCenter> ProfitCenters { get; set; }

    // ============================================================
    // VENDOR & CUSTOMER
    // ============================================================
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Customer> Customers { get; set; }

    // ============================================================
    // VENDOR PORTAL
    // ============================================================
    public DbSet<VendorPortalUser> VendorPortalUsers { get; set; }
    public DbSet<PortalInvoice> PortalInvoices { get; set; }
    public DbSet<PortalPayment> PortalPayments { get; set; }
    public DbSet<PortalNotification> PortalNotifications { get; set; }

    // ============================================================
    // CREDIT & DEBIT NOTES
    // ============================================================
    public DbSet<CreditNote> CreditNotes { get; set; }
    public DbSet<CreditNoteLine> CreditNoteLines { get; set; }
    public DbSet<DebitNote> DebitNotes { get; set; }
    public DbSet<DebitNoteLine> DebitNoteLines { get; set; }

    // ============================================================
    // PRODUCTS & INVENTORY
    // ============================================================
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }
    public DbSet<ProductInventory> ProductInventories { get; set; }

    // ============================================================
    // CURRENCY
    // ============================================================
    public DbSet<CurrencyRate> CurrencyRates { get; set; }

    // ============================================================
    // PAYMENT SCHEDULE
    // ============================================================
    public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
    public DbSet<PaymentScheduleHistory> PaymentScheduleHistories { get; set; }

    // ============================================================
    // INTERNAL ORDERS
    // ============================================================
    public DbSet<InternalOrder> InternalOrders { get; set; }
    public DbSet<InvoiceAggregate> InvoiceAggregates { get; set; }
    public DbSet<PaymentAggregate> PaymentAggregates { get; set; }

    // ============================================================
    // COMPLIANCE & INTERNAL CONTROLS
    // ============================================================
    public DbSet<ComplianceRequirement> ComplianceRequirements { get; set; }
    public DbSet<ComplianceRequirementControl> ComplianceRequirementControls { get; set; }
    public DbSet<ComplianceRequirementEvidence> ComplianceRequirementEvidences { get; set; }
    public DbSet<ComplianceReport> ComplianceReports { get; set; }
    public DbSet<InternalControl> InternalControls { get; set; }

    // ============================================================
    // IFRS REPORTS
    // ============================================================
    public DbSet<IFRSReport> IFRSReports { get; set; }
    public DbSet<IFRSMetric> IFRSMetrics { get; set; }

    // ============================================================
    // CONSOLIDATION
    // ============================================================
    public DbSet<Entity> Entities { get; set; }
    public DbSet<ConsolidationGroup> ConsolidationGroups { get; set; }
    public DbSet<ConsolidationGroupEntity> ConsolidationGroupEntities { get; set; }
    public DbSet<ConsolidationReport> ConsolidationReports { get; set; }
    public DbSet<EliminationEntry> EliminationEntries { get; set; }

    // ============================================================
    // AUDIT
    // ============================================================
    public DbSet<AuditLog> AuditLogs { get; set; }

    // ============================================================
    // LOCAL COPIES (From other modules)
    // ============================================================
    public DbSet<LocalCompany> LocalCompanies { get; set; }
    public DbSet<LocalBranch> LocalBranches { get; set; }
    public DbSet<LocalDepartment> LocalDepartments { get; set; }
    public DbSet<LocalEmployee> LocalEmployees { get; set; }
    public DbSet<LocalPosition> LocalPositions { get; set; }
    public DbSet<LocalJobGrade> LocalJobGrades { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Convert all DateTime properties to UTC for PostgreSQL compatibility
        UseUtcDateTimeConverter(modelBuilder);

        // ============================================================
        // CHART OF ACCOUNTS
        // ============================================================
        modelBuilder.Entity<ChartOfAccounts>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.AccountType);
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.IsDeleted);

            // ✅ ADD: Composite index for dashboard queries
            entity.HasIndex(x => new { x.AccountType, x.IsDeleted })
                .HasDatabaseName("IX_ChartOfAccounts_AccountType_IsDeleted");

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // INVOICE AGGREGATE
        // ============================================================
        modelBuilder.Entity<InvoiceAggregate>(entity =>
        {
            entity.HasIndex(x => x.Period);
            entity.HasIndex(x => x.AggregateType);
            entity.HasIndex(x => x.AggregateDate);

            // ✅ ADD: Composite indexes for aggregate queries
            entity.HasIndex(x => new { x.AggregateDate, x.AggregateType })
                .HasDatabaseName("IX_InvoiceAggregates_AggregateDate_AggregateType");

            entity.HasIndex(x => new { x.Period, x.AggregateType })
                .HasDatabaseName("IX_InvoiceAggregates_Period_AggregateType");
        });

        // ============================================================
        // PAYMENT AGGREGATE
        // ============================================================
        modelBuilder.Entity<PaymentAggregate>(entity =>
        {
            entity.HasIndex(x => x.Period);
            entity.HasIndex(x => x.AggregateType);
            entity.HasIndex(x => x.AggregateDate);

            // ✅ ADD: Composite indexes for aggregate queries
            entity.HasIndex(x => new { x.AggregateDate, x.AggregateType })
                .HasDatabaseName("IX_PaymentAggregates_AggregateDate_AggregateType");

            entity.HasIndex(x => new { x.Period, x.AggregateType })
                .HasDatabaseName("IX_PaymentAggregates_Period_AggregateType");
        });

        // ============================================================
        // FINANCIAL PERIOD
        // ============================================================
        modelBuilder.Entity<FinancialPeriod>(entity =>
        {
            entity.HasIndex(x => x.StartDate);
            entity.HasIndex(x => x.EndDate);
            entity.HasIndex(x => x.IsClosed);
            entity.HasIndex(x => x.Status);

            // ✅ ADD: Composite index for dashboard queries
            entity.HasIndex(x => new { x.StartDate, x.EndDate })
                .HasDatabaseName("IX_FinancialPeriods_StartDate_EndDate");
        });

        // ============================================================
        // JOURNAL ENTRIES
        // ============================================================
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasIndex(x => x.Reference);
            entity.HasIndex(x => x.EntryDate);
            entity.HasIndex(x => x.IsPosted);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.EntryDate, x.IsDeleted })
                .HasDatabaseName("IX_JournalEntries_EntryDate_IsDeleted");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_JournalEntries_PeriodId_IsDeleted");

            entity.HasIndex(x => new { x.IsPosted, x.EntryDate })
                .HasDatabaseName("IX_JournalEntries_IsPosted_EntryDate");

            // ✅ ADD: Covering index for journal entry queries
            entity.HasIndex(x => new { x.EntryDate, x.IsDeleted })
                .HasDatabaseName("IX_JournalEntries_EntryDate_IsDeleted_Include")
                .IncludeProperties(x => new { x.IsPosted, x.TotalDebit, x.TotalCredit });

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.JournalEntry)
                .HasForeignKey(x => x.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // JOURNAL LINES
        // ============================================================
        modelBuilder.Entity<JournalLine>(entity =>
        {
            entity.HasIndex(x => x.JournalEntryId);
            entity.HasIndex(x => x.AccountId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite indexes for performance
            entity.HasIndex(x => new { x.AccountId, x.PeriodId })
                .HasDatabaseName("IX_JournalLines_AccountId_PeriodId");

            entity.HasIndex(x => new { x.JournalEntryId, x.AccountId })
                .HasDatabaseName("IX_JournalLines_JournalEntryId_AccountId");

            entity.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // ACCOUNT CATEGORY
        // ============================================================
        modelBuilder.Entity<AccountCategory>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Type);

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // INVOICES (AP & AR)
        // ============================================================
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(x => x.InvoiceNumber).IsUnique();
            entity.HasIndex(x => x.InvoiceType);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.VendorId);
            entity.HasIndex(x => x.CustomerId);
            entity.HasIndex(x => x.InvoiceDate);
            entity.HasIndex(x => x.DueDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.InvoiceDate, x.IsDeleted })
                .HasDatabaseName("IX_Invoices_InvoiceDate_IsDeleted");

            entity.HasIndex(x => new { x.VendorId, x.IsDeleted })
                .HasDatabaseName("IX_Invoices_VendorId_IsDeleted");

            entity.HasIndex(x => new { x.CustomerId, x.IsDeleted })
                .HasDatabaseName("IX_Invoices_CustomerId_IsDeleted");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_Invoices_PeriodId_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.InvoiceDate })
                .HasDatabaseName("IX_Invoices_Status_InvoiceDate");

            // ✅ ADD: Sales-specific indexes (for dashboard queries)
            entity.HasIndex(x => new { x.InvoiceDate, x.InvoiceType, x.Status })
                .HasDatabaseName("IX_Invoices_Sales_Date_Type_Status")
                .HasFilter("\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

            entity.HasIndex(x => new { x.InvoiceDate, x.InvoiceType })
                .HasDatabaseName("IX_Invoices_Sales_Date_Type")
                .HasFilter("\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

            // ✅ ADD: Purchase-specific indexes (for dashboard queries)
            entity.HasIndex(x => new { x.VendorId, x.InvoiceDate })
                .HasDatabaseName("IX_Invoices_Purchase_Vendor_Date")
                .HasFilter("\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

            entity.HasIndex(x => new { x.InvoiceDate, x.InvoiceType })
                .HasDatabaseName("IX_Invoices_Purchase_Date_Type")
                .HasFilter("\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

            // ✅ ADD: Covering index for invoice queries
            entity.HasIndex(x => new { x.InvoiceDate, x.IsDeleted })
                .HasDatabaseName("IX_Invoices_InvoiceDate_IsDeleted_Include")
                .IncludeProperties(x => new { x.InvoiceNumber, x.TotalAmount, x.Status, x.PaidAmount, x.DueDate });



                 // ✅ NEW: Sales Data Query Index
                        entity.HasIndex(x => new { x.InvoiceDate, x.InvoiceType, x.Status })
                            .HasDatabaseName("IX_Invoices_Sales_Date_Type_Status")
                            .HasFilter("\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

                        // ✅ NEW: Purchase Data Query Index
                        entity.HasIndex(x => new { x.InvoiceDate, x.InvoiceType })
                            .HasDatabaseName("IX_Invoices_Purchase_Date_Type")
                            .HasFilter("\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

                        // ✅ NEW: Top Customers Index
                        entity.HasIndex(x => new { x.CustomerId, x.InvoiceDate, x.TotalAmount })
                            .HasDatabaseName("IX_Invoices_Customer_Date_Amount")
                            .HasFilter("\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

                        // ✅ NEW: Accounts Receivable Index
                        entity.HasIndex(x => new { x.Status, x.InvoiceDate })
                            .HasDatabaseName("IX_Invoices_AR_Status_Date")
                            .HasFilter("\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

                        // ✅ NEW: Accounts Payable Index
                        entity.HasIndex(x => new { x.Status, x.InvoiceDate })
                            .HasDatabaseName("IX_Invoices_AP_Status_Date")
                            .HasFilter("\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

                        // ✅ NEW: Top Vendors Index (if needed)
                        entity.HasIndex(x => new { x.VendorId, x.InvoiceDate, x.TotalAmount })
                            .HasDatabaseName("IX_Invoices_Vendor_Date_Amount")
                            .HasFilter("\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");


                             // ============================================================
                                // 🆕 ADD YOUR NEW INDEXES HERE
                                // ============================================================

                                // ✅ NEW: For Revenue Trend Query (Yearly Sales)
                                entity.HasIndex(x => new { x.InvoiceDate, x.InvoiceType, x.TotalAmount })
                                    .HasDatabaseName("IX_Invoices_Trend_Date_Type_Amount")
                                    .HasFilter("\"InvoiceType\" IN ('Sales', 'Purchase') AND \"IsDeleted\" = false");

                                // ✅ NEW: For Aging Report (Due Date queries)
                                entity.HasIndex(x => new { x.DueDate, x.Status, x.TotalAmount, x.PaidAmount })
                                    .HasDatabaseName("IX_Invoices_Aging_DueDate_Status")
                                    .HasFilter("\"Status\" != 'Paid' AND \"IsDeleted\" = false");

                                // ✅ NEW: For Customer queries
                                entity.HasIndex(x => new { x.CustomerId, x.InvoiceDate, x.TotalAmount, x.PaidAmount })
                                    .HasDatabaseName("IX_Invoices_Customer_All_Fields")
                                    .HasFilter("\"InvoiceType\" = 'Sales' AND \"IsDeleted\" = false");

                                // ✅ NEW: For Vendor queries
                                entity.HasIndex(x => new { x.VendorId, x.InvoiceDate, x.TotalAmount, x.PaidAmount })
                                    .HasDatabaseName("IX_Invoices_Vendor_All_Fields")
                                    .HasFilter("\"InvoiceType\" = 'Purchase' AND \"IsDeleted\" = false");

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.Invoice)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Payments)
                .WithOne(x => x.Invoice)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(tb => tb.HasCheckConstraint(
                "CK_Invoice_Party",
                "(\"VendorId\" IS NOT NULL AND \"CustomerId\" IS NULL) OR (\"VendorId\" IS NULL AND \"CustomerId\" IS NOT NULL)"));

            entity.ToTable(tb => tb.HasCheckConstraint(
                "CK_Invoice_Type",
                "\"InvoiceType\" IN ('Purchase', 'Sales')"));

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // INVOICE LINES
        // ============================================================
        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasIndex(x => x.InvoiceId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for dashboard queries
            entity.HasIndex(x => new { x.InvoiceId, x.PeriodId })
                .HasDatabaseName("IX_InvoiceLines_InvoiceId_PeriodId");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // ASSETS
        // ============================================================
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.SerialNumber);
            entity.HasIndex(x => x.AssetType);
            entity.HasIndex(x => x.AssetCategory);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.AcquisitionDate);
            entity.HasIndex(x => x.BranchId);
            entity.HasIndex(x => x.DepartmentId);
            entity.HasIndex(x => x.AssignedTo);
            entity.HasIndex(x => x.AccountId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.DateAdd, x.IsDeleted })
                .HasDatabaseName("IX_Assets_DateAdd_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.IsDeleted })
                .HasDatabaseName("IX_Assets_Status_IsDeleted");

            entity.HasIndex(x => new { x.AssetType, x.IsDeleted })
                .HasDatabaseName("IX_Assets_AssetType_IsDeleted");

            // ✅ ADD: Covering index for asset queries
            entity.HasIndex(x => new { x.DateAdd, x.IsDeleted })
                .HasDatabaseName("IX_Assets_DateAdd_IsDeleted_Include")
                .IncludeProperties(x => new { x.CurrentValue, x.AccumulatedDepreciation, x.Status });
                  entity.HasIndex(x => new { x.DateAdd, x.Status, x.IsDeleted })
                            .HasDatabaseName("IX_Assets_Date_Status_Include")
                            .HasFilter("\"IsDeleted\" = false");

                        // ✅ NEW: Asset Date Index with Include
                        entity.HasIndex(x => new { x.DateAdd, x.IsDeleted })
                            .HasDatabaseName("IX_Assets_Date_IsDeleted_Include")
                            .IncludeProperties(x => new { x.CurrentValue, x.AccumulatedDepreciation, x.Status });

            entity.HasOne(x => x.AssignedEmployee)
                .WithMany()
                .HasForeignKey(x => x.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Branch)
                .WithMany()
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // INVOICE AMENDMENT
        // ============================================================
        modelBuilder.Entity<InvoiceAmendment>(entity =>
        {
            entity.HasIndex(x => x.InvoiceId);
            entity.HasIndex(x => x.Status);

            entity.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PAYMENTS
        // ============================================================
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(x => x.PaymentNumber).IsUnique();
            entity.HasIndex(x => x.PaymentType);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.VendorId);
            entity.HasIndex(x => x.CustomerId);
            entity.HasIndex(x => x.PaymentDate);
            entity.HasIndex(x => x.InvoiceId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.PaymentDate, x.IsDeleted })
                .HasDatabaseName("IX_Payments_PaymentDate_IsDeleted");

            entity.HasIndex(x => new { x.VendorId, x.IsDeleted })
                .HasDatabaseName("IX_Payments_VendorId_IsDeleted");

            entity.HasIndex(x => new { x.CustomerId, x.IsDeleted })
                .HasDatabaseName("IX_Payments_CustomerId_IsDeleted");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_Payments_PeriodId_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.PaymentDate })
                .HasDatabaseName("IX_Payments_Status_PaymentDate");

            // ✅ ADD: Covering index for payment queries
            entity.HasIndex(x => new { x.PaymentDate, x.IsDeleted })
                .HasDatabaseName("IX_Payments_PaymentDate_IsDeleted_Include")
                .IncludeProperties(x => new { x.Amount, x.PaymentType, x.Status });

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Invoice)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.JournalEntry)
                .WithMany()
                .HasForeignKey(x => x.JournalEntryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.BankAccount)
                .WithMany()
                .HasForeignKey(x => x.BankAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(tb => tb.HasCheckConstraint(
                "CK_Payment_Type",
                "\"PaymentType\" IN ('Purchase', 'Sales')"));

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // RECEIPTS
        // ============================================================
        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasIndex(x => x.ReceiptNumber).IsUnique();
            entity.HasIndex(x => x.ReceiptDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance index for dashboard queries
            entity.HasIndex(x => new { x.ReceiptDate, x.IsDeleted })
                .HasDatabaseName("IX_Receipts_ReceiptDate_IsDeleted");

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.BankAccount)
                .WithMany()
                .HasForeignKey(x => x.BankAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PAYMENT APPROVAL CHAIN
        // ============================================================
        modelBuilder.Entity<PaymentApprovalChain>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.IsActive);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.IsActive, x.IsDeleted })
                .HasDatabaseName("IX_PaymentApprovalChains_IsActive_IsDeleted");

            entity.HasMany(x => x.Steps)
                .WithOne(x => x.PaymentApprovalChain)
                .HasForeignKey(x => x.PaymentApprovalChainId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // APPROVAL STEP
        // ============================================================
        modelBuilder.Entity<ApprovalStep>(entity =>
        {
            entity.HasIndex(x => x.PaymentApprovalChainId);
            entity.HasIndex(x => x.Order);

            entity.HasOne(x => x.PaymentApprovalChain)
                .WithMany(x => x.Steps)
                .HasForeignKey(x => x.PaymentApprovalChainId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // VOUCHERS
        // ============================================================
        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasIndex(x => x.VoucherNumber).IsUnique();
            entity.HasIndex(x => x.VoucherType);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.VoucherDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance index for dashboard queries
            entity.HasIndex(x => new { x.VoucherDate, x.IsDeleted })
                .HasDatabaseName("IX_Vouchers_VoucherDate_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.VoucherDate })
                .HasDatabaseName("IX_Vouchers_Status_VoucherDate");

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.Voucher)
                .HasForeignKey(x => x.VoucherId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // VOUCHER LINES
        // ============================================================
        modelBuilder.Entity<VoucherLine>(entity =>
        {
            entity.HasIndex(x => x.VoucherId);
            entity.HasIndex(x => x.AccountId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.VoucherId, x.AccountId })
                .HasDatabaseName("IX_VoucherLines_VoucherId_AccountId");

            entity.HasOne(x => x.Voucher)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.VoucherId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // BUDGET
        // ============================================================
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasIndex(x => x.Name);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.StartDate);
            entity.HasIndex(x => x.EndDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.StartDate, x.EndDate })
                .HasDatabaseName("IX_Budgets_StartDate_EndDate");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_Budgets_PeriodId_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.StartDate })
                .HasDatabaseName("IX_Budgets_Status_StartDate");

            entity.HasIndex(x => new { x.Status, x.IsDeleted })
                .HasDatabaseName("IX_Budgets_Status_IsDeleted");

            // ✅ ADD: Covering index for budget queries
            entity.HasIndex(x => new { x.Status, x.IsDeleted })
                .HasDatabaseName("IX_Budgets_Status_IsDeleted_Include")
                .IncludeProperties(x => new { x.TotalAmount, x.StartDate, x.EndDate });
                  // ✅ NEW: Budgets Index
                        entity.HasIndex(x => new { x.Status, x.StartDate, x.EndDate, x.IsDeleted })
                            .HasDatabaseName("IX_Budgets_Status_Date_Include")
                            .HasFilter("\"IsDeleted\" = false");

                        // ✅ NEW: Budget Status Index with Include
                        entity.HasIndex(x => new { x.Status, x.IsDeleted })
                            .HasDatabaseName("IX_Budgets_Status_IsDeleted_Include")
                            .IncludeProperties(x => new { x.TotalAmount, x.StartDate, x.EndDate });

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.Budget)
                .HasForeignKey(x => x.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // BUDGET LINES
        // ============================================================
        modelBuilder.Entity<BudgetLine>(entity =>
        {
            entity.HasIndex(x => x.BudgetId);
            entity.HasIndex(x => x.AccountId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.BudgetId, x.AccountId })
                .HasDatabaseName("IX_BudgetLines_BudgetId_AccountId");

            entity.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // BUDGET CONTROL
        // ============================================================
        modelBuilder.Entity<BudgetControl>(entity =>
        {
            entity.HasIndex(x => x.BudgetId);
            entity.HasIndex(x => x.AccountId);
            entity.HasIndex(x => x.PeriodId);
            entity.HasIndex(x => x.Status);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.BudgetId, x.AccountId, x.PeriodId })
                .HasDatabaseName("IX_BudgetControls_BudgetId_AccountId_PeriodId");

            entity.HasOne(x => x.Budget)
                .WithMany()
                .HasForeignKey(x => x.BudgetId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // BANK ACCOUNT
        // ============================================================
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasIndex(x => x.AccountNumber).IsUnique();
            entity.HasIndex(x => x.AccountType);
            entity.HasIndex(x => x.IsActive);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.DateAdd, x.IsDeleted })
                .HasDatabaseName("IX_BankAccounts_DateAdd_IsDeleted");

            entity.HasIndex(x => new { x.AccountType, x.IsDeleted })
                .HasDatabaseName("IX_BankAccounts_AccountType_IsDeleted");

            // ✅ ADD: Covering index for bank account queries
            entity.HasIndex(x => new { x.DateAdd, x.IsDeleted })
                .HasDatabaseName("IX_BankAccounts_DateAdd_IsDeleted_Include")
                .IncludeProperties(x => new { x.CurrentBalance, x.AccountType });

            entity.HasMany(x => x.Transactions)
                .WithOne(x => x.BankAccount)
                .HasForeignKey(x => x.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // BANK TRANSACTION
        // ============================================================
        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.HasIndex(x => x.BankAccountId);
            entity.HasIndex(x => x.TransactionDate);
            entity.HasIndex(x => x.TransactionType);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.TransactionDate, x.IsDeleted })
                .HasDatabaseName("IX_BankTransactions_TransactionDate_IsDeleted");

            entity.HasIndex(x => new { x.BankAccountId, x.TransactionDate })
                .HasDatabaseName("IX_BankTransactions_BankAccountId_TransactionDate");

            entity.HasIndex(x => new { x.Status, x.TransactionDate })
                .HasDatabaseName("IX_BankTransactions_Status_TransactionDate");

            entity.HasOne(x => x.BankAccount)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // EXPENSES
        // ============================================================
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasIndex(x => x.ExpenseDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.ExpenseDate, x.IsDeleted })
                .HasDatabaseName("IX_Expenses_ExpenseDate_IsDeleted");

            entity.HasIndex(x => new { x.ExpenseCategoryId, x.IsDeleted })
                .HasDatabaseName("IX_Expenses_CategoryId_IsDeleted");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_Expenses_PeriodId_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.ExpenseDate })
                .HasDatabaseName("IX_Expenses_Status_ExpenseDate");

            // ✅ ADD: Covering index for expense queries
            entity.HasIndex(x => new { x.ExpenseDate, x.IsDeleted })
                .HasDatabaseName("IX_Expenses_ExpenseDate_IsDeleted_Include")
                .IncludeProperties(x => new { x.Amount, x.ExpenseCategoryId });

                  // ✅ NEW: Expense Categories Index
                        entity.HasIndex(x => new { x.ExpenseDate, x.ExpenseCategoryId, x.IsDeleted })
                            .HasDatabaseName("IX_Expenses_Date_Category_Include")
                            .HasFilter("\"IsDeleted\" = false");

                        // ✅ NEW: Expense Date Index with Include
                        entity.HasIndex(x => new { x.ExpenseDate, x.IsDeleted })
                            .HasDatabaseName("IX_Expenses_Date_IsDeleted_Include")
                            .IncludeProperties(x => new { x.Amount, x.ExpenseCategoryId });

            entity.HasOne(x => x.ExpenseCategory)
                .WithMany()
                .HasForeignKey(x => x.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // EXPENSE CATEGORY
        // ============================================================
        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasIndex(x => x.CategoryType);
            entity.HasIndex(x => x.IsActive);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("IX_ExpenseCategories_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PURCHASE ORDERS
        // ============================================================
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasIndex(x => x.PurchaseOrderNumber).IsUnique();
            entity.HasIndex(x => x.OrderDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.VendorId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.OrderDate, x.IsDeleted })
                .HasDatabaseName("IX_PurchaseOrders_OrderDate_IsDeleted");

            entity.HasIndex(x => new { x.VendorId, x.OrderDate })
                .HasDatabaseName("IX_PurchaseOrders_VendorId_OrderDate");

            entity.HasIndex(x => new { x.Status, x.OrderDate })
                .HasDatabaseName("IX_PurchaseOrders_Status_OrderDate");

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.PurchaseOrder)
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PURCHASE ORDER LINES
        // ============================================================
        modelBuilder.Entity<PurchaseOrderLine>(entity =>
        {
            entity.HasIndex(x => x.PurchaseOrderId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.PurchaseOrderId, x.PeriodId })
                .HasDatabaseName("IX_PurchaseOrderLines_PurchaseOrderId_PeriodId");

            entity.HasOne(x => x.PurchaseOrder)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // SALES ORDERS
        // ============================================================
        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasIndex(x => x.OrderNumber).IsUnique();
            entity.HasIndex(x => x.OrderDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.CustomerId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.OrderDate, x.IsDeleted })
                .HasDatabaseName("IX_SalesOrders_OrderDate_IsDeleted");

            entity.HasIndex(x => new { x.CustomerId, x.OrderDate })
                .HasDatabaseName("IX_SalesOrders_CustomerId_OrderDate");

            entity.HasIndex(x => new { x.Status, x.OrderDate })
                .HasDatabaseName("IX_SalesOrders_Status_OrderDate");

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.SalesOrder)
                .HasForeignKey(x => x.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // SALES ORDER LINES
        // ============================================================
        modelBuilder.Entity<SalesOrderLine>(entity =>
        {
            entity.HasIndex(x => x.SalesOrderId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.SalesOrderId, x.PeriodId })
                .HasDatabaseName("IX_SalesOrderLines_SalesOrderId_PeriodId");

            entity.HasOne(x => x.SalesOrder)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // TAX RATES
        // ============================================================
        modelBuilder.Entity<TaxRate>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.IsActive);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // TAX RETURNS
        // ============================================================
        modelBuilder.Entity<TaxReturn>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.TaxType);
            entity.HasIndex(x => x.Period);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance index for dashboard queries
            entity.HasIndex(x => new { x.Period, x.Status })
                .HasDatabaseName("IX_TaxReturns_Period_Status");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // COST CENTER
        // ============================================================
        modelBuilder.Entity<CostCenter>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.IsActive);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.IsActive, x.IsDeleted })
                .HasDatabaseName("IX_CostCenters_IsActive_IsDeleted");

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PROFIT CENTER
        // ============================================================
        modelBuilder.Entity<ProfitCenter>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.IsActive);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.IsActive, x.IsDeleted })
                .HasDatabaseName("IX_ProfitCenters_IsActive_IsDeleted");

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // VENDOR
        // ============================================================
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Email);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.VendorType);
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.IsDeleted);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Status, x.IsDeleted })
                .HasDatabaseName("IX_Vendors_Status_IsDeleted");

            entity.HasMany(x => x.PortalUsers)
                .WithOne(x => x.Vendor)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CUSTOMER
        // ============================================================
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Email);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.CustomerType);
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.IsDeleted);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Status, x.IsDeleted })
                .HasDatabaseName("IX_Customers_Status_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // VENDOR PORTAL
        // ============================================================
        modelBuilder.Entity<VendorPortalUser>(entity =>
        {
            entity.HasIndex(x => x.VendorId);
            entity.HasIndex(x => x.Email);
            entity.HasIndex(x => x.Status);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.VendorId, x.Status })
                .HasDatabaseName("IX_VendorPortalUsers_VendorId_Status");

            entity.HasOne(x => x.Vendor)
                .WithMany(x => x.PortalUsers)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PORTAL INVOICE
        // ============================================================
        modelBuilder.Entity<PortalInvoice>(entity =>
        {
            entity.HasIndex(x => x.InvoiceNumber).IsUnique();
            entity.HasIndex(x => x.VendorId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.InvoiceDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.InvoiceDate, x.IsDeleted })
                .HasDatabaseName("IX_PortalInvoices_InvoiceDate_IsDeleted");

            entity.HasIndex(x => new { x.VendorId, x.InvoiceDate })
                .HasDatabaseName("IX_PortalInvoices_VendorId_InvoiceDate");

            entity.HasIndex(x => new { x.Status, x.InvoiceDate })
                .HasDatabaseName("IX_PortalInvoices_Status_InvoiceDate");

            entity.HasOne(x => x.Vendor)
                .WithMany(x => x.Invoices)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PORTAL PAYMENT
        // ============================================================
        modelBuilder.Entity<PortalPayment>(entity =>
        {
            entity.HasIndex(x => x.PaymentNumber).IsUnique();
            entity.HasIndex(x => x.InvoiceId);
            entity.HasIndex(x => x.VendorId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PaymentDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.PaymentDate, x.IsDeleted })
                .HasDatabaseName("IX_PortalPayments_PaymentDate_IsDeleted");

            entity.HasIndex(x => new { x.VendorId, x.PaymentDate })
                .HasDatabaseName("IX_PortalPayments_VendorId_PaymentDate");

            entity.HasIndex(x => new { x.Status, x.PaymentDate })
                .HasDatabaseName("IX_PortalPayments_Status_PaymentDate");

            entity.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Vendor)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PORTAL NOTIFICATION
        // ============================================================
        modelBuilder.Entity<PortalNotification>(entity =>
        {
            entity.HasIndex(x => x.VendorId);
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.IsRead);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.VendorId, x.IsRead })
                .HasDatabaseName("IX_PortalNotifications_VendorId_IsRead");

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CREDIT NOTE
        // ============================================================
        modelBuilder.Entity<CreditNote>(entity =>
        {
            entity.HasIndex(x => x.NoteNumber).IsUnique();
            entity.HasIndex(x => x.NoteDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.NoteDate, x.IsDeleted })
                .HasDatabaseName("IX_CreditNotes_NoteDate_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.NoteDate })
                .HasDatabaseName("IX_CreditNotes_Status_NoteDate");

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.CreditNote)
                .HasForeignKey(x => x.CreditNoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CREDIT NOTE LINES
        // ============================================================
        modelBuilder.Entity<CreditNoteLine>(entity =>
        {
            entity.HasIndex(x => x.CreditNoteId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.CreditNoteId, x.PeriodId })
                .HasDatabaseName("IX_CreditNoteLines_CreditNoteId_PeriodId");

            entity.HasOne(x => x.CreditNote)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.CreditNoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // DEBIT NOTE
        // ============================================================
        modelBuilder.Entity<DebitNote>(entity =>
        {
            entity.HasIndex(x => x.NoteNumber).IsUnique();
            entity.HasIndex(x => x.NoteDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.NoteDate, x.IsDeleted })
                .HasDatabaseName("IX_DebitNotes_NoteDate_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.NoteDate })
                .HasDatabaseName("IX_DebitNotes_Status_NoteDate");

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Lines)
                .WithOne(x => x.DebitNote)
                .HasForeignKey(x => x.DebitNoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // DEBIT NOTE LINES
        // ============================================================
        modelBuilder.Entity<DebitNoteLine>(entity =>
        {
            entity.HasIndex(x => x.DebitNoteId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.DebitNoteId, x.PeriodId })
                .HasDatabaseName("IX_DebitNoteLines_DebitNoteId_PeriodId");

            entity.HasOne(x => x.DebitNote)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.DebitNoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PRODUCT
        // ============================================================
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.IsActive);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.IsActive, x.IsDeleted })
                .HasDatabaseName("IX_Products_IsActive_IsDeleted");

            entity.HasMany(x => x.PriceHistory)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.InventoryTransactions)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PRODUCT PRICE
        // ============================================================
        modelBuilder.Entity<ProductPrice>(entity =>
        {
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => x.EffectiveDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.ProductId, x.EffectiveDate })
                .HasDatabaseName("IX_ProductPrices_ProductId_EffectiveDate");

            entity.HasOne(x => x.Product)
                .WithMany(x => x.PriceHistory)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PRODUCT INVENTORY
        // ============================================================
        modelBuilder.Entity<ProductInventory>(entity =>
        {
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => x.TransactionDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.ProductId, x.TransactionDate })
                .HasDatabaseName("IX_ProductInventories_ProductId_TransactionDate");

            entity.HasOne(x => x.Product)
                .WithMany(x => x.InventoryTransactions)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CURRENCY RATE
        // ============================================================
        modelBuilder.Entity<CurrencyRate>(entity =>
        {
            entity.HasIndex(x => new { x.FromCurrency, x.ToCurrency, x.RateDate });
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.RateDate, x.IsDeleted })
                .HasDatabaseName("IX_CurrencyRates_RateDate_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PAYMENT SCHEDULE
        // ============================================================
        modelBuilder.Entity<PaymentSchedule>(entity =>
        {
            entity.HasIndex(x => x.Name);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.NextPaymentDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.NextPaymentDate, x.IsDeleted })
                .HasDatabaseName("IX_PaymentSchedules_NextPaymentDate_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.NextPaymentDate })
                .HasDatabaseName("IX_PaymentSchedules_Status_NextPaymentDate");

            entity.HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.BankAccount)
                .WithMany()
                .HasForeignKey(x => x.BankAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.History)
                .WithOne(x => x.PaymentSchedule)
                .HasForeignKey(x => x.PaymentScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // PAYMENT SCHEDULE HISTORY
        // ============================================================
        modelBuilder.Entity<PaymentScheduleHistory>(entity =>
        {
            entity.HasIndex(x => x.PaymentScheduleId);
            entity.HasIndex(x => x.ScheduledDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.PaymentScheduleId, x.ScheduledDate })
                .HasDatabaseName("IX_PaymentScheduleHistories_PaymentScheduleId_ScheduledDate");

            entity.HasOne(x => x.PaymentSchedule)
                .WithMany(x => x.History)
                .HasForeignKey(x => x.PaymentScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // INTERNAL ORDER
        // ============================================================
        modelBuilder.Entity<InternalOrder>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.StartDate);
            entity.HasIndex(x => x.EndDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.StartDate, x.EndDate })
                .HasDatabaseName("IX_InternalOrders_StartDate_EndDate");

            entity.HasIndex(x => new { x.Status, x.StartDate })
                .HasDatabaseName("IX_InternalOrders_Status_StartDate");

            entity.HasOne(x => x.CostCenter)
                .WithMany()
                .HasForeignKey(x => x.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // COMPLIANCE
        // ============================================================
        modelBuilder.Entity<ComplianceRequirement>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.ComplianceStatus);
            entity.HasIndex(x => x.RiskLevel);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.ComplianceStatus, x.IsDeleted })
                .HasDatabaseName("IX_ComplianceRequirements_Status_IsDeleted");

            entity.HasIndex(x => new { x.RiskLevel, x.IsDeleted })
                .HasDatabaseName("IX_ComplianceRequirements_RiskLevel_IsDeleted");

            entity.HasMany(x => x.Controls)
                .WithOne(x => x.ComplianceRequirement)
                .HasForeignKey(x => x.ComplianceRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Evidence)
                .WithOne(x => x.ComplianceRequirement)
                .HasForeignKey(x => x.ComplianceRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // COMPLIANCE REQUIREMENT CONTROL
        // ============================================================
        modelBuilder.Entity<ComplianceRequirementControl>(entity =>
        {
            entity.HasIndex(x => x.ComplianceRequirementId);

            entity.HasOne(x => x.ComplianceRequirement)
                .WithMany(x => x.Controls)
                .HasForeignKey(x => x.ComplianceRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // COMPLIANCE REQUIREMENT EVIDENCE
        // ============================================================
        modelBuilder.Entity<ComplianceRequirementEvidence>(entity =>
        {
            entity.HasIndex(x => x.ComplianceRequirementId);

            entity.HasOne(x => x.ComplianceRequirement)
                .WithMany(x => x.Evidence)
                .HasForeignKey(x => x.ComplianceRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // COMPLIANCE REPORT
        // ============================================================
        modelBuilder.Entity<ComplianceReport>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.Type, x.Status })
                .HasDatabaseName("IX_ComplianceReports_Type_Status");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_ComplianceReports_PeriodId_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // INTERNAL CONTROL
        // ============================================================
        modelBuilder.Entity<InternalControl>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.Type, x.Status })
                .HasDatabaseName("IX_InternalControls_Type_Status");

            entity.HasIndex(x => new { x.Category, x.Status })
                .HasDatabaseName("IX_InternalControls_Category_Status");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // IFRS
        // ============================================================
        modelBuilder.Entity<IFRSReport>(entity =>
        {
            entity.HasIndex(x => x.Standard);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.Standard, x.Status })
                .HasDatabaseName("IX_IFRSReports_Standard_Status");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_IFRSReports_PeriodId_IsDeleted");

            entity.HasMany(x => x.Metrics)
                .WithOne(x => x.Report)
                .HasForeignKey(x => x.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // IFRS METRIC
        // ============================================================
        modelBuilder.Entity<IFRSMetric>(entity =>
        {
            entity.HasIndex(x => x.ReportId);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.ReportId, x.PeriodId })
                .HasDatabaseName("IX_IFRSMetrics_ReportId_PeriodId");

            entity.HasOne(x => x.Report)
                .WithMany(x => x.Metrics)
                .HasForeignKey(x => x.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CONSOLIDATION
        // ============================================================
        modelBuilder.Entity<Entity>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.IsDeleted);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Type, x.IsActive })
                .HasDatabaseName("IX_Entities_Type_IsActive");

            entity.HasOne(x => x.ParentEntity)
                .WithMany(x => x.Subsidiaries)
                .HasForeignKey(x => x.ParentEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CONSOLIDATION GROUP
        // ============================================================
        modelBuilder.Entity<ConsolidationGroup>(entity =>
        {
            entity.HasIndex(x => x.Name);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.ConsolidationDate);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.ConsolidationDate, x.IsDeleted })
                .HasDatabaseName("IX_ConsolidationGroups_Date_IsDeleted");

            entity.HasIndex(x => new { x.Status, x.ConsolidationDate })
                .HasDatabaseName("IX_ConsolidationGroups_Status_Date");

            entity.HasOne(x => x.ParentEntity)
                .WithMany()
                .HasForeignKey(x => x.ParentEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Entities)
                .WithOne()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.EliminationEntries)
                .WithOne(x => x.ConsolidationGroup)
                .HasForeignKey(x => x.ConsolidationGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CONSOLIDATION GROUP ENTITY
        // ============================================================
        modelBuilder.Entity<ConsolidationGroupEntity>(entity =>
        {
            entity.HasIndex(x => x.GroupId);
            entity.HasIndex(x => x.EntityId);

            // ✅ ADD: Composite index for performance
            entity.HasIndex(x => new { x.GroupId, x.EntityId })
                .HasDatabaseName("IX_ConsolidationGroupEntities_GroupId_EntityId");

            entity.HasOne(x => x.Group)
                .WithMany(x => x.Entities)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Entity)
                .WithMany()
                .HasForeignKey(x => x.EntityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // CONSOLIDATION REPORT
        // ============================================================
        modelBuilder.Entity<ConsolidationReport>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.Type, x.Status })
                .HasDatabaseName("IX_ConsolidationReports_Type_Status");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_ConsolidationReports_PeriodId_IsDeleted");

            entity.HasOne(x => x.ConsolidationGroup)
                .WithMany()
                .HasForeignKey(x => x.ConsolidationGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // ELIMINATION ENTRY
        // ============================================================
        modelBuilder.Entity<EliminationEntry>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.PeriodId);

            // ✅ ADD: Performance indexes for dashboard queries
            entity.HasIndex(x => new { x.Type, x.Status })
                .HasDatabaseName("IX_EliminationEntries_Type_Status");

            entity.HasIndex(x => new { x.PeriodId, x.IsDeleted })
                .HasDatabaseName("IX_EliminationEntries_PeriodId_IsDeleted");

            entity.HasOne(x => x.FromEntity)
                .WithMany()
                .HasForeignKey(x => x.FromEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ToEntity)
                .WithMany()
                .HasForeignKey(x => x.ToEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ConsolidationGroup)
                .WithMany(x => x.EliminationEntries)
                .HasForeignKey(x => x.ConsolidationGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // ============================================================
        // AUDIT LOG
        // ============================================================
      modelBuilder.Entity<AuditLog>(entity =>
      {
          entity.ToTable("AuditLogs");

          entity.HasKey(e => e.Id);

          // ✅ All nullable
          entity.Property(e => e.UserId).IsRequired(false);
          entity.Property(e => e.UserEmail).IsRequired(false);
          entity.Property(e => e.EntityId).IsRequired(false);

          // ✅ Required fields
          entity.Property(e => e.Action).IsRequired();
          entity.Property(e => e.EntityType).IsRequired();

          // ✅ FIX: Configure JSON properties to handle null/empty values
          entity.Property(e => e.ChangesJson)
              .HasColumnType("jsonb")
              .HasDefaultValue("{}")
              .HasConversion(
                  v => string.IsNullOrEmpty(v) ? "{}" : v,
                  v => string.IsNullOrEmpty(v) ? "{}" : v
              );

          entity.Property(e => e.MetadataJson)
              .HasColumnType("jsonb")
              .HasDefaultValue("{}")
              .HasConversion(
                  v => string.IsNullOrEmpty(v) ? "{}" : v,
                  v => string.IsNullOrEmpty(v) ? "{}" : v
              );

          entity.Property(e => e.NewValues)
              .HasColumnType("jsonb")
              .HasDefaultValue("{}")
              .HasConversion(
                  v => string.IsNullOrEmpty(v) ? "{}" : v,
                  v => string.IsNullOrEmpty(v) ? "{}" : v
              );

          entity.Property(e => e.OldValues)
              .HasColumnType("jsonb")
              .HasDefaultValue("{}")
              .HasConversion(
                  v => string.IsNullOrEmpty(v) ? "{}" : v,
                  v => string.IsNullOrEmpty(v) ? "{}" : v
              );

          entity.Property(e => e.RequestHeaders)
              .HasColumnType("jsonb")
              .HasDefaultValue("{}")
              .HasConversion(
                  v => string.IsNullOrEmpty(v) ? "{}" : v,
                  v => string.IsNullOrEmpty(v) ? "{}" : v
              );

          // Indexes
          entity.HasIndex(e => e.EntityType);
          entity.HasIndex(e => e.EntityId);
          entity.HasIndex(e => e.Action);
          entity.HasIndex(e => e.ActionDate);
          entity.HasIndex(e => e.UserId);

          // ✅ ADD: Performance indexes for dashboard queries
          entity.HasIndex(x => new { x.ActionDate, x.IsDeleted })
              .HasDatabaseName("IX_AuditLogs_ActionDate_IsDeleted");

          entity.HasIndex(x => new { x.EntityType, x.Action })
              .HasDatabaseName("IX_AuditLogs_EntityType_Action");

          entity.HasIndex(x => new { x.UserId, x.ActionDate })
              .HasDatabaseName("IX_AuditLogs_UserId_ActionDate");

          entity.HasQueryFilter(e => !e.IsDeleted);
      });

        // ============================================================
        // LOCAL ENTITIES
        // ============================================================
        modelBuilder.Entity<LocalCompany>(entity =>
        {
            entity.HasIndex(x => x.Name);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Name, x.IsDeleted })
                .HasDatabaseName("IX_LocalCompanies_Name_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalBranch>(entity =>
        {
            entity.HasIndex(x => x.Code);
            entity.HasIndex(x => x.Name);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Code, x.IsDeleted })
                .HasDatabaseName("IX_LocalBranches_Code_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalDepartment>(entity =>
        {
            entity.HasIndex(x => x.Name);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Name, x.IsDeleted })
                .HasDatabaseName("IX_LocalDepartments_Name_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalEmployee>(entity =>
        {
            entity.HasIndex(x => x.Code);
            entity.HasIndex(x => x.FirstName);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Code, x.IsDeleted })
                .HasDatabaseName("IX_LocalEmployees_Code_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalPosition>(entity =>
        {
            entity.HasIndex(x => x.Name);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Name, x.IsDeleted })
                .HasDatabaseName("IX_LocalPositions_Name_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<LocalJobGrade>(entity =>
        {
            entity.HasIndex(x => x.Name);

            // ✅ ADD: Index for dashboard queries
            entity.HasIndex(x => new { x.Name, x.IsDeleted })
                .HasDatabaseName("IX_LocalJobGrades_Name_IsDeleted");

            entity.HasQueryFilter(x => !x.IsDeleted);
        });
    }
    // ============================================================
    // HELPER METHOD: Convert all DateTime to UTC
    // ============================================================
    private static void UseUtcDateTimeConverter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(new UtcDateTimeConverter());
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new NullableUtcDateTimeConverter());
                }
            }
        }
    }
}

// ============================================================
// DATE TIME CONVERTERS FOR PostgreSQL UTC COMPATIBILITY
// ============================================================

/// <summary>
/// Converts DateTime to UTC for PostgreSQL compatibility
/// </summary>
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    { }
}

/// <summary>
/// Converts nullable DateTime to UTC for PostgreSQL compatibility
/// </summary>
public class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter()
        : base(
            v => v.HasValue && v.Value.Kind != DateTimeKind.Utc
                ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                : v,
            v => v.HasValue
                ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                : v)
    { }
}