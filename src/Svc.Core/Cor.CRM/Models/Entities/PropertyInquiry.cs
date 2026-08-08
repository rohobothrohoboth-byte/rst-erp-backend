// Cor.CRM/Models/Entities/PropertyInquiry.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum InquiryStatus
{
    New = 1,
    Contacted = 2,
    Viewed = 3,
    OfferMade = 4,
    Closed = 5
}

public class PropertyInquiry : BaseEntity
{
    public Guid PropertyId { get; set; }
    [ForeignKey(nameof(PropertyId))]
    public virtual Property? Property { get; set; }

    public Guid? LeadId { get; set; }
    [ForeignKey(nameof(LeadId))]
    public virtual Lead? Lead { get; set; }

    public Guid? CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public InquiryStatus Status { get; set; } = InquiryStatus.New;

    public DateTime? ContactedDate { get; set; }
    public DateTime? ViewedDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}