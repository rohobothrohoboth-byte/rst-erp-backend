using Leave.Domain.DTOs;
using MediatR;
using Leave.App.Interfaces;

namespace Leave.App.Queries;

public class ApprovedLeavesInRangeQry : IRequest<List<ApprovedLeaveRangeDto>>
{
    public Guid? EmployeeId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class UnpaidLeaveDaysQry : IRequest<UnpaidLeaveDaysDto>
{
    public Guid EmployeeId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class ApprovedLeavesInRangeHandler : IRequestHandler<ApprovedLeavesInRangeQry, List<ApprovedLeaveRangeDto>>
{
    private readonly IDapperHelper _dapper;

    public ApprovedLeavesInRangeHandler(IDapperHelper dapper) => _dapper = dapper;

    public async Task<List<ApprovedLeaveRangeDto>> Handle(ApprovedLeavesInRangeQry request, CancellationToken ct)
    {
        var from = request.From.Date;
        var to = request.To.Date;

        const string sql = """
            SELECT
                lr."Id" AS "RequestId",
                lr."EmployeeId",
                lr."LeaveTypeId",
                COALESCE(lt."Name", '') AS "LeaveTypeName",
                COALESCE(lt."LeaveCategory", 'Paid') AS "LeaveCategory",
                lr."StartDate",
                lr."EndDate",
                lr."DaysRequested",
                lr."IsHalfDay",
                lr."Status"
            FROM "LeaveRequest" lr
            LEFT JOIN "LeaveType" lt ON lt."Id" = lr."LeaveTypeId" AND lt."IsDeleted" = false
            WHERE lr."IsDeleted" = false
              AND (lr."Status" = 'Approved' OR lr."Status" = '1')
              AND lr."StartDate"::date <= @To::date
              AND lr."EndDate"::date >= @From::date
              AND (@EmployeeId IS NULL OR lr."EmployeeId" = @EmployeeId)
            ORDER BY lr."StartDate"
            """;

        var rows = await _dapper.QueryAsync<ApprovedLeaveRangeDto>(sql, new
        {
            From = from,
            To = to,
            EmployeeId = request.EmployeeId
        }, ct);

        return rows.ToList();
    }
}

public class UnpaidLeaveDaysHandler : IRequestHandler<UnpaidLeaveDaysQry, UnpaidLeaveDaysDto>
{
    private readonly IMediator _med;

    public UnpaidLeaveDaysHandler(IMediator med) => _med = med;

    public async Task<UnpaidLeaveDaysDto> Handle(UnpaidLeaveDaysQry request, CancellationToken ct)
    {
        var items = await _med.Send(new ApprovedLeavesInRangeQry
        {
            EmployeeId = request.EmployeeId,
            From = request.From,
            To = request.To
        }, ct);

        var unpaid = items
            .Where(x => string.Equals(x.LeaveCategory, "Unpaid", StringComparison.OrdinalIgnoreCase))
            .ToList();

        double days = 0;
        foreach (var item in unpaid)
        {
            var start = item.StartDate.Date < request.From.Date ? request.From.Date : item.StartDate.Date;
            var end = item.EndDate.Date > request.To.Date ? request.To.Date : item.EndDate.Date;
            if (end < start) continue;

            var span = (end - start).TotalDays + 1;
            if (item.IsHalfDay && span <= 1)
                days += 0.5;
            else
                days += span;
        }

        return new UnpaidLeaveDaysDto
        {
            EmployeeId = request.EmployeeId,
            From = request.From,
            To = request.To,
            UnpaidDays = days,
            Items = unpaid
        };
    }
}
