namespace Svc.HRM.Performance.Models.DTOs;

public class FeedbackDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? FromEmployeeId { get; set; }
    public Guid? ReviewId { get; set; }
    public string FeedbackType { get; set; } = default!;
    public string Content { get; set; } = default!;
    public int? Rating { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime DateAdd { get; set; }
}

public class FeedbackCreateDto
{
    public Guid EmployeeId { get; set; }
    public Guid? FromEmployeeId { get; set; }
    public Guid? ReviewId { get; set; }
    public string FeedbackType { get; set; } = "Peer";
    public string Content { get; set; } = default!;
    public int? Rating { get; set; }
    public bool IsAnonymous { get; set; }
}

public class ReviewTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? CriteriaJson { get; set; }
    public bool IsActive { get; set; }
}

public class ReviewTemplateCreateDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? CriteriaJson { get; set; }
    public bool IsActive { get; set; } = true;
}
