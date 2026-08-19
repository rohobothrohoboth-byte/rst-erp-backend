// Cor.Finance/Models/Entities/AccountType.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class AccountType : BaseEntity
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
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Asset, Liability, Equity, Revenue, Expense

    [Required]
    [MaxLength(10)]
    public string NormalBalance { get; set; } = "Debit"; // Debit or Credit

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    // Navigation
    public virtual ICollection<AccountSubtype> Subtypes { get; set; } = new List<AccountSubtype>();
}