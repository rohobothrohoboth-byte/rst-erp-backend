using FluentValidation;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Validators;

public class LoginVal : AbstractValidator<LoginDto>
{
    public LoginVal()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Please enter EMPLOYEE CODE.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter PASSWORD.");
    }
}