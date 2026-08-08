using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;

namespace Profile.App.Queries;

public class EmpContractAllQry : IRequest<List<EmpContractListDto>> { }
public class EmpContractByIdQry : IRequest<EmpContractListDto?> { public Guid Id { get; set; } }
public class EmpContractByEmployeeQry : IRequest<List<EmpContractListDto>> { public Guid EmployeeId { get; set; } }

public class EmpPromotionAllQry : IRequest<List<EmpPromotionListDto>> { }
public class EmpPromotionByIdQry : IRequest<EmpPromotionListDto?> { public Guid Id { get; set; } }
public class EmpPromotionByEmployeeQry : IRequest<List<EmpPromotionListDto>> { public Guid EmployeeId { get; set; } }

public class EmpTransferAllQry : IRequest<List<EmpTransferListDto>> { }
public class EmpTransferByIdQry : IRequest<EmpTransferListDto?> { public Guid Id { get; set; } }
public class EmpTransferByEmployeeQry : IRequest<List<EmpTransferListDto>> { public Guid EmployeeId { get; set; } }

public class EmpContractAllHandler(IDapperHelper dapper) : IRequestHandler<EmpContractAllQry, List<EmpContractListDto>>
{
    public async Task<List<EmpContractListDto>> Handle(EmpContractAllQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "ContractNumber", "Status", "ContractType", "StartDate", "EndDate", "SignedDate",
                   "TerminatedDate", "TerminationReason", "DocumentRef", "Notes", "EmployeeId", "RenewedFromId", xmin
            FROM "EmpContract" WHERE "IsDeleted" = false ORDER BY "DateAdd" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpContractRaw>(sql, null, ct)).ToList();
        return rows.Select(MapContract).ToList();
    }

    internal static EmpContractListDto MapContract(EmpContractRaw r) => new()
    {
        Id = r.Id,
        ContractNumber = r.ContractNumber,
        Status = r.Status,
        StatusName = MyEnumHelper.FormatEnum<ContractStatus>(r.Status),
        ContractType = r.ContractType,
        StartDate = r.StartDate,
        EndDate = r.EndDate,
        SignedDate = r.SignedDate,
        TerminatedDate = r.TerminatedDate,
        TerminationReason = r.TerminationReason,
        DocumentRef = r.DocumentRef,
        Notes = r.Notes,
        EmployeeId = r.EmployeeId,
        RenewedFromId = r.RenewedFromId,
        RowVersion = r.xmin.ToString()
    };
}

