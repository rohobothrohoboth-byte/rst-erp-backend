// Commands/BudgetCommands.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.BudgetCommands
{
    public class CreateBudgetCommand : IRequest<ProjectBudgetDto>
    {
        public Guid ProjectId { get; set; }
        public BudgetCategory Category { get; set; }
        public string? CategoryName { get; set; }
        public decimal PlannedAmount { get; set; }
        public DateTime? PlannedDate { get; set; }
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }

    public class UpdateBudgetCommand : IRequest<ProjectBudgetDto>
    {
        public Guid Id { get; set; }
        public decimal? PlannedAmount { get; set; }
        public decimal? ActualAmount { get; set; }
        public decimal? CommittedAmount { get; set; }
        public bool? IsApproved { get; set; }
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteBudgetCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class ApproveBudgetCommand : IRequest<ProjectBudgetDto>
    {
        public Guid Id { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Notes { get; set; }
    }
}