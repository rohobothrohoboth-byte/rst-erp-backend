// Persistence/ProcurementDbContext.cs
using Microsoft.EntityFrameworkCore;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Models.Entities.Local;
using System.Reflection;

namespace Cor.Procurement.Persistence;

public class ProcurementDbContext : DbContext
{
    public ProcurementDbContext(DbContextOptions<ProcurementDbContext> options)
        : base(options)
    {
    }

    // Local Entities
    public DbSet<LocalCompany> LocalCompanies { get; set; }
    public DbSet<LocalBranch> LocalBranches { get; set; }
    public DbSet<LocalDepartment> LocalDepartments { get; set; }
    public DbSet<LocalEmployee> LocalEmployees { get; set; }
    public DbSet<LocalPosition> LocalPositions { get; set; }
    public DbSet<LocalJobGrade> LocalJobGrades { get; set; }

    // Reference Data
    public DbSet<FinancialPeriod> FinancialPeriods { get; set; }
    public DbSet<Vendor> Vendors { get; set; }

    // Procurement Entities
    public DbSet<Requisition> Requisitions { get; set; }
    public DbSet<RequisitionLine> RequisitionLines { get; set; }
    public DbSet<RequisitionAttachment> RequisitionAttachments { get; set; }
    public DbSet<RequisitionApproval> RequisitionApprovals { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
    public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }
    public DbSet<LocalWarehouse> LocalWarehouses { get; set; }
public DbSet<Report> Reports { get; set; }
     public DbSet<Inspection> Inspections { get; set; }
        public DbSet<InspectionItem> InspectionItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
            public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
            public DbSet<VendorEvaluation> VendorEvaluations { get; set; }
              public DbSet<VendorContract> VendorContracts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ FIRST: Configure RowVersion for all BaseEntity types BEFORE other configurations
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var rowVersionProperty = entityType.FindProperty("RowVersion");
                if (rowVersionProperty != null)
                {
                    // ✅ THIS IS THE CRITICAL FIX - Disable concurrency token
                    rowVersionProperty.IsConcurrencyToken = false;
                    rowVersionProperty.SetMaxLength(50);
                }

                // Configure soft delete filter
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
                var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // LocalEmployee configuration
        modelBuilder.Entity<LocalEmployee>(entity =>
        {
            entity.HasIndex(x => x.Code);
            entity.HasIndex(x => x.FirstName);
            entity.HasIndex(x => new { x.Code, x.IsDeleted })
                .HasDatabaseName("IX_LocalEmployees_Code_IsDeleted");
            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        // FinancialPeriod configuration
        modelBuilder.Entity<FinancialPeriod>(entity =>
        {
            entity.ToTable("FinancialPeriods");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.PeriodType).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.FiscalYear).HasMaxLength(10);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Vendor configuration
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.ToTable("Vendors");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.TaxId);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAm).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);
            entity.Property(e => e.VendorType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.PaymentTerms).HasMaxLength(200);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.BankAccount).HasMaxLength(50);
            entity.Property(e => e.Website).HasMaxLength(200);
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.Rating).HasPrecision(3, 2);
            entity.Property(e => e.TotalSpent).HasPrecision(18, 2);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Requisition configuration
        modelBuilder.Entity<Requisition>(entity =>
        {
            entity.ToTable("Requisitions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RequisitionNumber).IsUnique();
            entity.Property(e => e.RequisitionNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DepartmentName).HasMaxLength(200);
            entity.Property(e => e.RequesterName).HasMaxLength(200);
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BudgetCode).HasMaxLength(100);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.PurchaseOrderNumber).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.RowVersion).HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // PurchaseOrder configuration
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.ToTable("PurchaseOrders");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PurchaseOrderNumber).IsUnique();
            entity.Property(e => e.PurchaseOrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.VendorName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PaymentTerms).HasMaxLength(200);
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.ReceivedBy).HasMaxLength(100);
            entity.Property(e => e.RequisitionNumber).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.RowVersion).HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // RequisitionLine configuration
        modelBuilder.Entity<RequisitionLine>(entity =>
        {
            entity.ToTable("RequisitionLines");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.RowVersion).HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Requisition)
                .WithMany(e => e.Lines)
                .HasForeignKey(e => e.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PurchaseOrderLine configuration
        modelBuilder.Entity<PurchaseOrderLine>(entity =>
        {
            entity.ToTable("PurchaseOrderLines");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.TaxRate).HasPrecision(5, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.RowVersion).HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.PurchaseOrder)
                .WithMany(e => e.Lines)
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RequisitionAttachment configuration
        modelBuilder.Entity<RequisitionAttachment>(entity =>
        {
            entity.ToTable("RequisitionAttachments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UploadedBy).HasMaxLength(100);
            entity.Property(e => e.RowVersion).HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Requisition)
                .WithMany(e => e.Attachments)
                .HasForeignKey(e => e.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RequisitionApproval configuration
        modelBuilder.Entity<RequisitionApproval>(entity =>
        {
            entity.ToTable("RequisitionApprovals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Comments).HasMaxLength(500);
            entity.Property(e => e.ApproverName).HasMaxLength(200);
            entity.Property(e => e.RowVersion).HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Requisition)
                .WithMany(e => e.Approvals)
                .HasForeignKey(e => e.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Relationships
        modelBuilder.Entity<Requisition>()
            .HasOne(e => e.PurchaseOrder)
            .WithOne(e => e.Requisition)
            .HasForeignKey<Requisition>(e => e.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(e => e.Period)
            .WithMany()
            .HasForeignKey(e => e.PeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}