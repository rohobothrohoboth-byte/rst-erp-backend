namespace Cor.Procurement.Models.DTOs;

public class VendorContractDto
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? VendorCode { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "Supply";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Value { get; set; }
    public string Status { get; set; } = "Pending";
    public bool AutoRenew { get; set; }
    public DateTime? RenewalDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public int AttachmentCount { get; set; }
    public List<string> Terms { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class CreateVendorContractDto
{
    public Guid VendorId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "Supply";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Value { get; set; }
    public string Status { get; set; } = "Pending";
    public bool AutoRenew { get; set; }
    public DateTime? RenewalDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public List<string> Terms { get; set; } = new();
    public string? Notes { get; set; }
}

public class UpdateVendorContractDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "Supply";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Value { get; set; }
    public string Status { get; set; } = "Pending";
    public bool AutoRenew { get; set; }
    public DateTime? RenewalDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public List<string> Terms { get; set; } = new();
    public string? Notes { get; set; }
    public string? RowVersion { get; set; }
}