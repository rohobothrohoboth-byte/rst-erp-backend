// Queries/CommentQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Queries.CommentQueries
{
    public class GetCommentByIdQuery : IRequest<ProjectCommentDto>
    {
        public Guid Id { get; set; }
    }

    public class GetCommentsByProjectQuery : IRequest<List<ProjectCommentDto>>
    {
        public Guid ProjectId { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? MilestoneId { get; set; }
        public Guid? IssueId { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsResolved { get; set; }
    }

    public class GetCommentThreadQuery : IRequest<List<ProjectCommentDto>>
    {
        public Guid CommentId { get; set; }
    }
}