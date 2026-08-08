namespace Cor.Procurement.Models.DTOs;

public class InspectionDto
{
    public Guid Id { get; set; }
    public string InspectionNumber { get; set; } = string.Empty;
    public Guid GoodsReceiptNoteId { get; set; }
    public string? GrnNumber { get; set; }
    public DateTime InspectionDate { get; set; }
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public string Status { get; set; } = "InProgress";
    public string? Department { get; set; }
    public string? Remarks { get; set; }
    public decimal QualityScore { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int TotalItems { get; set; }
    public int ItemsPassed { get; set; }
    public int ItemsFailed { get; set; }
    public List<InspectionItemDto> Items { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class InspectionItemDto
{
    public Guid? Id { get; set; }
    public Guid PurchaseOrderItemId { get; set; }
    public string? Description { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityAccepted { get; set; }
    public int QuantityRejected { get; set; }
    public string? Condition { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? InspectedBy { get; set; }
    public string Status { get; set; } = "Pending";
}

public class CreateInspectionDto
{
    public Guid GoodsReceiptNoteId { get; set; }
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public DateTime InspectionDate { get; set; }
    public string? Department { get; set; }
    public string? Remarks { get; set; }
    public List<CreateInspectionItemDto> Items { get; set; } = new();
}

public class CreateInspectionItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public string? Description { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityAccepted { get; set; }
    public int QuantityRejected { get; set; }
    public string? Condition { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? UnitPrice { get; set; }
}

public class CompleteInspectionDto
{
    public Guid GrnId { get; set; }
    public TeamDto Team { get; set; } = new();
    public DateTime InspectionDate { get; set; }
    public List<CompleteInspectionItemDto> Items { get; set; } = new();
    public string? Remarks { get; set; }
    public decimal QualityScore { get; set; }
}

public class CompleteInspectionItemDto
{
    public Guid? Id { get; set; }
    public Guid PurchaseOrderItemId { get; set; }
    public int QuantityAccepted { get; set; }
    public int QuantityRejected { get; set; }
    public string? Condition { get; set; }
    public string? RejectionReason { get; set; }
    public string? InspectedBy { get; set; }
}

public class TeamDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LeaderId { get; set; } = string.Empty;
    public List<TeamMemberDto> Members { get; set; } = new();
    public string? Department { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EstimatedEndDate { get; set; }
}

public class TeamMemberDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<string> AssignedItems { get; set; } = new();
}