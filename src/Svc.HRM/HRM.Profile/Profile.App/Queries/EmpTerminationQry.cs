using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;

namespace Profile.App.Queries;

public class EmpTerminationAllQry : IRequest<List<EmpTerminationListDto>> { }
public class EmpTerminationByIdQry : IRequest<EmpTerminationListDto?> { public Guid Id { get; set; } }
public class EmpTerminationByEmployeeQry : IRequest<List<EmpTerminationListDto>> { public Guid EmployeeId { get; set; } }
public class EmpOffboardingByTerminationQry : IRequest<List<EmpOffboardingTaskDto>> { public Guid TerminationId { get; set; } }

public class EmpTerminationAllHandler(IDapperHelper dapper) : IRequestHandler<EmpTerminationAllQry, List<EmpTerminationListDto>>
{
    public async Task<List<EmpTerminationListDto>> Handle(EmpTerminationAllQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT t."Id", t."Status", t."TerminationType", t."LastWorkingDate", t."NoticeDate", t."Reason", t."Comments",
                   t."ExitInterviewNotes", t."ApprovedById", t."ApprovedDate", t."AppliedDate",
                   t."RequestFinalPay", t."RequestLeaveSettlement", t."SettlementPayrollRunId", t."SettlementStatus",
                   t."SettlementNotes", t."LeaveUnpaidDaysSnapshot", t."EmployeeId", t.xmin,
                   COALESCE(c.total, 0) AS "OffboardingTotal",
                   COALESCE(c.done, 0) AS "OffboardingCompleted"
            FROM "EmpTermination" t
            LEFT JOIN (
                SELECT "TerminationId",
                       COUNT(*)::int AS total,
                       COUNT(*) FILTER (WHERE "Status" IN ('Completed','Skipped'))::int AS done
                FROM "EmpOffboardingTask"
                WHERE "IsDeleted" = false
                GROUP BY "TerminationId"
            ) c ON c."TerminationId" = t."Id"
            WHERE t."IsDeleted" = false
            ORDER BY t."DateAdd" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpTerminationRaw>(sql, null, ct)).ToList();
        return rows.Select(Map).ToList();
    }

    internal static EmpTerminationListDto Map(EmpTerminationRaw r) => new()
    {
        Id = r.Id,
        Status = r.Status,
        StatusName = MyEnumHelper.FormatEnum<HrChangeStatus>(r.Status),
        TerminationType = r.TerminationType,
        LastWorkingDate = r.LastWorkingDate,
        NoticeDate = r.NoticeDate,
        Reason = r.Reason,
        Comments = r.Comments,
        ExitInterviewNotes = r.ExitInterviewNotes,
        ApprovedById = r.ApprovedById,
        ApprovedDate = r.ApprovedDate,
        AppliedDate = r.AppliedDate,
        RequestFinalPay = r.RequestFinalPay,
        RequestLeaveSettlement = r.RequestLeaveSettlement,
        SettlementPayrollRunId = r.SettlementPayrollRunId,
        SettlementStatus = r.SettlementStatus,
        SettlementNotes = r.SettlementNotes,
        LeaveUnpaidDaysSnapshot = r.LeaveUnpaidDaysSnapshot,
        EmployeeId = r.EmployeeId,
        OffboardingTotal = r.OffboardingTotal,
        OffboardingCompleted = r.OffboardingCompleted,
        RowVersion = r.xmin.ToString()
    };
}

