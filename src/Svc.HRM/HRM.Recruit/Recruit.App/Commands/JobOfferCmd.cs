using Common;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.App.Services;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobOfferAddCmd : IRequest<JobOfferListDto>
{
    public JobOfferAddDto AddDto { get; set; } = default!;
}

public class JobOfferModCmd : IRequest<JobOfferListDto>
{
    public JobOfferModDto ModDto { get; set; } = default!;
}

public class JobOfferDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class JobOfferSubmitCmd : IRequest<JobOfferListDto>
{
    public Guid Id { get; set; }
}

public class JobOfferApproveCmd : IRequest<JobOfferListDto>
{
    public JobOfferApproveDto Dto { get; set; } = default!;
}

public class JobOfferRejectCmd : IRequest<JobOfferListDto>
{
    public JobOfferRejectDto Dto { get; set; } = default!;
}

public class JobOfferExtendCmd : IRequest<JobOfferListDto>
{
    public Guid Id { get; set; }
}

public class JobOfferAcceptCmd : IRequest<JobOfferListDto>
{
    public JobOfferRespondDto Dto { get; set; } = default!;
}

public class JobOfferDeclineCmd : IRequest<JobOfferListDto>
{
    public JobOfferRespondDto Dto { get; set; } = default!;
}

public class JobOfferWithdrawCmd : IRequest<JobOfferListDto>
{
    public Guid Id { get; set; }
}

public class HireFromOfferCmd : IRequest<HireFromOfferResultDto>
{
    public HireFromOfferDto Dto { get; set; } = default!;
}

public class JobOfferAddHandler : IRequestHandler<JobOfferAddCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobOfferAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobOfferListDto> Handle(JobOfferAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var app = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.JobApplicationId && !x.IsDeleted, ct)
                ?? throw new DomainException($"Job application [{request.AddDto.JobApplicationId}] NOT FOUND.");

            if (request.AddDto.ExpirationDate.Date < request.AddDto.OfferDate.Date)
                throw new ValException("Expiration date must be on or after offer date.");

            var hasSteps = request.AddDto.ApprovalSteps is { Count: > 0 };
            var status = hasSteps
                ? BoolToStr.EnumToString(OfferStatus.Draft)
                : BoolToStr.EnumToString(OfferStatus.Approved);

            var offer = new JobOffer
            {
                Id = Guid.CreateVersion7(),
                OfferNumber = await GenerateOfferNumber(ct),
                Status = status,
                OfferDate = DateTime.SpecifyKind(request.AddDto.OfferDate, DateTimeKind.Utc),
                ExpirationDate = DateTime.SpecifyKind(request.AddDto.ExpirationDate, DateTimeKind.Utc),
                OfferDocument = request.AddDto.OfferDocument.Trim(),
                JobApplicationId = app.Id,
                JobPostingId = app.JobPostingId,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(offer, ct);

            if (hasSteps)
            {
                foreach (var step in request.AddDto.ApprovalSteps!.OrderBy(s => s.StepOrder))
                {
                    await _uow.Add(new JobOfferApproval
                    {
                        Id = Guid.CreateVersion7(),
                        StepOrder = step.StepOrder,
                        Role = step.Role.Trim(),
                        Status = BoolToStr.EnumToString(ApprovalStatus.Pending),
                        JobOfferId = offer.Id,
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false
                    }, ct);
                }
            }

            await _uow.Commit(ct);

            var response = await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct);
            return response ?? throw new DomainException("Failed to retrieve created JOB OFFER.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task<string> GenerateOfferNumber(CancellationToken ct)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"JO-{year}-";
        var count = await _uow.Set<JobOffer>().CountAsync(x => x.OfferNumber.StartsWith(prefix), ct);
        return $"{prefix}{(count + 1):D3}";
    }
}

