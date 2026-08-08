using FluentValidation;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Validators;

public class LoginVal : AbstractValidator<LoginDto>
{
    public LoginVal()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("EMPLOYEE CODE is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("PASSWORD is required");
    }
}