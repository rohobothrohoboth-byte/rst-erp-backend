using Leave.App.Interfaces;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class LeaveRequestAllQry : IRequest<List<LeaveRequestListDto>> { }
public class LeaveRequestByIdQry : IRequest<LeaveRequestListDto?> { public Guid Id { get; set; } }

public class LeaveRequestAllQryHandler : IRequestHandler<LeaveRequestAllQry, List<LeaveRequestListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfile _hrmPro;

    public LeaveRequestAllQryHandler(IUnitOfWork unitOfWork, IHrmProfile hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<List<LeaveRequestListDto>> Handle(LeaveRequestAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeaveRequest>().GetAll();
        var dataL = new List<LeaveRequestListDto>();
        var empL = await _hrmPro.EmpList(cancellationToken);
        var lvtL = await _unitOfWork.Repository<LeaveType>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL!.FirstOrDefault(t => t.Id == data.EmployeeId);
            var app = "";
            if (data.ApprovedById != null)
            {
                var aEmp = empL!.FirstOrDefault(t => t.Id == (Guid)data.ApprovedById);
                app = aEmp != null ? aEmp.Name : "NOT AVAILABLE";
            }

            var lvt = lvtL.FirstOrDefault(t => t.Id == data.LeaveTypeId);
            var c = new LeaveRequestListDto
            {
                Id = data.Id,
                ApprovedById = data.ApprovedById,
                EmployeeId = data.EmployeeId,
                LeaveTypeId = data.LeaveTypeId,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                DateRequested = data.DateAdd,
                DateApproved = data.DateApproved,
                Comments = data.Comments,
                DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days",
                IsHalfDayStr = data.IsHalfDay.ToString(),
                StatusStr = ((LeaveRequestStatus)Enum.Parse(typeof(LeaveRequestStatus), data.Status)).ToDisplayName(),
                ApprovedBy = app,
                Employee = emp != null ? emp.Name : "NOT AVAILABLE",
                LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
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

public class LeaveRequestByIdQryHandler : IRequestHandler<LeaveRequestByIdQry, LeaveRequestListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfile _hrmPro;

    public LeaveRequestByIdQryHandler(IUnitOfWork unitOfWork, IHrmProfile hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<LeaveRequestListDto?> Handle(LeaveRequestByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveRequest>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _hrmPro.Emp(data.EmployeeId, cancellationToken);
        var lvt = await _unitOfWork.Repository<LeaveType>().GetById(data.LeaveTypeId);
        var app = "";
        if (data.ApprovedById != null)
        {
            var appEmp = await _hrmPro.Emp((Guid)data.ApprovedById, cancellationToken);
            app = appEmp != null ? appEmp.Name : "NOT AVAILABLE";
        }

        var c = new LeaveRequestListDto
        {
            Id = data.Id,
            ApprovedById = data.ApprovedById,
            EmployeeId = data.EmployeeId,
            LeaveTypeId = data.LeaveTypeId,
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            DateRequested = data.DateAdd,
            DateApproved = data.DateApproved,
            Comments = data.Comments,
            DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days",
            IsHalfDayStr = data.IsHalfDay.ToString(),
            StatusStr = ((LeaveRequestStatus)Enum.Parse(typeof(LeaveRequestStatus), data.Status)).ToDisplayName(),
            ApprovedBy = app,
            Employee = emp != null ? emp.Name : "NOT AVAILABLE",
            LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}