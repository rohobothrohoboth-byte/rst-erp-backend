// Cor.CRM/Models/Entities/ContractLine.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class ContractLine : BaseEntity
{
    public Guid ContractId { get; set; }
    [ForeignKey(nameof(ContractId))]
    public virtual Contract? Contract { get; set; }

    public Guid? ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public int SortOrder { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}