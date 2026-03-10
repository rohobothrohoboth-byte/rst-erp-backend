using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveTypeNameAllQry : IRequest<List<NameList>> { }
public class LeaveTypeNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class LeaveTypeNameAllHandler : IRequestHandler<LeaveTypeNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(LeaveTypeNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<LeaveType>(v, x => x.Id, x => x.Name).From<LeaveType>(v);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<NameList>(ct);
        return list;
    }
}

public class LeaveTypeNameByIdHandler : IRequestHandler<LeaveTypeNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(LeaveTypeNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<LeaveType, NameList>(v).From<LeaveType>(v).Where<LeaveType>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.FirstOrDefaultAsync<NameList>(ct);
        return list;
    }
}