public class EmpContractByIdHandler(IDapperHelper dapper) : IRequestHandler<EmpContractByIdQry, EmpContractListDto?>
{
    public async Task<EmpContractListDto?> Handle(EmpContractByIdQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "ContractNumber", "Status", "ContractType", "StartDate", "EndDate", "SignedDate",
                   "TerminatedDate", "TerminationReason", "DocumentRef", "Notes", "EmployeeId", "RenewedFromId", xmin
            FROM "EmpContract" WHERE "Id" = @Id AND "IsDeleted" = false LIMIT 1
            """;
        var row = await dapper.QueryFirstOrDefaultAsync<EmpContractRaw>(sql, new { request.Id }, ct);
        return row == null ? null : EmpContractAllHandler.MapContract(row);
    }
}

public class EmpContractByEmployeeHandler(IDapperHelper dapper) : IRequestHandler<EmpContractByEmployeeQry, List<EmpContractListDto>>
{
    public async Task<List<EmpContractListDto>> Handle(EmpContractByEmployeeQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "ContractNumber", "Status", "ContractType", "StartDate", "EndDate", "SignedDate",
                   "TerminatedDate", "TerminationReason", "DocumentRef", "Notes", "EmployeeId", "RenewedFromId", xmin
            FROM "EmpContract" WHERE "EmployeeId" = @EmployeeId AND "IsDeleted" = false ORDER BY "StartDate" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpContractRaw>(sql, new { request.EmployeeId }, ct)).ToList();
        return rows.Select(EmpContractAllHandler.MapContract).ToList();
    }
}

public class EmpPromotionAllHandler(IDapperHelper dapper) : IRequestHandler<EmpPromotionAllQry, List<EmpPromotionListDto>>
{
    public async Task<List<EmpPromotionListDto>> Handle(EmpPromotionAllQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "Status", "EffectiveDate", "Reason", "Comments", "ApprovedById", "ApprovedDate", "AppliedDate",
                   "EmployeeId", "FromJobGradeId", "FromPositionId", "FromDepartmentId", "FromJgStepId",
                   "ToJobGradeId", "ToPositionId", "ToDepartmentId", "ToJgStepId", xmin
            FROM "EmpPromotion" WHERE "IsDeleted" = false ORDER BY "DateAdd" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpPromotionRaw>(sql, null, ct)).ToList();
        return rows.Select(Map).ToList();
    }

    internal static EmpPromotionListDto Map(EmpPromotionRaw r) => new()
    {
        Id = r.Id,
        Status = r.Status,
        StatusName = MyEnumHelper.FormatEnum<HrChangeStatus>(r.Status),
        EffectiveDate = r.EffectiveDate,
        Reason = r.Reason,
        Comments = r.Comments,
        ApprovedById = r.ApprovedById,
        ApprovedDate = r.ApprovedDate,
        AppliedDate = r.AppliedDate,
        EmployeeId = r.EmployeeId,
        FromJobGradeId = r.FromJobGradeId,
        FromPositionId = r.FromPositionId,
        FromDepartmentId = r.FromDepartmentId,
        FromJgStepId = r.FromJgStepId,
        ToJobGradeId = r.ToJobGradeId,
        ToPositionId = r.ToPositionId,
        ToDepartmentId = r.ToDepartmentId,
        ToJgStepId = r.ToJgStepId,
        RowVersion = r.xmin.ToString()
    };
}

public class EmpPromotionByIdHandler(IDapperHelper dapper) : IRequestHandler<EmpPromotionByIdQry, EmpPromotionListDto?>
{
    public async Task<EmpPromotionListDto?> Handle(EmpPromotionByIdQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "Status", "EffectiveDate", "Reason", "Comments", "ApprovedById", "ApprovedDate", "AppliedDate",
                   "EmployeeId", "FromJobGradeId", "FromPositionId", "FromDepartmentId", "FromJgStepId",
                   "ToJobGradeId", "ToPositionId", "ToDepartmentId", "ToJgStepId", xmin
            FROM "EmpPromotion" WHERE "Id" = @Id AND "IsDeleted" = false LIMIT 1
            """;
        var row = await dapper.QueryFirstOrDefaultAsync<EmpPromotionRaw>(sql, new { request.Id }, ct);
        return row == null ? null : EmpPromotionAllHandler.Map(row);
    }
}

public class EmpPromotionByEmployeeHandler(IDapperHelper dapper) : IRequestHandler<EmpPromotionByEmployeeQry, List<EmpPromotionListDto>>
{
    public async Task<List<EmpPromotionListDto>> Handle(EmpPromotionByEmployeeQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "Status", "EffectiveDate", "Reason", "Comments", "ApprovedById", "ApprovedDate", "AppliedDate",
                   "EmployeeId", "FromJobGradeId", "FromPositionId", "FromDepartmentId", "FromJgStepId",
                   "ToJobGradeId", "ToPositionId", "ToDepartmentId", "ToJgStepId", xmin
            FROM "EmpPromotion" WHERE "EmployeeId" = @EmployeeId AND "IsDeleted" = false ORDER BY "EffectiveDate" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpPromotionRaw>(sql, new { request.EmployeeId }, ct)).ToList();
        return rows.Select(EmpPromotionAllHandler.Map).ToList();
    }
}

