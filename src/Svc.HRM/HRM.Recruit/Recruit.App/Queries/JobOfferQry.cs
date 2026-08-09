using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class JobOfferAllQry : IRequest<List<JobOfferListDto>> { }

public class JobOfferByIdQry : IRequest<JobOfferViewDto?>
{
    public Guid Id { get; set; }
}

public class JobOfferByApplicationQry : IRequest<List<JobOfferListDto>>
{
    public Guid JobApplicationId { get; set; }
}

public class JobOfferAllHandler : IRequestHandler<JobOfferAllQry, List<JobOfferListDto>>
{
    private readonly IDapperHelper _dapper;

    public JobOfferAllHandler(IDapperHelper dapper) => _dapper = dapper;

    public async Task<List<JobOfferListDto>> Handle(JobOfferAllQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT o."Id", o."OfferNumber", o."Status", o."OfferDate", o."ExpirationDate",
                   o."OfferDocument", o."JobApplicationId", o."JobPostingId",
                   o."DateAdd", o."DateMod", o.xmin,
                   jp."PostNumber",
                   ja."EmployeeId" AS "HiredEmployeeId",
                   COALESCE(ap."FirstName" || ' ' || ap."LastName", '') AS "ApplicantName"
            FROM "JobOffer" o
            INNER JOIN "JobPosting" jp ON jp."Id" = o."JobPostingId" AND jp."IsDeleted" = false
            INNER JOIN "JobApplication" ja ON ja."Id" = o."JobApplicationId" AND ja."IsDeleted" = false
            LEFT JOIN "Applicant" a ON a."Id" = ja."ApplicantId" AND a."IsDeleted" = false
            LEFT JOIN "ApplicantPerson" ap ON ap."Id" = a."PersonId" AND ap."IsDeleted" = false
            WHERE o."IsDeleted" = false
            ORDER BY o."DateAdd" DESC
            """;

        var rows = (await _dapper.QueryAsync<JobOfferQueryRow>(sql, null, ct)).ToList();
        return rows.Select(MapList).ToList();
    }

    internal static JobOfferListDto MapList(JobOfferQueryRow raw) => new()
    {
        Id = raw.Id,
        OfferNumber = raw.OfferNumber,
        Status = raw.Status,
        StatusName = MyEnumHelper.FormatEnum<OfferStatus>(raw.Status),
        OfferDate = raw.OfferDate,
        OfferDateAm = raw.OfferDate.ToString("dd/MM/yyyy"),
        ExpirationDate = raw.ExpirationDate,
        ExpirationDateAm = raw.ExpirationDate.ToString("dd/MM/yyyy"),
        OfferDocument = raw.OfferDocument,
        JobApplicationId = raw.JobApplicationId,
        JobPostingId = raw.JobPostingId,
        ApplicantName = raw.ApplicantName,
        PostNumber = raw.PostNumber,
        HiredEmployeeId = raw.HiredEmployeeId,
        DateAdd = raw.DateAdd,
        DateAddAm = raw.DateAdd.ToString("dd/MM/yyyy HH:mm"),
        DateMod = raw.DateMod,
        DateModAm = raw.DateMod?.ToString("dd/MM/yyyy HH:mm") ?? "",
        RowVersion = raw.xmin.ToString()
    };
}

public class JobOfferByIdHandler : IRequestHandler<JobOfferByIdQry, JobOfferViewDto?>
{
    private readonly IDapperHelper _dapper;

    public JobOfferByIdHandler(IDapperHelper dapper) => _dapper = dapper;

    public async Task<JobOfferViewDto?> Handle(JobOfferByIdQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT o."Id", o."OfferNumber", o."Status", o."OfferDate", o."ExpirationDate",
                   o."OfferDocument", o."JobApplicationId", o."JobPostingId",
                   o."DateAdd", o."DateMod", o.xmin,
                   jp."PostNumber",
                   ja."EmployeeId" AS "HiredEmployeeId",
                   COALESCE(ap."FirstName" || ' ' || ap."LastName", '') AS "ApplicantName"
            FROM "JobOffer" o
            INNER JOIN "JobPosting" jp ON jp."Id" = o."JobPostingId" AND jp."IsDeleted" = false
            INNER JOIN "JobApplication" ja ON ja."Id" = o."JobApplicationId" AND ja."IsDeleted" = false
            LEFT JOIN "Applicant" a ON a."Id" = ja."ApplicantId" AND a."IsDeleted" = false
            LEFT JOIN "ApplicantPerson" ap ON ap."Id" = a."PersonId" AND ap."IsDeleted" = false
            WHERE o."Id" = @Id AND o."IsDeleted" = false
            LIMIT 1
            """;

        var raw = await _dapper.QueryFirstOrDefaultAsync<JobOfferQueryRow>(sql, new { request.Id }, ct);
        if (raw == null) return null;

        var list = JobOfferAllHandler.MapList(raw);
        var view = new JobOfferViewDto
        {
            Id = list.Id,
            OfferNumber = list.OfferNumber,
            Status = list.Status,
            StatusName = list.StatusName,
            OfferDate = list.OfferDate,
            OfferDateAm = list.OfferDateAm,
            ExpirationDate = list.ExpirationDate,
            ExpirationDateAm = list.ExpirationDateAm,
            OfferDocument = list.OfferDocument,
            JobApplicationId = list.JobApplicationId,
            JobPostingId = list.JobPostingId,
            ApplicantName = list.ApplicantName,
            PostNumber = list.PostNumber,
            HiredEmployeeId = list.HiredEmployeeId,
            DateAdd = list.DateAdd,
            DateAddAm = list.DateAddAm,
            DateMod = list.DateMod,
            DateModAm = list.DateModAm,
            RowVersion = list.RowVersion
        };

        const string approvalSql = """
            SELECT "Id", "StepOrder", "Role", "Status", "ApprovedDate", "ApprovedById"
            FROM "JobOfferApproval"
            WHERE "JobOfferId" = @Id AND "IsDeleted" = false
            ORDER BY "StepOrder"
            """;
        var approvals = (await _dapper.QueryAsync<JobOfferApprovalDto>(approvalSql, new { request.Id }, ct)).ToList();
        foreach (var a in approvals)
            a.StatusName = MyEnumHelper.FormatEnum<ApprovalStatus>(a.Status);
        view.Approvals = approvals;

        const string reviewSql = """
            SELECT "Id", "AcceptanceDate", "RejectionDate", "RejectionReason", "ApprovalComments"
            FROM "JobOfferReview"
            WHERE "JobOfferId" = @Id AND "IsDeleted" = false
            ORDER BY "DateAdd" DESC
            LIMIT 1
            """;
        view.Review = await _dapper.QueryFirstOrDefaultAsync<JobOfferReviewDto>(reviewSql, new { request.Id }, ct);
        return view;
    }
}

