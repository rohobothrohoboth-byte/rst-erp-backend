using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;

namespace Recruit.App.Queries;

public class OnboardingAssignAllQry : IRequest<List<OnboardingAssignListDto>> { }

public class OnboardingAssignByIdQry : IRequest<OnboardingAssignListDto?>
{
    public Guid Id { get; set; }
}

public class OnboardingAssignByEmployeeQry : IRequest<List<OnboardingAssignListDto>>
{
    public Guid EmployeeId { get; set; }
}

public class OnboardingAssignAllHandler : IRequestHandler<OnboardingAssignAllQry, List<OnboardingAssignListDto>>
{
    private readonly IDapperHelper _dapper;

    public OnboardingAssignAllHandler(IDapperHelper dapper) => _dapper = dapper;

    public async Task<List<OnboardingAssignListDto>> Handle(OnboardingAssignAllQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT a."Id", a."IsMandatory", a."Status", a."ScheduledDate", a."CompletedDate",
                   a."VerifyById", a."EmployeeId", a."OnboardingTaskId",
                   a."DateAdd", a."DateMod", a.xmin,
                   t."TaskName", t."SequenceOrder"
            FROM "OnboardingAssign" a
            INNER JOIN "OnboardingTask" t ON t."Id" = a."OnboardingTaskId" AND t."IsDeleted" = false
            WHERE a."IsDeleted" = false
            ORDER BY a."EmployeeId", t."SequenceOrder"
            """;

        var rows = (await _dapper.QueryAsync<OnboardingAssignRawDto>(sql, null, ct)).ToList();
        return rows.Select(Map).ToList();
    }

    internal static OnboardingAssignListDto Map(OnboardingAssignRawDto raw) => new()
    {
        Id = raw.Id,
        IsMandatory = raw.IsMandatory,
        Status = raw.Status,
        StatusName = MyEnumHelper.FormatEnum<OnboardingStatus>(raw.Status),
        ScheduledDate = raw.ScheduledDate,
        ScheduledDateAm = raw.ScheduledDate.ToString("dd/MM/yyyy"),
        CompletedDate = raw.CompletedDate,
        CompletedDateAm = raw.CompletedDate?.ToString("dd/MM/yyyy"),
        VerifyById = raw.VerifyById,
        EmployeeId = raw.EmployeeId,
        OnboardingTaskId = raw.OnboardingTaskId,
        TaskName = raw.TaskName,
        SequenceOrder = raw.SequenceOrder,
        DateAdd = raw.DateAdd,
        DateAddAm = raw.DateAdd.ToString("dd/MM/yyyy HH:mm"),
        DateMod = raw.DateMod,
        DateModAm = raw.DateMod?.ToString("dd/MM/yyyy HH:mm"),
        RowVersion = raw.xmin.ToString()
    };
}

public class OnboardingAssignByIdHandler : IRequestHandler<OnboardingAssignByIdQry, OnboardingAssignListDto?>
{
    private readonly IDapperHelper _dapper;

    public OnboardingAssignByIdHandler(IDapperHelper dapper) => _dapper = dapper;

    public async Task<OnboardingAssignListDto?> Handle(OnboardingAssignByIdQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT a."Id", a."IsMandatory", a."Status", a."ScheduledDate", a."CompletedDate",
                   a."VerifyById", a."EmployeeId", a."OnboardingTaskId",
                   a."DateAdd", a."DateMod", a.xmin,
                   t."TaskName", t."SequenceOrder"
            FROM "OnboardingAssign" a
            INNER JOIN "OnboardingTask" t ON t."Id" = a."OnboardingTaskId" AND t."IsDeleted" = false
            WHERE a."Id" = @Id AND a."IsDeleted" = false
            LIMIT 1
            """;

        var raw = await _dapper.QueryFirstOrDefaultAsync<OnboardingAssignRawDto>(sql, new { request.Id }, ct);
        return raw == null ? null : OnboardingAssignAllHandler.Map(raw);
    }
}

public class OnboardingAssignByEmployeeHandler : IRequestHandler<OnboardingAssignByEmployeeQry, List<OnboardingAssignListDto>>
{
    private readonly IDapperHelper _dapper;

    public OnboardingAssignByEmployeeHandler(IDapperHelper dapper) => _dapper = dapper;

    public async Task<List<OnboardingAssignListDto>> Handle(OnboardingAssignByEmployeeQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT a."Id", a."IsMandatory", a."Status", a."ScheduledDate", a."CompletedDate",
                   a."VerifyById", a."EmployeeId", a."OnboardingTaskId",
                   a."DateAdd", a."DateMod", a.xmin,
                   t."TaskName", t."SequenceOrder"
            FROM "OnboardingAssign" a
            INNER JOIN "OnboardingTask" t ON t."Id" = a."OnboardingTaskId" AND t."IsDeleted" = false
            WHERE a."EmployeeId" = @EmployeeId AND a."IsDeleted" = false
            ORDER BY t."SequenceOrder"
            """;

        var rows = (await _dapper.QueryAsync<OnboardingAssignRawDto>(sql, new { request.EmployeeId }, ct)).ToList();
        return rows.Select(OnboardingAssignAllHandler.Map).ToList();
    }
}
