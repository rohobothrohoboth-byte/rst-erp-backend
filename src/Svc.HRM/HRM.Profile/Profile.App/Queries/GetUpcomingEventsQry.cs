// Profile.App/Queries/GetUpcomingEventsQry.cs

using Dapper;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;

namespace Profile.App.Queries;

public class GetUpcomingEventsQry : IRequest<List<EventDto>> { }

public class GetUpcomingEventsHandler : IRequestHandler<GetUpcomingEventsQry, List<EventDto>>
{
    private readonly IDapperHelper _dapper;

    public GetUpcomingEventsHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<EventDto>> Handle(GetUpcomingEventsQry request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var allEvents = new List<EventDto>();

        // ✅ Query 1: Birthdays from EmpBio
        const string birthdaysSql = @"
            SELECT
                e.""Id"" as EventId,
                p.""FirstName"" || ' ' || p.""LastName"" as EventTitle,
                eb.""BirthDate"" as EventDate,
                'Birthday' as EventType,
                p.""FirstName"" || ' ' || p.""LastName"" as Description
            FROM ""Employee"" e
            INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
            INNER JOIN ""EmpBio"" eb ON e.""Id"" = eb.""EmployeeId""
            WHERE e.""IsDeleted"" = false
                AND eb.""IsDeleted"" = false
                AND eb.""BirthDate"" IS NOT NULL
                AND EXTRACT(MONTH FROM eb.""BirthDate"") = EXTRACT(MONTH FROM CURRENT_DATE)
            ORDER BY EXTRACT(DAY FROM eb.""BirthDate"")
            LIMIT 10";

        var birthdays = await _dapper.QueryAsync<EventDto>(birthdaysSql, null, ct);
        allEvents.AddRange(birthdays);

        // ✅ Query 2: Work Anniversaries from Employee
        const string anniversariesSql = @"
            SELECT
                e.""Id"" as EventId,
                p.""FirstName"" || ' ' || p.""LastName"" || ' - Work Anniversary' as EventTitle,
                e.""EmploymentDate"" as EventDate,
                'Anniversary' as EventType,
                'Work Anniversary' as Description
            FROM ""Employee"" e
            INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
            WHERE e.""IsDeleted"" = false
                AND e.""EmploymentDate"" IS NOT NULL
                AND EXTRACT(MONTH FROM e.""EmploymentDate"") = EXTRACT(MONTH FROM CURRENT_DATE)
            ORDER BY EXTRACT(DAY FROM e.""EmploymentDate"")
            LIMIT 10";

        var anniversaries = await _dapper.QueryAsync<EventDto>(anniversariesSql, null, ct);
        allEvents.AddRange(anniversaries);

        return allEvents.OrderBy(e => e.EventDate).ToList();
    }
}