public class EmpTerminationByIdHandler(IDapperHelper dapper) : IRequestHandler<EmpTerminationByIdQry, EmpTerminationListDto?>
{
    public async Task<EmpTerminationListDto?> Handle(EmpTerminationByIdQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT t."Id", t."Status", t."TerminationType", t."LastWorkingDate", t."NoticeDate", t."Reason", t."Comments",
                   t."ExitInterviewNotes", t."ApprovedById", t."ApprovedDate", t."AppliedDate",
                   t."RequestFinalPay", t."RequestLeaveSettlement", t."SettlementPayrollRunId", t."SettlementStatus",
                   t."SettlementNotes", t."LeaveUnpaidDaysSnapshot", t."EmployeeId", t.xmin,
                   COALESCE(c.total, 0) AS "OffboardingTotal",
                   COALESCE(c.done, 0) AS "OffboardingCompleted"
            FROM "EmpTermination" t
            LEFT JOIN (
                SELECT "TerminationId",
                       COUNT(*)::int AS total,
                       COUNT(*) FILTER (WHERE "Status" IN ('Completed','Skipped'))::int AS done
                FROM "EmpOffboardingTask"
                WHERE "IsDeleted" = false
                GROUP BY "TerminationId"
            ) c ON c."TerminationId" = t."Id"
            WHERE t."Id" = @Id AND t."IsDeleted" = false
            LIMIT 1
            """;
        var row = await dapper.QueryFirstOrDefaultAsync<EmpTerminationRaw>(sql, new { request.Id }, ct);
        if (row == null) return null;

        var dto = EmpTerminationAllHandler.Map(row);
        var tasks = (await dapper.QueryAsync<EmpOffboardingTaskRaw>("""
            SELECT "Id", "TerminationId", "Category", "Title", "Status", "AssignedToId", "DueDate", "CompletedAt",
                   "Notes", "SortOrder", xmin
            FROM "EmpOffboardingTask"
            WHERE "TerminationId" = @Id AND "IsDeleted" = false
            ORDER BY "SortOrder", "DateAdd"
            """, new { request.Id }, ct)).ToList();
        dto.Tasks = tasks.Select(MapTask).ToList();
        return dto;
    }

    internal static EmpOffboardingTaskDto MapTask(EmpOffboardingTaskRaw x) => new()
    {
        Id = x.Id,
        TerminationId = x.TerminationId,
        Category = x.Category,
        Title = x.Title,
        Status = x.Status,
        AssignedToId = x.AssignedToId,
        DueDate = x.DueDate,
        CompletedAt = x.CompletedAt,
        Notes = x.Notes,
        SortOrder = x.SortOrder,
        RowVersion = x.xmin.ToString()
    };
}

public class EmpTerminationByEmployeeHandler(IDapperHelper dapper) : IRequestHandler<EmpTerminationByEmployeeQry, List<EmpTerminationListDto>>
{
    public async Task<List<EmpTerminationListDto>> Handle(EmpTerminationByEmployeeQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT t."Id", t."Status", t."TerminationType", t."LastWorkingDate", t."NoticeDate", t."Reason", t."Comments",
                   t."ExitInterviewNotes", t."ApprovedById", t."ApprovedDate", t."AppliedDate",
                   t."RequestFinalPay", t."RequestLeaveSettlement", t."SettlementPayrollRunId", t."SettlementStatus",
                   t."SettlementNotes", t."LeaveUnpaidDaysSnapshot", t."EmployeeId", t.xmin,
                   COALESCE(c.total, 0) AS "OffboardingTotal",
                   COALESCE(c.done, 0) AS "OffboardingCompleted"
            FROM "EmpTermination" t
            LEFT JOIN (
                SELECT "TerminationId",
                       COUNT(*)::int AS total,
                       COUNT(*) FILTER (WHERE "Status" IN ('Completed','Skipped'))::int AS done
                FROM "EmpOffboardingTask"
                WHERE "IsDeleted" = false
                GROUP BY "TerminationId"
            ) c ON c."TerminationId" = t."Id"
            WHERE t."EmployeeId" = @EmployeeId AND t."IsDeleted" = false
            ORDER BY t."LastWorkingDate" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpTerminationRaw>(sql, new { request.EmployeeId }, ct)).ToList();
        return rows.Select(EmpTerminationAllHandler.Map).ToList();
    }
}

public class EmpOffboardingByTerminationHandler(IDapperHelper dapper) : IRequestHandler<EmpOffboardingByTerminationQry, List<EmpOffboardingTaskDto>>
{
    public async Task<List<EmpOffboardingTaskDto>> Handle(EmpOffboardingByTerminationQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "TerminationId", "Category", "Title", "Status", "AssignedToId", "DueDate", "CompletedAt",
                   "Notes", "SortOrder", xmin
            FROM "EmpOffboardingTask"
            WHERE "TerminationId" = @TerminationId AND "IsDeleted" = false
            ORDER BY "SortOrder", "DateAdd"
            """;
        var rows = (await dapper.QueryAsync<EmpOffboardingTaskRaw>(sql, new { request.TerminationId }, ct)).ToList();
        return rows.Select(EmpTerminationByIdHandler.MapTask).ToList();
    }
}

public class EmpTerminationRaw
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public string TerminationType { get; set; } = default!;
    public DateTime LastWorkingDate { get; set; }
    public DateTime? NoticeDate { get; set; }
    public string Reason { get; set; } = default!;
    public string? Comments { get; set; }
    public string? ExitInterviewNotes { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? AppliedDate { get; set; }
    public bool RequestFinalPay { get; set; }
    public bool RequestLeaveSettlement { get; set; }
    public Guid? SettlementPayrollRunId { get; set; }
    public string? SettlementStatus { get; set; }
    public string? SettlementNotes { get; set; }
    public decimal? LeaveUnpaidDaysSnapshot { get; set; }
    public Guid EmployeeId { get; set; }
    public int OffboardingTotal { get; set; }
    public int OffboardingCompleted { get; set; }
    public uint xmin { get; set; }
}

public class EmpOffboardingTaskRaw
{
    public Guid Id { get; set; }
    public Guid TerminationId { get; set; }
    public string Category { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public Guid? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public int SortOrder { get; set; }
    public uint xmin { get; set; }
}
