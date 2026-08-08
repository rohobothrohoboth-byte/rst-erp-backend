using System;

namespace Svc.Task.Models.Entities;

public class Task
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "pending";
    public string Priority { get; set; } = "medium";
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public Guid AssignedTo { get; set; }
    public Guid AssignedBy { get; set; }
    public string? Category { get; set; }
    public string? Module { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? LastOverdueNotified { get; set; } // ✅ Add this
}