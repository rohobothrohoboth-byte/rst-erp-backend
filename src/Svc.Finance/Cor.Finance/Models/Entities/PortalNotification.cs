// Models/Entities/PortalNotification.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class PortalNotification : BaseEntity
{
    [Required]
    public Guid VendorId { get; set; }

    [MaxLength(50)]
    public string? Type { get; set; } // 'Invoice_Approved' | 'Invoice_Rejected' | 'Payment_Scheduled' | 'Payment_Made' | 'Reminder' | 'Portal_Update'

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    [MaxLength(500)]
    public string? Link { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }
}