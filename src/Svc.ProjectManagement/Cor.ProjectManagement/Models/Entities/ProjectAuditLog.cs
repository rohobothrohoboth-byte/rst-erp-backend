// Models/Entities/ProjectAuditLog.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum AuditAction
    {
        Created = 1,
        Updated = 2,
        Deleted = 3,
        Restored = 4,
        StatusChanged = 5,
        Assigned = 6,
        Unassigned = 7,
        Approved = 8,
        Rejected = 9,
        Submitted = 10,
        Completed = 11,
        Commented = 12,
        DocumentAdded = 13,
        DocumentRemoved = 14,
        ResourceAllocated = 15,
        ResourceReleased = 16,
        BudgetAdjusted = 17,
        RiskUpdated = 18,
        IssueResolved = 19,
        ChangeImplemented = 20
    }

    public class ProjectAuditLog : BaseEntity
    {
        [Required]
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        public AuditAction Action { get; set; }

        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;

        public string ActionDescription { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string? UserRole { get; set; }
        public string? UserDepartment { get; set; }

        public string? ClientIp { get; set; }
        public string? UserAgent { get; set; }

        [Column(TypeName = "jsonb")]
        public string? OldValues { get; set; }

        [Column(TypeName = "jsonb")]
        public string? NewValues { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        // Navigation Property
        public virtual Project Project { get; set; } = null!;
    }
}