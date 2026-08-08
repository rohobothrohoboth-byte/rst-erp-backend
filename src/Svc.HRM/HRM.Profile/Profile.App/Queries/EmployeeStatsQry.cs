using MediatR;
using Dapper;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmployeeStatsQry : IRequest<EmployeeStatsDto>
{
}

public class EmployeeStatsDto
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int OnLeave { get; set; }
    public int Pending { get; set; }
    public int Terminated { get; set; }
    public int Retired { get; set; }
}

public class EmployeeStatsQryHandler : IRequestHandler<EmployeeStatsQry, EmployeeStatsDto>
{
    private readonly IDapperHelper _dapper;

    public EmployeeStatsQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<EmployeeStatsDto> Handle(EmployeeStatsQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                COUNT(*) as Total,
                SUM(CASE WHEN ""EmpState"" = 'Active' THEN 1 ELSE 0 END) as Active,
                SUM(CASE WHEN ""EmpState"" = 'On Leave' THEN 1 ELSE 0 END) as OnLeave,
                SUM(CASE WHEN ""EmpState"" = 'Pending' THEN 1 ELSE 0 END) as Pending,
                SUM(CASE WHEN ""EmpState"" = 'Terminated' THEN 1 ELSE 0 END) as Terminated,
                SUM(CASE WHEN ""EmpState"" = 'Retired' THEN 1 ELSE 0 END) as Retired
            FROM ""Employee""
            WHERE ""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<EmployeeStatsDto>(sql, null, ct);
        return result ?? new EmployeeStatsDto();
    }
}