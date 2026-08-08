using FluentValidation;
using Svc.Notification.Models.Dtos;

namespace Svc.Notification.Validators;

public class NotificationAddVal : AbstractValidator<NotificationAddDto>
{
    public NotificationAddVal()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters");

        RuleFor(x => x.Type)
            .Must(x => new[] { "info", "success", "warning", "error" }.Contains(x))
            .WithMessage("Type must be info, success, warning, or error");

        RuleFor(x => x.Priority)
            .Must(x => new[] { "low", "medium", "high", "urgent" }.Contains(x))
            .WithMessage("Priority must be low, medium, high, or urgent");
    }
}