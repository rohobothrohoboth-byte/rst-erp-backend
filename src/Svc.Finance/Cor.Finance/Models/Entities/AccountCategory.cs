using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class AccountCategory : BaseEntity
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
    public string Type { get; set; } = string.Empty; // Asset, Liability, Equity, Revenue, Expense

    public bool IsActive { get; set; } = true;

    public new bool IsDeleted { get; set; } = false;

    public Guid? ParentId { get; set; }

    // Navigation
    [ForeignKey(nameof(ParentId))]
    public virtual AccountCategory? Parent { get; set; }

    public virtual ICollection<AccountCategory> Children { get; set; } = new List<AccountCategory>();

    public virtual ICollection<ChartOfAccounts> Accounts { get; set; } = new List<ChartOfAccounts>();
}