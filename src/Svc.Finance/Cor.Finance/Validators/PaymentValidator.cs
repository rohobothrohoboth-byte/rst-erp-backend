// Validators/PaymentValidator.cs

using Cor.Finance.Models.DTOs;
using FluentValidation;

namespace Cor.Finance.Validators;

public class AddPaymentValidator : AbstractValidator<AddPaymentDto>
{
    public AddPaymentValidator()
    {
        RuleFor(x => x.PeriodId)
            .NotEmpty()
            .WithMessage("PeriodId is required");

        RuleFor(x => x.PaymentDate)
            .NotEmpty()
            .WithMessage("Payment date is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required");

        RuleFor(x => x.PaymentType)
            .NotEmpty()
            .WithMessage("Payment type is required");

        RuleFor(x => x.PaymentType)
            .Must(x => x == "Purchase" || x == "Sales")
            .WithMessage("Payment type must be 'Purchase' or 'Sales'");
    }
}

public class EditPaymentValidator : AbstractValidator<EditPaymentDto>
{
    public EditPaymentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Payment ID is required");

        RuleFor(x => x.PeriodId)
            .NotEmpty()
            .WithMessage("PeriodId is required");

        RuleFor(x => x.PaymentDate)
            .NotEmpty()
            .WithMessage("Payment date is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required");
    }
}