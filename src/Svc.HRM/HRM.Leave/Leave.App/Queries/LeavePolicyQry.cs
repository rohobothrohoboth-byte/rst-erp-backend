using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeavePolicyAllQry : IRequest<List<LeavePolicyListDto>> { }
public class LeavePolicyByIdQry : IRequest<LeavePolicyListDto?> { public Guid Id { get; set; } }
public class ActiveLeavePolicyQry : IRequest<List<LeavePolicyListDto>> { }



public class LeavePolicyAllHandler : IRequestHandler<LeavePolicyAllQry, List<LeavePolicyListDto>>
{
    private readonly IDapperHelper _dapper;
    public LeavePolicyAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeavePolicyListDto>> Handle(LeavePolicyAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicy>(v, x => x.Id, x => x.Name, x => x.Code, x => x.AllowEncashment, x => x.RequiresAttachment, x => x.Status, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeaveType, LeavePolicyListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeavePolicy>(v)
            .Join<LeavePolicy, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .OrderBy<LeavePolicy>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeavePolicyListDto>(ct);

        foreach (var data in list)
        {
            data.StatusStr = MyEnumHelper.FormatEnum<PolicyStatus>(data.Status);
            data.AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment);
            data.RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class LeavePolicyByIdHandler : IRequestHandler<LeavePolicyByIdQry, LeavePolicyListDto?>
{
    private readonly IDapperHelper _dapper;
    public LeavePolicyByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeavePolicyListDto?> Handle(LeavePolicyByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicy>(v, x => x.Id, x => x.Name, x => x.Code, x => x.AllowEncashment, x => x.RequiresAttachment, x => x.Status, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeaveType, LeavePolicyListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeavePolicy>(v)
            .Join<LeavePolicy, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeavePolicy>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeavePolicyListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.StatusStr = MyEnumHelper.FormatEnum<PolicyStatus>(data.Status);
        data.AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment);
        data.RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActiveLeavePolicyHandler : IRequestHandler<ActiveLeavePolicyQry, List<LeavePolicyListDto>>
{
    private readonly IDapperHelper _dapper;
    public ActiveLeavePolicyHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeavePolicyListDto>> Handle(ActiveLeavePolicyQry request, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicy>(v, x => x.Id, x => x.Name, x => x.Code, x => x.AllowEncashment, x => x.RequiresAttachment, x => x.Status, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<LeaveType, LeavePolicyListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeavePolicy>(v)
            .Join<LeavePolicy, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeavePolicy>(v, x => x.Status == stat)
            .OrderBy<LeavePolicy>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeavePolicyListDto>(ct);

        foreach (var data in list)
        {
            data.StatusStr = MyEnumHelper.FormatEnum<PolicyStatus>(data.Status);
            data.AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment);
            data.RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}