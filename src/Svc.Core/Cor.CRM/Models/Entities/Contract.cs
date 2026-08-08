// Cor.CRM/Models/Entities/Contract.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum ContractStatus
{
    Draft = 1,
    Pending = 2,
    Active = 3,
    Signed = 4,
    Expired = 5,
    Terminated = 6
}

public class Contract : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public Guid CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    public Guid? OpportunityId { get; set; }
    [ForeignKey(nameof(OpportunityId))]
    public virtual Opportunity? Opportunity { get; set; }

    public Guid? QuoteId { get; set; }
    [ForeignKey(nameof(QuoteId))]
    public virtual Quote? Quote { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalValue { get; set; }

    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }

    [MaxLength(2000)]
    public string? TermsAndConditions { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation Properties
    public virtual ICollection<ContractLine> ContractLines { get; set; } = new List<ContractLine>();
}