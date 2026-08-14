// Validators/ProjectValidator.cs
using FluentValidation;
using Cor.ProjectManagement.Commands.ProjectCommands;
 using Cor.ProjectManagement.Commands.MilestoneCommands;
 using Cor.ProjectManagement.Commands.ResourceCommands;
 using Cor.ProjectManagement.Commands.TimesheetCommands;
 using Cor.ProjectManagement.Commands.TaskCommands;
namespace Cor.ProjectManagement.Validators
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).WithMessage("Budget must be greater than or equal to 0");

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 5).WithMessage("Priority must be between 1 and 5");

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate)
                .WithMessage("End date must be after start date");
        }
    }

    public class UpdateProjectValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.Name)
                .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage("Project name cannot exceed 200 characters");

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue)
                .WithMessage("Budget must be greater than or equal to 0");

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 5).When(x => x.Priority.HasValue)
                .WithMessage("Priority must be between 1 and 5");

            RuleFor(x => x)
                .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate.Value)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("End date must be after start date");
        }
    }

    public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.EstimatedHours)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated hours must be greater than or equal to 0");

            RuleFor(x => x.EstimatedCost)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated cost must be greater than or equal to 0");

            RuleFor(x => x)
                .Must(x => !x.DueDate.HasValue || x.DueDate.Value >= x.StartDate)
                .WithMessage("Due date must be after start date");
        }
    }

    public class CreateTimesheetValidator : AbstractValidator<CreateTimesheetCommand>
    {
        public CreateTimesheetValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .Must(d => d <= DateTime.UtcNow).WithMessage("Date cannot be in the future");

            RuleFor(x => x.HoursWorked)
                .GreaterThan(0).WithMessage("Hours worked must be greater than 0")
                .LessThanOrEqualTo(24).WithMessage("Hours worked cannot exceed 24");

            RuleFor(x => x.OvertimeHours)
                .GreaterThanOrEqualTo(0).WithMessage("Overtime hours must be greater than or equal to 0");

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(0).WithMessage("Hourly rate must be greater than or equal to 0");
        }
    }

    public class AllocateResourceValidator : AbstractValidator<AllocateResourceCommand>
    {
        public AllocateResourceValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.ResourceId)
                .NotEmpty().WithMessage("Resource ID is required");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid resource type");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            RuleFor(x => x.CostPerUnit)
                .GreaterThanOrEqualTo(0).WithMessage("Cost per unit must be greater than or equal to 0");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x)
                .Must(x => !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate)
                .WithMessage("End date must be after start date");
        }
    }

    public class CreateMilestoneValidator : AbstractValidator<CreateMilestoneCommand>
    {
        public CreateMilestoneValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Milestone title is required")
                .MaximumLength(200).WithMessage("Milestone title cannot exceed 200 characters");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required");

            RuleFor(x => x)
                .Must(x => x.DueDate >= DateTime.UtcNow.AddDays(-1))
                .WithMessage("Due date must not be in the distant past");
        }
    }
}