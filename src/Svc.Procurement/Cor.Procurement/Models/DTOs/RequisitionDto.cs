// Models/DTOs/RequisitionDto.cs
using System;
using System.Collections.Generic;

namespace Cor.Procurement.Models.DTOs;

public class RequisitionDto
{
    public Guid Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? RequesterId { get; set; }
    public string? RequesterName { get; set; }
    public DateTime RequiredDate { get; set; }
    public DateTime SubmittedDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Draft";
    public decimal TotalAmount { get; set; }
    public string? BudgetCode { get; set; }
    public List<RequisitionLineDto> Lines { get; set; } = new();
    public List<RequisitionAttachmentDto> Attachments { get; set; } = new();
    public List<RequisitionApprovalDto> Approvals { get; set; } = new();
    public Guid? PurchaseOrderId { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class RequisitionLineDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? Notes { get; set; }
}

public class RequisitionAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? UploadedBy { get; set; }
}

public class RequisitionApprovalDto
{
    public Guid Id { get; set; }
    public Guid ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Comments { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int ApprovalLevel { get; set; }
}

public class CreateRequisitionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? RequesterId { get; set; }
    public string? RequesterName { get; set; }
    public DateTime RequiredDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string? BudgetCode { get; set; }

    // ✅ ADD THESE PROPERTIES
    public Guid? PeriodId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }

    public List<CreateRequisitionLineDto> Lines { get; set; } = new();
}


public class UpdateRequisitionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime RequiredDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string? BudgetCode { get; set; }
    public List<UpdateRequisitionLineDto> Lines { get; set; } = new();
    public string? RowVersion { get; set; }
}

public class CreateRequisitionLineDto
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? Notes { get; set; }
}

public class UpdateRequisitionLineDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? Notes { get; set; }
}

public class RequisitionApprovalActionDto
{
    public Guid RequisitionId { get; set; }
    public string? Action { get; set; } // "Approve" or "Reject"
    public string? Comments { get; set; }
    public string? RejectionReason { get; set; }
    public Guid? ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public int? ApprovalLevel { get; set; }
}