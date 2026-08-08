using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Queries;

public class GetBudgetsByProjectQuery : IRequest<List<BudgetDto>>
{
    public Guid ProjectId { get; set; }
}

public class GetBudgetByIdQuery : IRequest<BudgetDto>
{
    public Guid Id { get; set; }
}

public class GetBudgetSummaryQuery : IRequest<BudgetSummaryDto>
{
    public Guid ProjectId { get; set; }
}



public class GetBudgetByCategoryQuery : IRequest<List<BudgetDto>>
{
    public Guid ProjectId { get; set; }
    public string Category { get; set; } = string.Empty;
}
public class GetBudgetByProjectQuery : IRequest<BudgetDto>
{
    public Guid ProjectId { get; set; }
}