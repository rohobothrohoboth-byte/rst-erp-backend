// Cor.Finance/Models/Entities/AccountSubtype.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class AccountSubtype : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAm { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid AccountTypeId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    // Navigation
    [ForeignKey(nameof(AccountTypeId))]
    public virtual AccountType AccountType { get; set; } = null!;
}