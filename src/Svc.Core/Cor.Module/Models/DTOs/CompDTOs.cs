using System.Text.Json.Serialization;

namespace Cor.Module.Models.DTOs;

public class CompListDto : BaseDTO
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string BranchCount { get; set; } = default!;
    [JsonIgnore]
    public int CountBra { get; set; }
}

public class AddCompDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
}

public class EditCompDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class CompDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}
