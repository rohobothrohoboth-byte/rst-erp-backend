// src/Svc.Procurement/Cor.Procurement/Models/DTOs/ProcurementDtos.cs

namespace Cor.Procurement.Models.DTOs;

public class FinancialPeriodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PeriodType { get; set; } = "MONTHLY";
    public string Status { get; set; } = "OPEN";
    public bool IsClosed { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid? ClosedBy { get; set; }
    public string? Notes { get; set; }
    public string? FiscalYear { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public Guid? SourceId { get; set; }
    public string? Description { get; set; }
}

// ✅ NEW: ContactPerson DTO
public class ContactPersonDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Position { get; set; }
}

public class VendorDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string VendorType { get; set; } = "Supplier";
    public string Status { get; set; } = "Active";
    public string? PaymentTerms { get; set; }
    public string? Currency { get; set; }
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? Website { get; set; }

    // ✅ CHANGE: ContactPerson from string to ContactPersonDto
    public ContactPersonDto? ContactPerson { get; set; }

    public decimal? Rating { get; set; }
    public decimal? TotalSpent { get; set; }
    public int? TotalTransactions { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public Guid? SourceId { get; set; }
     public DateTime? SyncedAt { get; set; }
    public bool IsLocalOnly { get; set; }
}

public class PurchaseOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? DisplayNameAm { get; set; }
    public bool IsActive { get; set; }
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class RequisitionStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? DisplayNameAm { get; set; }
    public bool IsActive { get; set; }
}