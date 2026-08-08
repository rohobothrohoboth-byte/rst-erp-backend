// Cor.CRM/Models/Entities/PropertyViewing.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.CRM.Models.Entities.Local;
namespace Cor.CRM.Models.Entities;

public enum ViewingStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}

public class PropertyViewing : BaseEntity
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

    public Guid? AgentId { get; set; }
    [ForeignKey(nameof(AgentId))]
    public virtual LocalEmployee? Agent { get; set; }

    public DateTime ViewingDate { get; set; }
    public DateTime? EndTime { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public ViewingStatus Status { get; set; } = ViewingStatus.Scheduled;

    [MaxLength(500)]
    public string? Feedback { get; set; }

    public int? Rating { get; set; } // 1-5

    public bool IsVirtual { get; set; }
    public bool IsFollowUp { get; set; }
}