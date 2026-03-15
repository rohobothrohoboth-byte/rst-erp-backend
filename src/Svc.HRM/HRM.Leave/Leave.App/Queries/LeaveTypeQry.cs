using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveTypeAllQry : IRequest<List<LeaveTypeListDto>> { }
public class LeaveTypeByIdQry : IRequest<LeaveTypeListDto?> { public Guid Id { get; set; } }



public class LeaveTypeAllHandler : IRequestHandler<LeaveTypeAllQry, List<LeaveTypeListDto>>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeaveTypeListDto>> Handle(LeaveTypeAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<LeaveType>(v, x => x.Id, x => x.Name, x => x.LeaveCategory, x => x.RequiresApproval, x => x.AllowHalfDay, x => x.HolidaysAsLeave, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<LeaveType>(v)
            .OrderBy<LeaveType>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeaveTypeListDto>(ct);

        foreach (var data in list)
        {
            data.LeaveCategoryStr = MyEnumHelper.FormatEnum<LeaveCategory>(data.LeaveCategory);
            data.RequiresApprovalStr = BoolToStr.FormatBool(data.RequiresApproval);
            data.AllowHalfDayStr = BoolToStr.FormatBool(data.AllowHalfDay);
            data.HolidaysAsLeaveStr = BoolToStr.FormatBool(data.HolidaysAsLeave);
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class LeaveTypeByIdHandler : IRequestHandler<LeaveTypeByIdQry, LeaveTypeListDto?>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeaveTypeListDto?> Handle(LeaveTypeByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<LeaveType>(v, x => x.Id, x => x.Name, x => x.LeaveCategory, x => x.RequiresApproval, x => x.AllowHalfDay, x => x.HolidaysAsLeave, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<LeaveType>(v)
            .OrderBy<LeaveType>(v, x => x.DateAdd, desc: true)
            .Where<LeaveType>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveTypeListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.LeaveCategoryStr = MyEnumHelper.FormatEnum<LeaveCategory>(data.LeaveCategory);
        data.RequiresApprovalStr = BoolToStr.FormatBool(data.RequiresApproval);
        data.AllowHalfDayStr = BoolToStr.FormatBool(data.AllowHalfDay);
        data.HolidaysAsLeaveStr = BoolToStr.FormatBool(data.HolidaysAsLeave);
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}