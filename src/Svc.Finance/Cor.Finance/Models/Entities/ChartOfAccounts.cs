using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Finance.Models.Entities.Local;

namespace Cor.Finance.Models.Entities;

public class ChartOfAccounts : BaseEntity
{
    private string? _normalBalance;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [MaxLength(200)]
    public string? NameAm { get; set; }

    [Required]
    [MaxLength(50)]
    public string AccountType { get; set; } = default!;

    [MaxLength(50)]
    public string? AccountSubType { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? ParentId { get; set; }

    public int Level { get; set; } = 1;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? OpeningBalance { get; set; }

    [Required]
    [MaxLength(10)]
    public string NormalBalance
    {
        get => GetDefaultNormalBalance(AccountType);
        set => _normalBalance = NormalizeNormalBalance(value, AccountType);
    }

    public DateTime? OpeningBalanceDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }

    public int? UsefulLife { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalvageValue { get; set; }

    public DateTime? AcquisitionDate { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    [MaxLength(100)]
    public string? Manufacturer { get; set; }

    [MaxLength(100)]
    public string? Model { get; set; }

    [MaxLength(100)]
    public string? AssignedTo { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual AccountCategory? Category { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual ChartOfAccounts? Parent { get; set; }

    public virtual ICollection<ChartOfAccounts> Children { get; set; } = new List<ChartOfAccounts>();

    [ForeignKey(nameof(DepartmentId))]
    public virtual LocalDepartment? Department { get; set; }

    [NotMapped]
    public bool IsAsset => AccountType == "Asset";

    [NotMapped]
    public bool IsLiability => AccountType == "Liability";

    [NotMapped]
    public bool IsEquity => AccountType == "Equity";

    [NotMapped]
    public bool IsRevenue => AccountType == "Revenue";

    [NotMapped]
    public bool IsExpense => AccountType == "Expense";

    [NotMapped]
    public bool IsDebitNormal => string.Equals(NormalBalance, "Debit", StringComparison.OrdinalIgnoreCase);

    [NotMapped]
    public bool IsCreditNormal => string.Equals(NormalBalance, "Credit", StringComparison.OrdinalIgnoreCase);

    private static string GetDefaultNormalBalance(string? accountType) =>
        accountType?.Trim().ToLowerInvariant() switch
        {
            "liability" => "Credit",
            "equity" => "Credit",
            "revenue" => "Credit",
            "asset" => "Debit",
            "expense" => "Debit",
            _ => "Debit"
        };

    private static string NormalizeNormalBalance(string? value, string? accountType)
    {
        // Normal balance is foundationally determined by AccountType.
        // The persisted property remains for compatibility, but account type
        // is the authoritative source for the normal-side classification.
        return GetDefaultNormalBalance(accountType);
    }
}