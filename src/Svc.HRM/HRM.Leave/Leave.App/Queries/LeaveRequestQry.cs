using Common;
using Dapper;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveRequestAllQry : IRequest<List<LeaveRequestListDto>> { }
public class LeaveRequestByIdQry : IRequest<LeaveRequestListDto?> { public Guid Id { get; set; } }
public class LeaveRequestMyQry : IRequest<List<LeaveRequestListDto>> { public Guid Id { get; set; } }



public class LeaveRequestAllHandler : IRequestHandler<LeaveRequestAllQry, List<LeaveRequestListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    public LeaveRequestAllHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
    }

    public async Task<List<LeaveRequestListDto>> Handle(LeaveRequestAllQry request, CancellationToken ct)
    {
        var empTask = await _hrmProfile.GetListEmp(ct);
        var empDict = empTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.Status, x => x.EmployeeId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeaveType, LeaveRequestListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var result = new List<LeaveRequestListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<LeaveRequestListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            //var emp = "Not Assigned";
            //if (data.EmployeeId != null)
            //{
            //    empDict.TryGetValue((Guid)data.EmployeeId, out var empN);
            //    emp = empN?.Name ?? "";
            //}
            empDict.TryGetValue(data.EmployeeId, out var emp);
            result.Add(new LeaveRequestListDto
            {
                Id = data.Id,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                DateRequested = data.DateAdd,
                DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days",
                IsHalfDayStr = BoolToStr.FormatBool(data.IsHalfDay),
                StatusStr = MyEnumHelper.FormatEnum<Status>(data.Status),
                Employee = emp?.Name ?? "",
                LeaveType = data.LeaveType,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }

        return result;
    }
}

public class LeaveRequestByIdHandler : IRequestHandler<LeaveRequestByIdQry, LeaveRequestListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    public LeaveRequestByIdHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
    }

    public async Task<LeaveRequestListDto?> Handle(LeaveRequestByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.Status, x => x.EmployeeId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeaveType, LeaveRequestListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveRequestListDto>(sql, parameters, ct);
        if (data == null) return null;

        var emp = await _hrmProfile.GetEmp((data.EmployeeId).ToString(), ct);

        data.DateRequested = data.DateAdd;
        data.DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days";
        data.IsHalfDayStr = BoolToStr.FormatBool(data.IsHalfDay);
        data.StatusStr = MyEnumHelper.FormatEnum<Status>(data.Status);
        data.Employee = emp.Res.Name ?? "";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class LeaveRequestMyHandler : IRequestHandler<LeaveRequestMyQry, List<LeaveRequestListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;
    public LeaveRequestMyHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
    }

    public async Task<List<LeaveRequestListDto>> Handle(LeaveRequestMyQry request, CancellationToken ct)
    {
        var emp = (await _hrmProfile.GetEmp(request.Id.ToString(), ct));

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.Status, x => x.EmployeeId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeaveType, LeaveRequestListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.EmployeeId == request.Id);

        var (sql, parameters) = qb.Build();
        var result = new List<LeaveRequestListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<LeaveRequestListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            result.Add(new LeaveRequestListDto
            {
                Id = data.Id,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                DateRequested = data.DateAdd,
                DaysRequestedStr = $"{data.DaysRequested:#,##0.##} days",
                IsHalfDayStr = BoolToStr.FormatBool(data.IsHalfDay),
                StatusStr = MyEnumHelper.FormatEnum<Status>(data.Status),
                Employee = emp.Res?.Name ?? "",
                LeaveType = data.LeaveType,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }

        return result;
    }
}