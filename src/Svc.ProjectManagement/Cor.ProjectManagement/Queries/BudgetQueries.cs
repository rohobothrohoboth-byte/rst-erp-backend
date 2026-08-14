// Queries/BudgetQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.BudgetQueries
{
    public class GetBudgetByIdQuery : IRequest<ProjectBudgetDto>
    {
        public Guid Id { get; set; }
    }

    public class GetBudgetsByProjectQuery : IRequest<List<ProjectBudgetDto>>
    {
        public Guid ProjectId { get; set; }
        public BudgetCategory? Category { get; set; }
        public bool? IsApproved { get; set; }
    }

    public class GetBudgetSummaryQuery : IRequest<BudgetSummaryDto>
    {
        public Guid ProjectId { get; set; }
    }

    public class GetBudgetUtilizationQuery : IRequest<BudgetUtilizationDto>
    {
        public Guid ProjectId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}