using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public abstract class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
    public bool IsDeleted { get; set; } = false;

    // ? Period tracking
    public Guid? PeriodId { get; set; }

    // ? Single navigation property for Period
    [ForeignKey("PeriodId")]
    public virtual FinancialPeriod? Period { get; set; }

    // Audit fields
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }

    public void SetRowVersion(uint rowVersion)
    {
        RowVersion = rowVersion.ToString();
    }


}