public class JobOfferByApplicationHandler : IRequestHandler<JobOfferByApplicationQry, List<JobOfferListDto>>
{
    private readonly IDapperHelper _dapper;

    public JobOfferByApplicationHandler(IDapperHelper dapper) => _dapper = dapper;

    public async Task<List<JobOfferListDto>> Handle(JobOfferByApplicationQry request, CancellationToken ct)
    {
        const string sql = """
            SELECT o."Id", o."OfferNumber", o."Status", o."OfferDate", o."ExpirationDate",
                   o."OfferDocument", o."JobApplicationId", o."JobPostingId",
                   o."DateAdd", o."DateMod", o.xmin,
                   jp."PostNumber",
                   ja."EmployeeId" AS "HiredEmployeeId",
                   COALESCE(ap."FirstName" || ' ' || ap."LastName", '') AS "ApplicantName"
            FROM "JobOffer" o
            INNER JOIN "JobPosting" jp ON jp."Id" = o."JobPostingId" AND jp."IsDeleted" = false
            INNER JOIN "JobApplication" ja ON ja."Id" = o."JobApplicationId" AND ja."IsDeleted" = false
            LEFT JOIN "Applicant" a ON a."Id" = ja."ApplicantId" AND a."IsDeleted" = false
            LEFT JOIN "ApplicantPerson" ap ON ap."Id" = a."PersonId" AND ap."IsDeleted" = false
            WHERE o."JobApplicationId" = @JobApplicationId AND o."IsDeleted" = false
            ORDER BY o."DateAdd" DESC
            """;

        var rows = (await _dapper.QueryAsync<JobOfferQueryRow>(sql, new { request.JobApplicationId }, ct)).ToList();
        return rows.Select(JobOfferAllHandler.MapList).ToList();
    }
}

public class JobOfferQueryRow : JobOfferRawDto
{
    public string? ApplicantName { get; set; }
    public string? PostNumber { get; set; }
    public Guid? HiredEmployeeId { get; set; }
}
