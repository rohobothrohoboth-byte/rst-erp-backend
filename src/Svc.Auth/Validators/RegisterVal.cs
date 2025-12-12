using FluentValidation;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Validators;

public class RegisterStp1Val : AbstractValidator<RegisterStep1>
{
    public RegisterStp1Val()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee Id is not found.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("User ROLE should be selected.");
        RuleFor(x => x.PerModules).NotEmpty().WithMessage("A minimum of 1 MODULE should be selected.");
    }
}


public class RegisterStp2Val : AbstractValidator<RegisterStep2>
{
    public RegisterStp2Val()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee Id is not found.");
        RuleFor(x => x.PerMenus).NotEmpty().WithMessage("A minimum of 1 MENU PERMISSION should be selected.");
    }
}

public class RegisterStp3Val : AbstractValidator<RegisterStep3>
{
    public RegisterStp3Val()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee Id is not found.");
        RuleFor(x => x.PerAccess).NotEmpty().WithMessage("A minimum of 1 ACCESS PERMISSION should be selected.");
    }
}