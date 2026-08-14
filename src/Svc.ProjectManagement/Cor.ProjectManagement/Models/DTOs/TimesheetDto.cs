// Models/DTOs/TimesheetDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class TimesheetDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? TaskId { get; set; }
        public string? TaskName { get; set; }
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal OvertimeHours { get; set; }
        public string Description { get; set; } = string.Empty;
        public TimesheetStatus Status { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public Guid? SubmittedById { get; set; }
        public string? SubmittedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public string ApprovalNote { get; set; } = string.Empty;
        public DateTime? RejectedAt { get; set; }
        public Guid? RejectedById { get; set; }
        public string? RejectedByName { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class TimesheetCreateDto
    {
        [Required]
        public Guid UserId { get; set; }

        public string? UserName { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? TaskId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal HoursWorked { get; set; }

        public decimal OvertimeHours { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal HourlyRate { get; set; }

        public bool IsBillable { get; set; } = true;

        public string? CreatedBy { get; set; }
    }

    public class TimesheetUpdateDto
    {
        public decimal? HoursWorked { get; set; }
        public decimal? OvertimeHours { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public Guid? TaskId { get; set; }
        public bool? IsBillable { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class TimesheetSubmitDto
    {
        public List<Guid> TimesheetIds { get; set; } = new List<Guid>();
        public string? SubmittedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class TimesheetApproveDto
    {
        public List<Guid> TimesheetIds { get; set; } = new List<Guid>();
        public string? ApprovedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class TimesheetRejectDto
    {
        public List<Guid> TimesheetIds { get; set; } = new List<Guid>();
        public string? RejectedBy { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
    }

     public class TimesheetSummaryDto
        {
            public Guid UserId { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public decimal TotalHours { get; set; }
            public decimal OvertimeHours { get; set; }
            public decimal TotalAmount { get; set; }
            public List<DailyTimesheetSummaryDto> DailySummary { get; set; } = new();
            public List<ProjectTimesheetSummaryDto> ProjectSummary { get; set; } = new();
        }

        public class DailyTimesheetSummaryDto
        {
            public DateTime Date { get; set; }
            public decimal Hours { get; set; }
            public decimal Overtime { get; set; }
            public decimal Amount { get; set; }
        }

        public class ProjectTimesheetSummaryDto
        {
            public Guid ProjectId { get; set; }
            public string ProjectName { get; set; } = string.Empty;
            public decimal TotalHours { get; set; }
            public decimal TotalAmount { get; set; }
        }
        public class TimesheetFilterDto
            {
                public Guid? ProjectId { get; set; }
                public Guid? UserId { get; set; }
                public DateTime? FromDate { get; set; }
                public DateTime? ToDate { get; set; }
                public TimesheetStatus? Status { get; set; }
                public int Page { get; set; } = 1;
                public int PageSize { get; set; } = 20;
                public string? OrderBy { get; set; }
                public bool Descending { get; set; } = false;
            }

             public class SubmitTimesheetDto
                {
                    public List<Guid> TimesheetIds { get; set; } = new();
                    public string? SubmittedBy { get; set; }
                    public string? Notes { get; set; }
                }

                public class ApproveTimesheetDto
                {
                    public List<Guid> TimesheetIds { get; set; } = new();
                    public string? ApprovedBy { get; set; }
                    public string? Notes { get; set; }
                }

                public class RejectTimesheetDto
                {
                    public List<Guid> TimesheetIds { get; set; } = new();
                    public string? RejectedBy { get; set; }
                    public string Reason { get; set; } = string.Empty;
                }
}