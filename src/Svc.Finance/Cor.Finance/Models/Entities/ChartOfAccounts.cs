using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Finance.Models.Entities.Local;
namespace Cor.Finance.Models.Entities;

public class ChartOfAccounts : BaseEntity
{
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
    [MaxLength(10)]  // ✅ ADD THIS - MaxLength for NormalBalance
    public string NormalBalance { get; set; } = "Debit"; // "Debit" or "Credit"

    public DateTime? OpeningBalanceDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }

    // ============================================================
    // ASSET SPECIFIC FIELDS (Only used for Asset type accounts)
    // ============================================================

    public int? UsefulLife { get; set; } // In years

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SalvageValue { get; set; } // Residual value

    public DateTime? AcquisitionDate { get; set; } // Purchase date

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

    // ============================================================
    // FOREIGN KEYS
    // ============================================================

    public Guid? CategoryId { get; set; }

    // ============================================================
    // NAVIGATION PROPERTIES
    // ============================================================

    [ForeignKey(nameof(CategoryId))]
    public virtual AccountCategory? Category { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual ChartOfAccounts? Parent { get; set; }

    public virtual ICollection<ChartOfAccounts> Children { get; set; } = new List<ChartOfAccounts>();

    [ForeignKey(nameof(DepartmentId))]
    public virtual LocalDepartment? Department { get; set; }

    // ============================================================
    // HELPER PROPERTIES (Not mapped to database)
    // ============================================================

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
    public bool IsDebitNormal => NormalBalance == "Debit";

    [NotMapped]
    public bool IsCreditNormal => NormalBalance == "Credit";
}