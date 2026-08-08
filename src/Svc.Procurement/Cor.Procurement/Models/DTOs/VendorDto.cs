namespace Cor.Procurement.Models.DTOs;





public class CreateVendorDto
{
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
    public ContactPersonDto? ContactPerson { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateVendorDto
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
    public ContactPersonDto? ContactPerson { get; set; }
    public bool? IsActive { get; set; }
    public string? RowVersion { get; set; }
}