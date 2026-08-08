// Models/Entities/AuditLog.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Inventory.Models.Enums;

namespace Cor.Inventory.Models.Entities;

public class AuditLog : BaseEntity
{
    // ✅ UserId - nullable
    public Guid? UserId { get; set; }

    // ✅ UserEmail - nullable
    [MaxLength(255)]
    public string? UserEmail { get; set; }

    [MaxLength(50)]
    public string? UserRole { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    // ✅ FIX: Make EntityId nullable (some endpoints don't have an entity ID)
    // [Required]  ← REMOVE THIS
    [MaxLength(100)]
    public string? EntityId { get; set; }

    [Column(TypeName = "jsonb")]
    public string? OldValues { get; set; }

    [Column(TypeName = "jsonb")]
    public string? NewValues { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ChangesJson { get; set; }

    [Column(TypeName = "jsonb")]
    public string? MetadataJson { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [MaxLength(200)]
    public string? UserName { get; set; }

    public string? RequestId { get; set; }

    public AuditStatus Status { get; set; } = AuditStatus.Success;

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    public long? DurationMs { get; set; }

    public DateTime ActionDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [Column(TypeName = "jsonb")]
    public string? RequestHeaders { get; set; }

    [MaxLength(500)]
    public string? QueryString { get; set; }

    [MaxLength(45)]
    public string? ClientIP { get; set; }


}