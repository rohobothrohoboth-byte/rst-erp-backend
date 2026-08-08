using FluentValidation;
using Leave.App.Commands;
using Leave.App.Interfaces;
using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Validators;

//public class LeaveTypeVal : AbstractValidator<LeaveTypeAddCmd>
//{
//    public LeaveTypeVal(IUnitOfWork _unitOfWork)
//    {
//        RuleFor(x => x.AddDto.Name)
//            .NotEmpty()
//            .MustAsync(async (name) => await _unitOfWork.Set<LeaveType>().FirstOrDefaultAsync(l => l.Name == name) == null)
//            .WithMessage("Leave Type already exists");
//    }
//}

public class LeaveTypeVal : AbstractValidator<LeaveTypeAddCmd>
{
    public LeaveTypeVal(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.AddDto.Name)
            .NotEmpty()
            .MustAsync(async (name, ct) => !await unitOfWork.Set<LeaveType>().AnyAsync(x => x.Name == name, ct))
            .WithMessage("Leave Type already exists");
    }
}