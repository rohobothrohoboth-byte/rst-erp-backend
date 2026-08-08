// Models/Entities/BaseEntity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Procurement.Models.Entities.Local;
namespace Cor.Procurement.Models.Entities;

public abstract class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }

    [MaxLength(50)]
    public string? RowVersion { get; set; }

    public bool IsDeleted { get; set; } = false;

    public Guid? PeriodId { get; set; }

    [ForeignKey("PeriodId")]
    public virtual FinancialPeriod? Period { get; set; }

    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }

    public void UpdateRowVersion()
    {
        RowVersion = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}