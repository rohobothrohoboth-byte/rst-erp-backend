// Queries/IssueQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.IssueQueries
{
    public class GetIssueByIdQuery : IRequest<ProjectIssueDto>
    {
        public Guid Id { get; set; }
    }

    public class GetIssuesByProjectQuery : IRequest<List<ProjectIssueDto>>
    {
        public Guid ProjectId { get; set; }
        public IssueType? Type { get; set; }
        public IssuePriority? Priority { get; set; }
        public IssueStatus? Status { get; set; }
        public Guid? AssignedToId { get; set; }
    }

    public class GetIssueSummaryQuery : IRequest<IssueSummaryDto>
    {
        public Guid ProjectId { get; set; }
    }
}