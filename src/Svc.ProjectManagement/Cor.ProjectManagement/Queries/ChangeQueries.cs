// Queries/ChangeQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.ChangeQueries
{
    public class GetChangeByIdQuery : IRequest<ProjectChangeDto>
    {
        public Guid Id { get; set; }
    }

    public class GetChangesByProjectQuery : IRequest<List<ProjectChangeDto>>
    {
        public Guid ProjectId { get; set; }
        public ChangeType? Type { get; set; }
        public ChangePriority? Priority { get; set; }
        public ChangeStatus? Status { get; set; }
    }

    public class GetChangeSummaryQuery : IRequest<ChangeSummaryDto>
    {
        public Guid ProjectId { get; set; }
    }
}