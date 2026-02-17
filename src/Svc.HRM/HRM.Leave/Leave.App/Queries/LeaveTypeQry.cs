using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveTypeAllQry : IRequest<List<LeaveTypeListDto>> { }
public class LeaveTypeByIdQry : IRequest<LeaveTypeListDto?> { public Guid Id { get; set; } }

public class LeaveTypeAllQryHandler : IRequestHandler<LeaveTypeAllQry, List<LeaveTypeListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveTypeAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork;}

    public async Task<List<LeaveTypeListDto>> Handle(LeaveTypeAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeaveType>().GetAll();
        var dataL = new List<LeaveTypeListDto>();

        foreach (var data in dbData)
        {
            var c = new LeaveTypeListDto
            {
                Id = data.Id,
                Name = data.Name,
                LeaveCategory = data.LeaveCategory,
                RequiresApproval = data.RequiresApproval,
                AllowHalfDay = data.AllowHalfDay,
                HolidaysAsLeave = data.HolidaysAsLeave,
                IsActive = data.IsActive,
                LeaveCategoryStr = ((LeaveCategory)Enum.Parse(typeof(LeaveCategory), data.LeaveCategory)).ToDisplayName(),
                RequiresApprovalStr = BoolToStr.FormatBool(data.RequiresApproval),
                AllowHalfDayStr = BoolToStr.FormatBool(data.AllowHalfDay),
                HolidaysAsLeaveStr = BoolToStr.FormatBool(data.HolidaysAsLeave),
                IsActiveStr = BoolToStr.FormatStat(data.IsActive),
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class LeaveTypeByIdQryHandler : IRequestHandler<LeaveTypeByIdQry, LeaveTypeListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveTypeByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<LeaveTypeListDto?> Handle(LeaveTypeByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveType>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new LeaveTypeListDto
        {
            Id = data.Id,
            Name = data.Name,
            LeaveCategory = data.LeaveCategory,
            RequiresApproval = data.RequiresApproval,
            AllowHalfDay = data.AllowHalfDay,
            HolidaysAsLeave = data.HolidaysAsLeave,
            IsActive = data.IsActive,
            LeaveCategoryStr = ((LeaveCategory)Enum.Parse(typeof(LeaveCategory), data.LeaveCategory)).ToDisplayName(),
            RequiresApprovalStr = BoolToStr.FormatBool(data.RequiresApproval),
            AllowHalfDayStr = BoolToStr.FormatBool(data.AllowHalfDay),
            HolidaysAsLeaveStr = BoolToStr.FormatBool(data.HolidaysAsLeave),
            IsActiveStr = BoolToStr.FormatStat(data.IsActive),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}