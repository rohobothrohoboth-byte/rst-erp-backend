using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Commands;

// ============================================================
// CREATE BUDGET
// ============================================================
public class CreateBudgetCommand : IRequest<BudgetDto>
{
    public CreateBudgetDto CreateDto { get; set; } = new();
}

// ============================================================
// UPDATE BUDGET
// ============================================================
public class UpdateBudgetCommand : IRequest<BudgetDto>
{
    public UpdateBudgetDto UpdateDto { get; set; } = new();
}

// ============================================================
// DELETE BUDGET
// ============================================================
public class DeleteBudgetCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============================================================
// BULK CREATE BUDGETS
// ============================================================
public class BulkCreateBudgetsCommand : IRequest<List<BudgetDto>>
{
    public List<CreateBudgetDto> Budgets { get; set; } = new();
}

// ============================================================
// UPDATE BUDGET STATUS
// ============================================================
public class UpdateBudgetStatusCommand : IRequest<BudgetDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty; // Draft, Approved, InProgress, Completed
}