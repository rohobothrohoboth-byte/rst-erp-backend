// Models/Entities/Timesheet.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cor.ProjectManagement.Models.Entities
{
    public enum TimesheetStatus
    {
        Draft = 1,
        Submitted = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        Paid = 6
    }

    public class Timesheet : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? TaskId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal HoursWorked { get; set; }

        public decimal OvertimeHours { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        public TimesheetStatus Status { get; set; } = TimesheetStatus.Draft;

        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime? SubmittedAt { get; set; }
        public Guid? SubmittedById { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public string ApprovalNote { get; set; } = string.Empty;

        public DateTime? RejectedAt { get; set; }
        public Guid? RejectedById { get; set; }
        public string RejectionReason { get; set; } = string.Empty;

        public bool IsBillable { get; set; } = true;

        public string? TaskName { get; set; }

        public virtual Project Project { get; set; } = null!;
        public virtual ProjectTask? Task { get; set; }
    }
}