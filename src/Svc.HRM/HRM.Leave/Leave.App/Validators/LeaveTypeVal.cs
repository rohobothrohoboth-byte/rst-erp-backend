using FluentValidation;
using Leave.App.Commands;
using Leave.App.Interfaces;
using Leave.Domain.Entities;

namespace Leave.App.Validators;

public class LeaveTypeVal : AbstractValidator<LeaveTypeAddCmd>
{
    public LeaveTypeVal(IUnitOfWork _unitOfWork)
    {
        RuleFor(x => x.AddDto.Name)
            .NotEmpty()
            .MustAsync(async (name, ct) => await _unitOfWork.Repository<LeaveType>().GetFoD(l => l.Name == name) == null)
            .WithMessage("Leave Type already exists");
    }
}