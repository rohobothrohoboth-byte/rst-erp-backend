// Recruit.App/Queries/PublicQry.cs

using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;
using Recruit.App.Services;
using Microsoft.EntityFrameworkCore;
namespace Recruit.App.Queries;

public class CheckApplicantExistsQry : IRequest<ApplicantExistsDto>
{
    public string Email { get; set; } = default!;
}

public class ApplicantByContactQry : IRequest<ExternalApplicantRegistrationResponseDto>
{
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
}

public class JobAppByApplicantQry : IRequest<List<JobAppListDto>>
{
    public Guid ApplicantId { get; set; }
}

public class WithdrawApplicationCmd : IRequest<JobAppListDto>
{
    public Guid ApplicationId { get; set; }
    public string? Reason { get; set; }
}

public class CheckApplicantExistsHandler : IRequestHandler<CheckApplicantExistsQry, ApplicantExistsDto>
{
    private readonly IDapperHelper _dapper;

    public CheckApplicantExistsHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<ApplicantExistsDto> Handle(CheckApplicantExistsQry request, CancellationToken ct)
    {
        var sql = @"
            SELECT
                a.""Id"" as ApplicantId,
                a.""RegisteredDate"",
                pp.""FirstName"",
                pp.""MiddleName"",
                pp.""LastName"",
                pc.""Email""
            FROM ""Applicant"" a
            JOIN ""ApplicantContact"" pc ON a.""ContactId"" = pc.""Id""
            JOIN ""ApplicantPerson"" pp ON a.""PersonId"" = pp.""Id""
            WHERE pc.""Email"" = @Email AND a.""IsDeleted"" = false
            LIMIT 1
        ";

        var result = await _dapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { request.Email }, ct);

        if (result == null)
        {
            return new ApplicantExistsDto { Exists = false };
        }

        var fullName = $"{result.FirstName} {result.MiddleName} {result.LastName}".Trim();

        return new ApplicantExistsDto
        {
            Exists = true,
            ApplicantId = result.ApplicantId,
            FullName = fullName,
            Email = result.Email
        };
    }
}

public class ApplicantByContactHandler : IRequestHandler<ApplicantByContactQry, ExternalApplicantRegistrationResponseDto>
{
    private readonly IDapperHelper _dapper;

    public ApplicantByContactHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<ExternalApplicantRegistrationResponseDto> Handle(ApplicantByContactQry request, CancellationToken ct)
    {
        var sql = @"
            SELECT
                a.""Id"" as ApplicantId,
                a.""PersonId"",
                a.""ContactId"",
                a.""AddressId"",
                pp.""FirstName"",
                pp.""MiddleName"",
                pp.""LastName"",
                pc.""Email"",
                pc.""Phone"",
                pc.""AlternatePhone""
            FROM ""Applicant"" a
            JOIN ""ApplicantContact"" pc ON a.""ContactId"" = pc.""Id""
            JOIN ""ApplicantPerson"" pp ON a.""PersonId"" = pp.""Id""
            WHERE pc.""Email"" = @Email AND a.""IsDeleted"" = false
            LIMIT 1
        ";

        var result = await _dapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { request.Email }, ct);

        if (result == null)
        {
            return null!;
        }

        var fullName = $"{result.FirstName} {result.MiddleName} {result.LastName}".Trim();

        return new ExternalApplicantRegistrationResponseDto
        {
            ApplicantId = result.ApplicantId,
            PersonId = result.PersonId,
            ContactId = result.ContactId,
            AddressId = result.AddressId,
            FullName = fullName,
            Email = result.Email
        };
    }
}

public class JobAppByApplicantHandler : IRequestHandler<JobAppByApplicantQry, List<JobAppListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IJobAppService _jobAppService;

    public JobAppByApplicantHandler(IDapperHelper dapper, IJobAppService jobAppService)
    {
        _dapper = dapper;
        _jobAppService = jobAppService;
    }

    public async Task<List<JobAppListDto>> Handle(JobAppByApplicantQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<JobApplication>(v, x => x.Id, x => x.AppliedDate, x => x.Status, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<JobApplication>(v)
            .Where<JobApplication>(v, x => x.ApplicantId == request.ApplicantId)
            .OrderBy<JobApplication>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var list = (await _dapper.QueryAsync<JobAppListDto>(sql, parameters, ct)).ToList();

        if (list.Count == 0) { return []; }

        var ids = list.Select(x => x.Id).ToList();
        var jobInfos = await _jobAppService.GetJobAppBatchInfo(ids, ct);

        foreach (var data in list)
        {
            jobInfos.TryGetValue(data.Id, out var jAppInfo);
            data.StatusStr = MyEnumHelper.FormatEnum<ApplicationStatus>(data.Status);
            data.Applicant = jAppInfo?.Applicant ?? "";
            data.JobPostingNum = jAppInfo?.PostNumber ?? "";
            data.Position = jAppInfo?.Position ?? "";
            data.Department = jAppInfo?.Department ?? "";
            data.Period = jAppInfo?.Period ?? "";
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}
public class WithdrawApplicationHandler : IRequestHandler<WithdrawApplicationCmd, JobAppListDto>
{
    private readonly IUnitOfWork _uow; // ✅ Use IUnitOfWork instead of DbSet
    private readonly IMediator _med;

    public WithdrawApplicationHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobAppListDto> Handle(WithdrawApplicationCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // ✅ Use IUnitOfWork to access the DbSet
            var jobApp = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == request.ApplicationId && !x.IsDeleted, ct);

            if (jobApp == null)
                throw new DomainException($"Job application with Id {request.ApplicationId} NOT FOUND.");

            // Only allow withdrawal if status is Applied, UnderReview, or Shortlisted
            var currentStatus = jobApp.Status;
            var allowedStatuses = new[] {
                BoolToStr.EnumToString(ApplicationStatus.Applied),
                BoolToStr.EnumToString(ApplicationStatus.UnderReview),
                BoolToStr.EnumToString(ApplicationStatus.Shortlisted)
            };

            if (!allowedStatuses.Contains(currentStatus))
            {
                throw new DomainException($"Cannot withdraw application with status '{currentStatus}'. Only Applied, Under Review, or Shortlisted applications can be withdrawn.");
            }

            jobApp.Status = BoolToStr.EnumToString(ApplicationStatus.Withdrawn);
            jobApp.DateMod = DateTime.UtcNow;
            await _uow.Update(jobApp);
            await _uow.Commit(ct);

            var response = await _med.Send(new JobAppByIdQry { Id = request.ApplicationId }, ct);
            return response ?? new JobAppListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}