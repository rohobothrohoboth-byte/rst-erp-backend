// Cor.CRM/Models/Entities/SocialMediaAccount.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum SocialMediaAccountStatus
{
    Connected = 1,
    Disconnected = 2,
    Expired = 3
}

public class SocialMediaAccount : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Platform { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AccountId { get; set; }

    [MaxLength(500)]
    public string? AccessToken { get; set; }

    [MaxLength(500)]
    public string? RefreshToken { get; set; }

    public DateTime? TokenExpiryDate { get; set; }

    public SocialMediaAccountStatus Status { get; set; } = SocialMediaAccountStatus.Connected;

    [MaxLength(200)]
    public string? PageId { get; set; }

    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

   // [MaxLength(50)]
   // public string? CreatedByUserId { get; set; }
}