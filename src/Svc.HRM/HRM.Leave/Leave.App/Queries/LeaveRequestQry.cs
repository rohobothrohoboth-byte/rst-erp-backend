using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class LeaveRequestAllQry : IRequest<List<LeaveRequestListDto>> { }
public class LeaveRequestByIdQry : IRequest<LeaveRequestListDto?> { public Guid Id { get; set; } }
public class LeaveRequestMyQry : IRequest<List<LeaveRequestListDto>> { public Guid Id { get; set; } }

public class LeaveRequestAllQryHandler : IRequestHandler<LeaveRequestAllQry, List<LeaveRequestListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;

    public LeaveRequestAllQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<List<LeaveRequestListDto>> Handle(LeaveRequestAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeaveRequest>().GetAll();
        var dataL = new List<LeaveRequestListDto>();
        var empL = await _hrmPro.GetListEmp(cancellationToken);
        var lvtL = await _unitOfWork.Repository<LeaveType>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL.Res.FirstOrDefault(t => t.Id == data.EmployeeId.ToString());
            //var app = "";
            //if (data.ApprovedById != null)
            //{
            //    var aEmpId = (Guid)data.ApprovedById;
            //    var aEmp = empL.Res.FirstOrDefault(t => t.Id == aEmpId.ToString());
            //    app = aEmp != null ? aEmp.Name : "NOT AVAILABLE";
            //}

            var lvt = lvtL.FirstOrDefault(t => t.Id == data.LeaveTypeId);
            var c = new LeaveRequestListDto
            {
                Id = data.Id,
                LeaveTypeId = data.LeaveTypeId,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                DateRequested = data.DateAdd,
                DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days",
                IsHalfDayStr = data.IsHalfDay.ToString(),
                StatusStr = ((Status)Enum.Parse(typeof(Status), data.Status)).ToDisplayName(),
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
    private readonly IHrmProfileClient _hrmPro;

    public LeaveRequestByIdQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<LeaveRequestListDto?> Handle(LeaveRequestByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveRequest>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _hrmPro.GetEmp(data.EmployeeId.ToString(), cancellationToken);
        var lvt = await _unitOfWork.Repository<LeaveType>().GetById(data.LeaveTypeId);
        //var app = "";
        //if (data.ApprovedById != null)
        //{
        //    var appEmpId = (Guid)data.ApprovedById;
        //    var appEmp = await _hrmPro.GetEmp(appEmpId.ToString(), cancellationToken);
        //    app = appEmp.Res.Name != null ? appEmp.Res.Name : "NOT AVAILABLE";
        //}

        var c = new LeaveRequestListDto
        {
            Id = data.Id,
            LeaveTypeId = data.LeaveTypeId,
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            DateRequested = data.DateAdd,
            DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days",
            IsHalfDayStr = data.IsHalfDay.ToString(),
            StatusStr = ((Status)Enum.Parse(typeof(Status), data.Status)).ToDisplayName(),
            Employee = emp.Res.Name != null ? emp.Res.Name : "NOT AVAILABLE",
            LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class LeaveRequestMyQryHandler : IRequestHandler<LeaveRequestMyQry, List<LeaveRequestListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;

    public LeaveRequestMyQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<List<LeaveRequestListDto>> Handle(LeaveRequestMyQry request, CancellationToken cancellationToken)
    {
        var dataL = new List<LeaveRequestListDto>();
        var myReq = (await _unitOfWork.Repository<LeaveRequest>().Find(r => r.EmployeeId == request.Id)).ToList();
        if (myReq.Count <= 0) { return dataL; }
        var emp = (await _hrmPro.GetEmp(request.Id.ToString(), cancellationToken)).Res.Name;
        var empL = (await _hrmPro.GetListEmp(cancellationToken));
        var lvtL = await _unitOfWork.Repository<LeaveType>().GetAll();

        foreach (var data in myReq)
        {
            var lvt = lvtL.FirstOrDefault(l => l.Id == data.LeaveTypeId);
            //var app = "";
            //if (data.ApprovedById != null)
            //{
            //    var appEmpId = (Guid)data.ApprovedById;
            //    var appEmp = empL.Res.FirstOrDefault(e => e.Id == appEmpId.ToString());
            //    app = appEmp.Name ?? "NOT AVAILABLE";
            //}

            var c = new LeaveRequestListDto
            {
                Id = data.Id,
                LeaveTypeId = data.LeaveTypeId,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                DateRequested = data.DateAdd,
                DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days",
                IsHalfDayStr = data.IsHalfDay.ToString(),
                StatusStr = ((Status)Enum.Parse(typeof(Status), data.Status)).ToDisplayName(),
                Employee = emp != null ? emp : "NOT AVAILABLE",
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