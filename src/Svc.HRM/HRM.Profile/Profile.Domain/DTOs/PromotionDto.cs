namespace Profile.Domain.DTOs;

public sealed class PromotionListDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string? FromPosition { get; set; }
    public string ToPosition { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = default!;
    public string? ApprovedBy { get; set; }
}

public sealed class PromotionAddDto
{
    public Guid EmployeeId { get; set; }
    public string? FromPosition { get; set; }
    public string ToPosition { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";
}

public sealed class PromotionModDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? FromPosition { get; set; }
    public string ToPosition { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ApprovedBy { get; set; }
}

public sealed class PromotionApproveDto
{
    public Guid Id { get; set; }
    public string? ApprovedBy { get; set; }
}