public class EmpTransferAllHandler(IDapperHelper dapper) : IRequestHandler<EmpTransferAllQry, List<EmpTransferListDto>>
{
    public async Task<List<EmpTransferListDto>> Handle(EmpTransferAllQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "Status", "EffectiveDate", "Reason", "Comments", "ApprovedById", "ApprovedDate", "AppliedDate",
                   "EmployeeId", "FromDepartmentId", "FromPositionId", "FromJobGradeId",
                   "ToDepartmentId", "ToPositionId", "ToJobGradeId", xmin
            FROM "EmpTransfer" WHERE "IsDeleted" = false ORDER BY "DateAdd" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpTransferRaw>(sql, null, ct)).ToList();
        return rows.Select(Map).ToList();
    }

    internal static EmpTransferListDto Map(EmpTransferRaw r) => new()
    {
        Id = r.Id,
        Status = r.Status,
        StatusName = MyEnumHelper.FormatEnum<HrChangeStatus>(r.Status),
        EffectiveDate = r.EffectiveDate,
        Reason = r.Reason,
        Comments = r.Comments,
        ApprovedById = r.ApprovedById,
        ApprovedDate = r.ApprovedDate,
        AppliedDate = r.AppliedDate,
        EmployeeId = r.EmployeeId,
        FromDepartmentId = r.FromDepartmentId,
        FromPositionId = r.FromPositionId,
        FromJobGradeId = r.FromJobGradeId,
        ToDepartmentId = r.ToDepartmentId,
        ToPositionId = r.ToPositionId,
        ToJobGradeId = r.ToJobGradeId,
        RowVersion = r.xmin.ToString()
    };
}

public class EmpTransferByIdHandler(IDapperHelper dapper) : IRequestHandler<EmpTransferByIdQry, EmpTransferListDto?>
{
    public async Task<EmpTransferListDto?> Handle(EmpTransferByIdQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "Status", "EffectiveDate", "Reason", "Comments", "ApprovedById", "ApprovedDate", "AppliedDate",
                   "EmployeeId", "FromDepartmentId", "FromPositionId", "FromJobGradeId",
                   "ToDepartmentId", "ToPositionId", "ToJobGradeId", xmin
            FROM "EmpTransfer" WHERE "Id" = @Id AND "IsDeleted" = false LIMIT 1
            """;
        var row = await dapper.QueryFirstOrDefaultAsync<EmpTransferRaw>(sql, new { request.Id }, ct);
        return row == null ? null : EmpTransferAllHandler.Map(row);
    }
}

public class EmpTransferByEmployeeHandler(IDapperHelper dapper) : IRequestHandler<EmpTransferByEmployeeQry, List<EmpTransferListDto>>
{
    public async Task<List<EmpTransferListDto>> Handle(EmpTransferByEmployeeQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT "Id", "Status", "EffectiveDate", "Reason", "Comments", "ApprovedById", "ApprovedDate", "AppliedDate",
                   "EmployeeId", "FromDepartmentId", "FromPositionId", "FromJobGradeId",
                   "ToDepartmentId", "ToPositionId", "ToJobGradeId", xmin
            FROM "EmpTransfer" WHERE "EmployeeId" = @EmployeeId AND "IsDeleted" = false ORDER BY "EffectiveDate" DESC
            """;
        var rows = (await dapper.QueryAsync<EmpTransferRaw>(sql, new { request.EmployeeId }, ct)).ToList();
        return rows.Select(EmpTransferAllHandler.Map).ToList();
    }
}

public class EmpContractRaw
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string ContractType { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public DateTime? TerminatedDate { get; set; }
    public string? TerminationReason { get; set; }
    public string? DocumentRef { get; set; }
    public string? Notes { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? RenewedFromId { get; set; }
    public uint xmin { get; set; }
}

public class EmpPromotionRaw
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string? Comments { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? AppliedDate { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid FromJobGradeId { get; set; }
    public Guid FromPositionId { get; set; }
    public Guid FromDepartmentId { get; set; }
    public Guid? FromJgStepId { get; set; }
    public Guid ToJobGradeId { get; set; }
    public Guid ToPositionId { get; set; }
    public Guid ToDepartmentId { get; set; }
    public Guid? ToJgStepId { get; set; }
    public uint xmin { get; set; }
}

public class EmpTransferRaw
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string? Comments { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? AppliedDate { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid FromDepartmentId { get; set; }
    public Guid FromPositionId { get; set; }
    public Guid? FromJobGradeId { get; set; }
    public Guid ToDepartmentId { get; set; }
    public Guid ToPositionId { get; set; }
    public Guid? ToJobGradeId { get; set; }
    public uint xmin { get; set; }
}
