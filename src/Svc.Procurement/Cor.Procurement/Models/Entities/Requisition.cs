// Models/Entities/Requisition.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Procurement.Models.Entities.Local;
namespace Cor.Procurement.Models.Entities;

public class Requisition : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string RequisitionNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Guid? DepartmentId { get; set; }

    [MaxLength(200)]
    public string? DepartmentName { get; set; }

    public Guid? RequesterId { get; set; }

    [MaxLength(200)]
    public string? RequesterName { get; set; }

    [Required]
    public DateTime RequiredDate { get; set; }

    [Required]
    public DateTime SubmittedDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Priority { get; set; } = "Medium";

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft";

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(100)]
    public string? BudgetCode { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    public Guid? PurchaseOrderId { get; set; }

    [MaxLength(50)]
    public string? PurchaseOrderNumber { get; set; }

    public new bool IsDeleted { get; set; }

    public new DateTime DateAdd { get; set; }

    public new DateTime? DateMod { get; set; }

    [MaxLength(50)]
    public new string? RowVersion { get; set; }

    // ============================================================
    // ✅ NAVIGATION PROPERTIES
    // ============================================================

    [ForeignKey(nameof(DepartmentId))]
    public virtual LocalDepartment? Department { get; set; }

    [ForeignKey(nameof(PurchaseOrderId))]
    public virtual PurchaseOrder? PurchaseOrder { get; set; }

    public virtual ICollection<RequisitionLine> Lines { get; set; } = new List<RequisitionLine>();

    public virtual ICollection<RequisitionAttachment> Attachments { get; set; } = new List<RequisitionAttachment>();

    public virtual ICollection<RequisitionApproval> Approvals { get; set; } = new List<RequisitionApproval>();
}