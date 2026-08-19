// Commands/TimesheetCommands/CreateTimesheetCommand.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Commands.TimesheetCommands
{
    public class CreateTimesheetCommand : IRequest<TimesheetDto>
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? TaskId { get; set; }
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal OvertimeHours { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public bool IsBillable { get; set; } = true;
        public string? CreatedBy { get; set; }
    }

    public class UpdateTimesheetCommand : IRequest<TimesheetDto>
    {
        public Guid Id { get; set; }
        public decimal? HoursWorked { get; set; }
        public decimal? OvertimeHours { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public Guid? TaskId { get; set; }
        public bool? IsBillable { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteTimesheetCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class SubmitTimesheetCommand : IRequest<List<TimesheetDto>>
    {
        public List<Guid> TimesheetIds { get; set; } = new List<Guid>();
        public string? SubmittedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class ApproveTimesheetCommand : IRequest<List<TimesheetDto>>
    {
        public List<Guid> TimesheetIds { get; set; } = new List<Guid>();
        public string? ApprovedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class RejectTimesheetCommand : IRequest<List<TimesheetDto>>
    {
        public List<Guid> TimesheetIds { get; set; } = new List<Guid>();
        public string? RejectedBy { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}