public class JobOfferModHandler : IRequestHandler<JobOfferModCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobOfferModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobOfferListDto> Handle(JobOfferModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.ModDto.Id}] NOT FOUND.");

            EnsureEditable(entity.Status);
            EnsureRowVersion(entity, request.ModDto.RowVersion);

            entity.OfferDate = DateTime.SpecifyKind(request.ModDto.OfferDate, DateTimeKind.Utc);
            entity.ExpirationDate = DateTime.SpecifyKind(request.ModDto.ExpirationDate, DateTimeKind.Utc);
            entity.OfferDocument = request.ModDto.OfferDocument.Trim();
            entity.DateMod = DateTime.UtcNow;

            await _uow.Update(entity);
            await _uow.Commit(ct);

            return await _med.Send(new JobOfferByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{entity.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private static void EnsureEditable(string status)
    {
        var parsed = MyEnumHelper.TryParseEnum<OfferStatus>(status);
        if (parsed is not (OfferStatus.Draft or OfferStatus.Pending or OfferStatus.Approved))
            throw new DomainException("Only Draft/Pending/Approved offers can be modified.");
    }

    private static void EnsureRowVersion(JobOffer entity, string rowVersion)
    {
        if (!string.IsNullOrEmpty(rowVersion) && entity.xmin.ToString() != rowVersion)
            throw new DomainException("The record has been modified by another user. Please refresh and try again.");
    }
}

public class JobOfferDelHandler : IRequestHandler<JobOfferDelCmd>
{
    private readonly IUnitOfWork _uow;

    public JobOfferDelHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(JobOfferDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.Id}] NOT FOUND.");

            var status = MyEnumHelper.TryParseEnum<OfferStatus>(entity.Status);
            if (status is OfferStatus.Accepted or OfferStatus.Extended)
                throw new DomainException("Accepted or Extended offers cannot be deleted.");

            entity.IsDeleted = true;
            entity.DateMod = DateTime.UtcNow;
            await _uow.Update(entity);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobOfferSubmitHandler : IRequestHandler<JobOfferSubmitCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobOfferSubmitHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobOfferListDto> Handle(JobOfferSubmitCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await LoadOffer(request.Id, ct);
            var status = MyEnumHelper.TryParseEnum<OfferStatus>(offer.Status);
            if (status != OfferStatus.Draft)
                throw new DomainException("Only Draft offers can be submitted for approval.");

            var steps = await _uow.Set<JobOfferApproval>()
                .Where(x => x.JobOfferId == offer.Id && !x.IsDeleted)
                .ToListAsync(ct);

            offer.Status = steps.Count == 0
                ? BoolToStr.EnumToString(OfferStatus.Approved)
                : BoolToStr.EnumToString(OfferStatus.Pending);
            offer.DateMod = DateTime.UtcNow;

            await _uow.Update(offer);
            await _uow.Commit(ct);

            return await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{offer.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task<JobOffer> LoadOffer(Guid id, CancellationToken ct) =>
        await _uow.Set<JobOffer>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
        ?? throw new DomainException($"JOB OFFER [{id}] NOT FOUND.");
}

public class JobOfferApproveHandler : IRequestHandler<JobOfferApproveCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobOfferApproveHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobOfferListDto> Handle(JobOfferApproveCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.Dto.OfferId && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.Dto.OfferId}] NOT FOUND.");

            var status = MyEnumHelper.TryParseEnum<OfferStatus>(offer.Status);
            if (status is not (OfferStatus.Pending or OfferStatus.Draft or OfferStatus.Approved))
                throw new DomainException("Offer is not awaiting approval.");

            var steps = await _uow.Set<JobOfferApproval>()
                .Where(x => x.JobOfferId == offer.Id && !x.IsDeleted)
                .OrderBy(x => x.StepOrder)
                .ToListAsync(ct);

            if (steps.Count == 0)
            {
                offer.Status = BoolToStr.EnumToString(OfferStatus.Approved);
            }
            else
            {
                var pending = steps.FirstOrDefault(s =>
                    MyEnumHelper.TryParseEnum<ApprovalStatus>(s.Status) == ApprovalStatus.Pending)
                    ?? throw new DomainException("No pending approval step found.");

                pending.Status = BoolToStr.EnumToString(ApprovalStatus.Approved);
                pending.ApprovedById = request.Dto.ApprovedById;
                pending.ApprovedDate = DateTime.UtcNow;
                pending.DateMod = DateTime.UtcNow;
                await _uow.Update(pending);

                var allApproved = steps.All(s =>
                    s.Id == pending.Id ||
                    MyEnumHelper.TryParseEnum<ApprovalStatus>(s.Status) == ApprovalStatus.Approved);

                offer.Status = allApproved
                    ? BoolToStr.EnumToString(OfferStatus.Approved)
                    : BoolToStr.EnumToString(OfferStatus.Pending);
            }

            if (!string.IsNullOrWhiteSpace(request.Dto.Comments))
            {
                await _uow.Add(new JobOfferReview
                {
                    Id = Guid.CreateVersion7(),
                    JobOfferId = offer.Id,
                    ApprovalComments = request.Dto.Comments.Trim(),
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                }, ct);
            }

            offer.DateMod = DateTime.UtcNow;
            await _uow.Update(offer);
            await _uow.Commit(ct);

            return await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{offer.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobOfferRejectHandler : IRequestHandler<JobOfferRejectCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IRecruitNotificationService _notificationService;

    public JobOfferRejectHandler(IUnitOfWork uow, IMediator med, IRecruitNotificationService notificationService)
    {
        _uow = uow;
        _med = med;
        _notificationService = notificationService;
    }

    public async Task<JobOfferListDto> Handle(JobOfferRejectCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.Dto.OfferId && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.Dto.OfferId}] NOT FOUND.");

            offer.Status = BoolToStr.EnumToString(OfferStatus.Rejected);
            offer.DateMod = DateTime.UtcNow;

            var pending = await _uow.Set<JobOfferApproval>()
                .Where(x => x.JobOfferId == offer.Id && !x.IsDeleted)
                .OrderBy(x => x.StepOrder)
                .FirstOrDefaultAsync(x => x.Status == BoolToStr.EnumToString(ApprovalStatus.Pending), ct);

            if (pending != null)
            {
                pending.Status = BoolToStr.EnumToString(ApprovalStatus.Rejected);
                pending.ApprovedById = request.Dto.RejectedById;
                pending.ApprovedDate = DateTime.UtcNow;
                pending.DateMod = DateTime.UtcNow;
                await _uow.Update(pending);
            }

            await _uow.Add(new JobOfferReview
            {
                Id = Guid.CreateVersion7(),
                JobOfferId = offer.Id,
                RejectionDate = DateTime.UtcNow,
                RejectionReason = request.Dto.Reason.Trim(),
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            }, ct);

            var app = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == offer.JobApplicationId && !x.IsDeleted, ct);
            if (app != null)
            {
                app.Status = BoolToStr.EnumToString(ApplicationStatus.Rejected);
                app.DateMod = DateTime.UtcNow;
                await _uow.Update(app);
            }

            await _uow.Update(offer);
            await _uow.Commit(ct);

            await _notificationService.NotifyOfferRejectedAsync(offer.Id, ct);

            return await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{offer.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobOfferExtendHandler : IRequestHandler<JobOfferExtendCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IRecruitNotificationService _notificationService;

    public JobOfferExtendHandler(IUnitOfWork uow, IMediator med, IRecruitNotificationService notificationService)
    {
        _uow = uow;
        _med = med;
        _notificationService = notificationService;
    }

    public async Task<JobOfferListDto> Handle(JobOfferExtendCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.Id}] NOT FOUND.");

            var status = MyEnumHelper.TryParseEnum<OfferStatus>(offer.Status);
            if (status != OfferStatus.Approved)
                throw new DomainException("Only Approved offers can be extended to the candidate.");

            if (offer.ExpirationDate.Date < DateTime.UtcNow.Date)
            {
                offer.Status = BoolToStr.EnumToString(OfferStatus.Expired);
                offer.DateMod = DateTime.UtcNow;
                await _uow.Update(offer);
                await _uow.Commit(ct);
                throw new DomainException("Offer has expired and cannot be extended.");
            }

            offer.Status = BoolToStr.EnumToString(OfferStatus.Extended);
            offer.DateMod = DateTime.UtcNow;

            var app = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == offer.JobApplicationId && !x.IsDeleted, ct);
            if (app != null)
            {
                app.Status = BoolToStr.EnumToString(ApplicationStatus.OfferExtended);
                app.DateMod = DateTime.UtcNow;
                await _uow.Update(app);
            }

            await _uow.Update(offer);
            await _uow.Commit(ct);

            await _notificationService.NotifyOfferExtendedAsync(offer.Id, ct);

            return await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{offer.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobOfferAcceptHandler : IRequestHandler<JobOfferAcceptCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IRecruitNotificationService _notificationService;

    public JobOfferAcceptHandler(IUnitOfWork uow, IMediator med, IRecruitNotificationService notificationService)
    {
        _uow = uow;
        _med = med;
        _notificationService = notificationService;
    }

    public async Task<JobOfferListDto> Handle(JobOfferAcceptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.Dto.OfferId && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.Dto.OfferId}] NOT FOUND.");

            var status = MyEnumHelper.TryParseEnum<OfferStatus>(offer.Status);
            if (status != OfferStatus.Extended)
                throw new DomainException("Only Extended offers can be accepted.");

            if (offer.ExpirationDate.Date < DateTime.UtcNow.Date)
            {
                offer.Status = BoolToStr.EnumToString(OfferStatus.Expired);
                offer.DateMod = DateTime.UtcNow;
                await _uow.Update(offer);
                await _uow.Commit(ct);
                throw new DomainException("Offer has expired.");
            }

            offer.Status = BoolToStr.EnumToString(OfferStatus.Accepted);
            offer.DateMod = DateTime.UtcNow;

            await _uow.Add(new JobOfferReview
            {
                Id = Guid.CreateVersion7(),
                JobOfferId = offer.Id,
                AcceptanceDate = DateTime.UtcNow,
                ApprovalComments = request.Dto.Comments?.Trim(),
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            }, ct);

            var app = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == offer.JobApplicationId && !x.IsDeleted, ct);
            if (app != null)
            {
                app.Status = BoolToStr.EnumToString(ApplicationStatus.OfferAccepted);
                app.DateMod = DateTime.UtcNow;
                await _uow.Update(app);
            }

            await _uow.Update(offer);
            await _uow.Commit(ct);

            await _notificationService.NotifyOfferAcceptedAsync(offer.Id, ct);

            return await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{offer.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobOfferDeclineHandler : IRequestHandler<JobOfferDeclineCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IRecruitNotificationService _notificationService;

    public JobOfferDeclineHandler(IUnitOfWork uow, IMediator med, IRecruitNotificationService notificationService)
    {
        _uow = uow;
        _med = med;
        _notificationService = notificationService;
    }

    public async Task<JobOfferListDto> Handle(JobOfferDeclineCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.Dto.OfferId && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.Dto.OfferId}] NOT FOUND.");

            var status = MyEnumHelper.TryParseEnum<OfferStatus>(offer.Status);
            if (status != OfferStatus.Extended)
                throw new DomainException("Only Extended offers can be declined.");

            offer.Status = BoolToStr.EnumToString(OfferStatus.Declined);
            offer.DateMod = DateTime.UtcNow;

            await _uow.Add(new JobOfferReview
            {
                Id = Guid.CreateVersion7(),
                JobOfferId = offer.Id,
                RejectionDate = DateTime.UtcNow,
                RejectionReason = request.Dto.RejectionReason?.Trim() ?? "Declined by candidate",
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            }, ct);

            var app = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == offer.JobApplicationId && !x.IsDeleted, ct);
            if (app != null)
            {
                app.Status = BoolToStr.EnumToString(ApplicationStatus.OfferRejected);
                app.DateMod = DateTime.UtcNow;
                await _uow.Update(app);
            }

            await _uow.Update(offer);
            await _uow.Commit(ct);

            await _notificationService.NotifyOfferRejectedAsync(offer.Id, ct);

            return await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{offer.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobOfferWithdrawHandler : IRequestHandler<JobOfferWithdrawCmd, JobOfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobOfferWithdrawHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobOfferListDto> Handle(JobOfferWithdrawCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException($"JOB OFFER [{request.Id}] NOT FOUND.");

            var status = MyEnumHelper.TryParseEnum<OfferStatus>(offer.Status);
            if (status is OfferStatus.Accepted or OfferStatus.Declined or OfferStatus.Expired)
                throw new DomainException("Offer cannot be withdrawn in its current status.");

            offer.Status = BoolToStr.EnumToString(OfferStatus.Withdrawn);
            offer.DateMod = DateTime.UtcNow;
            await _uow.Update(offer);
            await _uow.Commit(ct);

            return await _med.Send(new JobOfferByIdQry { Id = offer.Id }, ct)
                ?? throw new DomainException($"JOB OFFER [{offer.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class HireFromOfferHandler : IRequestHandler<HireFromOfferCmd, HireFromOfferResultDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IHrmProfileHireClient _hireClient;
    private readonly ICorHrmmClient _hrmmClient;

    public HireFromOfferHandler(IUnitOfWork uow, IHrmProfileHireClient hireClient, ICorHrmmClient hrmmClient)
    {
        _uow = uow;
        _hireClient = hireClient;
        _hrmmClient = hrmmClient;
    }

    public async Task<HireFromOfferResultDto> Handle(HireFromOfferCmd request, CancellationToken ct)
    {
        var offer = await _uow.Set<JobOffer>()
            .FirstOrDefaultAsync(x => x.Id == request.Dto.OfferId && !x.IsDeleted, ct)
            ?? throw new DomainException($"JOB OFFER [{request.Dto.OfferId}] NOT FOUND.");

        var status = MyEnumHelper.TryParseEnum<OfferStatus>(offer.Status);
        if (status != OfferStatus.Accepted)
            throw new DomainException("Only Accepted offers can be converted to employees.");

        var app = await _uow.Set<JobApplication>()
            .FirstOrDefaultAsync(x => x.Id == offer.JobApplicationId && !x.IsDeleted, ct)
            ?? throw new DomainException("Linked job application not found.");

        if (app.EmployeeId.HasValue && app.EmployeeId.Value != Guid.Empty)
            throw new DomainException($"Application already linked to employee [{app.EmployeeId}].");

        if (!app.ApplicantId.HasValue)
            throw new DomainException("Only external applicant offers can be hired via this endpoint. Internal transfers require Profile.");

        var applicant = await _uow.Set<Applicant>()
            .Include(a => a.Person)
            .Include(a => a.Address)
            .Include(a => a.Contact)
            .FirstOrDefaultAsync(x => x.Id == app.ApplicantId.Value && !x.IsDeleted, ct)
            ?? throw new DomainException("Applicant not found for this offer.");

        var posting = await _uow.Set<JobPosting>()
            .FirstOrDefaultAsync(x => x.Id == offer.JobPostingId && !x.IsDeleted, ct)
            ?? throw new DomainException("Job posting not found.");

        var jobReq = await _uow.Set<JobRequisition>()
            .FirstOrDefaultAsync(x => x.Id == posting.JobReqId && !x.IsDeleted, ct)
            ?? throw new DomainException("Job requisition not found.");

        var plan = await _uow.Set<WorkforcePlan>()
            .FirstOrDefaultAsync(x => x.Id == jobReq.WorkforcePlanId && !x.IsDeleted, ct)
            ?? throw new DomainException("Workforce plan not found.");

        var positionId = request.Dto.PositionId ?? jobReq.PositionId;
        var jgStepId = request.Dto.JgStepId ?? jobReq.JgStepId;
        var departmentId = request.Dto.DepartmentId ?? plan.DepartmentId;

        if (request.Dto.JobGradeId == Guid.Empty)
            throw new ValException("JobGradeId is required to hire from offer.");

        // Validate position exists in Cor.HRMM (best-effort)
        try { await _hrmmClient.GetPosition(positionId.ToString(), ct); }
        catch { /* non-blocking; Profile will validate on hire */ }

        var hireReq = new HireEmployeeRequest
        {
            FirstName = applicant.Person.FirstName,
            FirstNameAm = applicant.Person.FirstNameAm,
            MiddleName = applicant.Person.MiddleName,
            MiddleNameAm = applicant.Person.MiddleNameAm,
            LastName = applicant.Person.LastName,
            LastNameAm = applicant.Person.LastNameAm,
            Gender = applicant.Person.Gender,
            Nationality = applicant.Person.Nationality,
            EmploymentDate = request.Dto.EmploymentDate ?? DateTime.UtcNow,
            JobGradeId = request.Dto.JobGradeId,
            JgStepId = jgStepId,
            PositionId = positionId,
            DepartmentId = departmentId,
            EmploymentType = request.Dto.EmploymentType,
            EmploymentNature = request.Dto.EmploymentNature,
            WorkArrangement = request.Dto.WorkArrangement,
            BirthDate = request.Dto.BirthDate ?? DateTime.UtcNow.AddYears(-25),
            MaritalStatus = request.Dto.MaritalStatus,
            AddressType = string.IsNullOrWhiteSpace(applicant.Address.AddressType)
                ? BoolToStr.EnumToString(AddressType.Res)
                : applicant.Address.AddressType,
            Country = applicant.Address.Country,
            Region = string.IsNullOrWhiteSpace(applicant.Address.Region) ? "N/A" : applicant.Address.Region,
            Subcity = applicant.Address.Subcity,
            Zone = applicant.Address.Zone,
            Woreda = applicant.Address.Woreda,
            Kebele = applicant.Address.Kebele,
            HouseNo = applicant.Address.HouseNo,
            Telephone = string.IsNullOrWhiteSpace(applicant.Contact.Phone) ? "0000000000" : applicant.Contact.Phone,
            PoBox = applicant.Contact.PoBox,
            Fax = applicant.Contact.Fax,
            Email = applicant.Contact.Email
        };

        var hireResult = await _hireClient.CreateEmployeeAsync(hireReq, ct);

        await _uow.Begin(ct);
        try
        {
            app.EmployeeId = hireResult.EmployeeId;
            app.DateMod = DateTime.UtcNow;
            await _uow.Update(app);

            var assignments = 0;
            if (request.Dto.AssignOnboardingTasks)
            {
                var tasks = await _uow.Set<OnboardingTask>()
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.SequenceOrder)
                    .ToListAsync(ct);

                var schedule = DateTime.UtcNow.Date.AddDays(Math.Max(1, request.Dto.OnboardingDaysOffset));
                foreach (var task in tasks)
                {
                    await _uow.Add(new OnboardingAssign
                    {
                        Id = Guid.CreateVersion7(),
                        EmployeeId = hireResult.EmployeeId,
                        OnboardingTaskId = task.Id,
                        IsMandatory = true,
                        Status = BoolToStr.EnumToString(OnboardingStatus.Pending),
                        ScheduledDate = DateTime.SpecifyKind(schedule, DateTimeKind.Utc),
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false
                    }, ct);
                    assignments++;
                }
            }

            await _uow.Commit(ct);

            return new HireFromOfferResultDto
            {
                OfferId = offer.Id,
                EmployeeId = hireResult.EmployeeId,
                JobApplicationId = app.Id,
                OnboardingAssignmentsCreated = assignments
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}
