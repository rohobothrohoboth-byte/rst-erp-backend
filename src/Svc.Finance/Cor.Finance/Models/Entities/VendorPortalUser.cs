// Models/Entities/VendorPortalUser.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class VendorPortalUser : BaseEntity
{
    [Required]
    public Guid VendorId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Role { get; set; } // 'Admin' | 'Submitter' | 'Viewer'

    [MaxLength(20)]
    public string? Status { get; set; } // 'Active' | 'Inactive' | 'Pending'

    public DateTime? LastLogin { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }
}