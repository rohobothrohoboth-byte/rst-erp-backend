// Commands/IssueCommands.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.IssueCommands
{
    public class CreateIssueCommand : IRequest<ProjectIssueDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public IssueType Type { get; set; }
        public IssuePriority Priority { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ReportedByName { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateIssueCommand : IRequest<ProjectIssueDto>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IssueType? Type { get; set; }
        public IssuePriority? Priority { get; set; }
        public IssueStatus? Status { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Resolution { get; set; }
        public string? RootCause { get; set; }
        public string? Impact { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteIssueCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class ResolveIssueCommand : IRequest<ProjectIssueDto>
    {
        public Guid Id { get; set; }
        public string Resolution { get; set; } = string.Empty;
        public string? RootCause { get; set; }
        public string? ResolvedBy { get; set; }
    }

    public class UpdateIssueStatusCommand : IRequest<ProjectIssueDto>
    {
        public Guid Id { get; set; }
        public IssueStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }
}