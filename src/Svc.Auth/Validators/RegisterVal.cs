using FluentValidation;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Validators;

public class RegisterStp1Val : AbstractValidator<RegStep1>
{
    public RegisterStp1Val()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee Id is not found.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("User ROLE should be selected.");
        RuleFor(x => x.PerModules).NotEmpty().WithMessage("At least one MODULE must be provided.");
    }
}


public class RegisterStp2Val : AbstractValidator<RegStep2>
{
    public RegisterStp2Val()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Employee Id is not found.");
        RuleFor(x => x.PerMenus).NotEmpty().WithMessage("At least one MENU PERMISSION must be provided.");
    }
}

public class RegisterStp3Val : AbstractValidator<RegStep3>
{
    public RegisterStp3Val()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Employee Id is not found.");
        RuleFor(x => x.PerAccess).NotEmpty().WithMessage("At least one ACCESS PERMISSION must be provided.");
    }
}