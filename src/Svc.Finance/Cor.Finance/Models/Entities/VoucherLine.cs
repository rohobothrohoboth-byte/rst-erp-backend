// Models/Entities/VoucherLine.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class VoucherLine : BaseEntity
{
    [Required]
    public Guid VoucherId { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DebitAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditAmount { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ❌ REMOVED duplicate: public Guid? PeriodId { get; set; }
    // ❌ REMOVED duplicate: public virtual FinancialPeriod? Period { get; set; }

    // Navigation
    [ForeignKey(nameof(VoucherId))]
    public virtual Voucher? Voucher { get; set; }

    [ForeignKey(nameof(AccountId))]
    public virtual ChartOfAccounts? Account { get; set; }
}