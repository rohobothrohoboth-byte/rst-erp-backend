using System.ComponentModel.DataAnnotations;

namespace Cor.Procurement.Models.Entities.Local;

public class LocalWarehouse : LocalBaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Location { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? WarehouseType { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } = "Active";

    public bool IsActive { get; set; } = true;

    // Local copy tracking
    public DateTime? SyncedAt { get; set; }
    public Guid? SourceId { get; set; }
}