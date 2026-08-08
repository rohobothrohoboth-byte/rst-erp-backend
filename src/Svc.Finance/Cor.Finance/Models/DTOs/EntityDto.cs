// Models/DTOs/EntityDto.cs
namespace Cor.Finance.Models.DTOs;

public class EntityDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Country { get; set; }
    public string? Currency { get; set; }
    public DateTime? FiscalYearStart { get; set; }
    public DateTime? FiscalYearEnd { get; set; }
    public bool IsActive { get; set; }
    public string? ConsolidationMethod { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public Guid? ParentEntityId { get; set; }
    public string? ParentEntityName { get; set; }
    public decimal OwnershipPercentage { get; set; }
    public bool IsConsolidated { get; set; } // ✅ ADD THIS
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class AddEntityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Country { get; set; }
    public string? Currency { get; set; }
    public DateTime? FiscalYearStart { get; set; }
    public DateTime? FiscalYearEnd { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ConsolidationMethod { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public Guid? ParentEntityId { get; set; }
    public decimal? OwnershipPercentage { get; set; }
    public bool IsConsolidated { get; set; } = false; // ✅ ADD THIS
}

public class EditEntityDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Country { get; set; }
    public string? Currency { get; set; }
    public DateTime? FiscalYearStart { get; set; }
    public DateTime? FiscalYearEnd { get; set; }
    public bool IsActive { get; set; }
    public string? ConsolidationMethod { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public Guid? ParentEntityId { get; set; }
    public decimal? OwnershipPercentage { get; set; }
    public bool IsConsolidated { get; set; } // ✅ ADD THIS
    public string? RowVersion { get; set; }
}