// Validators/BudgetValidator.cs
using FluentValidation;
using Cor.ProjectManagement.Commands.BudgetCommands;

namespace Cor.ProjectManagement.Validators
{
    public class CreateBudgetValidator : AbstractValidator<CreateBudgetCommand>
    {
        public CreateBudgetValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid budget category");

            RuleFor(x => x.PlannedAmount)
                .GreaterThan(0).WithMessage("Planned amount must be greater than 0")
                .LessThan(1000000000).WithMessage("Planned amount is too large");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }

    public class UpdateBudgetValidator : AbstractValidator<UpdateBudgetCommand>
    {
        public UpdateBudgetValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Budget ID is required");

            RuleFor(x => x.PlannedAmount)
                .GreaterThan(0).When(x => x.PlannedAmount.HasValue)
                .WithMessage("Planned amount must be greater than 0");

            RuleFor(x => x.ActualAmount)
                .GreaterThanOrEqualTo(0).When(x => x.ActualAmount.HasValue)
                .WithMessage("Actual amount must be greater than or equal to 0");

            RuleFor(x => x.CommittedAmount)
                .GreaterThanOrEqualTo(0).When(x => x.CommittedAmount.HasValue)
                .WithMessage("Committed amount must be greater than or equal to 0");
        }
    }
}