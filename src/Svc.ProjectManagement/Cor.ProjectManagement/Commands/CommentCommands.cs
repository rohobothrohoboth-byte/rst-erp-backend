// Commands/CommentCommands.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Commands.CommentCommands
{
    public class CreateCommentCommand : IRequest<ProjectCommentDto>
    {
        public string Content { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? MilestoneId { get; set; }
        public Guid? IssueId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string? AuthorName { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateCommentCommand : IRequest<ProjectCommentDto>
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsResolved { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteCommentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class PinCommentCommand : IRequest<ProjectCommentDto>
    {
        public Guid Id { get; set; }
        public bool IsPinned { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ResolveCommentCommand : IRequest<ProjectCommentDto>
    {
        public Guid Id { get; set; }
        public bool IsResolved { get; set; }
        public string? UpdatedBy { get; set; }
    }
}