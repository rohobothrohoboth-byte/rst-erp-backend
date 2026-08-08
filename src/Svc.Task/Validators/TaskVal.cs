using FluentValidation;
using Svc.Task.Models.Dtos;

namespace Svc.Task.Validators;

public class TaskAddVal : AbstractValidator<TaskAddDto>
{
    public TaskAddVal()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");

        RuleFor(x => x.AssignedTo)
            .NotEmpty().WithMessage("Assigned user is required");

        RuleFor(x => x.AssignedBy)
            .NotEmpty().WithMessage("Created by user is required");

        RuleFor(x => x.Priority)
            .Must(x => new[] { "low", "medium", "high", "urgent" }.Contains(x))
            .WithMessage("Priority must be low, medium, high, or urgent");
    }
}

public class TaskUpdateStatusVal : AbstractValidator<UpdateTaskStatusDto>
{
    public TaskUpdateStatusVal()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(x => new[] { "pending", "in_progress", "completed", "cancelled", "overdue" }.Contains(x))
            .WithMessage("Status must be pending, in_progress, completed, cancelled, or overdue");
    }
}