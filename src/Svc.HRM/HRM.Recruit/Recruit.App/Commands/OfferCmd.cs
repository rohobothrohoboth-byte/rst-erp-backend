using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.App.Services;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class OfferAddCmd : IRequest<OfferListDto> { public OfferAddDto AddDto { get; set; } = default!; }
public class OfferModCmd : IRequest<OfferListDto> { public OfferModDto ModDto { get; set; } = default!; }
public class OfferDelCmd : IRequest { public Guid Id { get; set; } }
public class OfferSendCmd : IRequest<OfferListDto> { public Guid Id { get; set; } }
public class OfferRespondCmd : IRequest<OfferListDto> { public OfferResponseDto Dto { get; set; } = default!; }

public class OfferAddHandler : IRequestHandler<OfferAddCmd, OfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public OfferAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<OfferListDto> Handle(OfferAddCmd request, CancellationToken ct)
    {
        var d = request.AddDto;
        if (d.ApplicantId == Guid.Empty) throw new DomainException("Applicant is required.");
        if (d.JobPostingId == Guid.Empty) throw new DomainException("Job posting is required.");

        await _uow.Begin(ct);
        try
        {
            // Link to the applicant's application for this posting (if one exists).
            var app = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.ApplicantId == d.ApplicantId && x.JobPostingId == d.JobPostingId && !x.IsDeleted, ct);

            var offer = new JobOffer
            {
                OfferNumber = await GenerateOfferNumber(ct),
                Status = "Draft",
                OfferDate = DateTime.UtcNow,
                ExpirationDate = EnsureUtc(d.ExpiryDate),
                OfferDocument = string.Empty,
                ApplicantId = d.ApplicantId,
                JobPostingId = d.JobPostingId,
                JobApplicationId = app?.Id ?? Guid.Empty,
                Salary = d.Salary,
                Currency = string.IsNullOrWhiteSpace(d.Currency) ? "ETB" : d.Currency,
                Benefits = d.Benefits,
                StartDate = EnsureUtc(d.StartDate),
                Notes = d.Notes,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };
            await _uow.Add(offer, ct);
            await _uow.Commit(ct);

            return await _med.Send(new OfferByIdQry { Id = offer.Id }, ct) ?? new OfferListDto();
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
        var count = await _uow.Set<JobOffer>().CountAsync(x => x.OfferNumber.StartsWith($"OF-{year}-"), ct);
        return $"OF-{year}-{(count + 1):D3}";
    }

    private static DateTime EnsureUtc(DateTime d) =>
        d.Kind == DateTimeKind.Utc ? d : DateTime.SpecifyKind(d, DateTimeKind.Utc);
}

public class OfferModHandler : IRequestHandler<OfferModCmd, OfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public OfferModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<OfferListDto> Handle(OfferModCmd request, CancellationToken ct)
    {
        var m = request.ModDto;
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>().FirstOrDefaultAsync(x => x.Id == m.Id && !x.IsDeleted, ct);
            if (offer == null) throw new DomainException($"OFFER with id [{m.Id}] NOT FOUND.");

            offer.Salary = m.Salary;
            offer.Benefits = m.Benefits;
            offer.StartDate = m.StartDate.Kind == DateTimeKind.Utc ? m.StartDate : DateTime.SpecifyKind(m.StartDate, DateTimeKind.Utc);
            offer.ExpirationDate = m.ExpiryDate.Kind == DateTimeKind.Utc ? m.ExpiryDate : DateTime.SpecifyKind(m.ExpiryDate, DateTimeKind.Utc);
            offer.Notes = m.Notes;
            if (!string.IsNullOrWhiteSpace(m.Status)) offer.Status = m.Status;
            offer.DateMod = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(m.RowVersion) && uint.TryParse(m.RowVersion, out var rv)) offer.SetRowVersion(rv);

            await _uow.Update(offer);
            await _uow.Commit(ct);

            return await _med.Send(new OfferByIdQry { Id = offer.Id }, ct) ?? new OfferListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OfferDelHandler : IRequestHandler<OfferDelCmd>
{
    private readonly IUnitOfWork _uow;
    public OfferDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(OfferDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (offer == null) throw new DomainException($"OFFER with id [{request.Id}] NOT FOUND.");
            await _uow.Delete(offer);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OfferSendHandler : IRequestHandler<OfferSendCmd, OfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IRecruitNotificationService _notify;
    public OfferSendHandler(IUnitOfWork uow, IMediator med, IRecruitNotificationService notify) { _uow = uow; _med = med; _notify = notify; }

    public async Task<OfferListDto> Handle(OfferSendCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
            if (offer == null) throw new DomainException($"OFFER with id [{request.Id}] NOT FOUND.");

            offer.Status = "Sent";
            offer.DateMod = DateTime.UtcNow;
            await _uow.Update(offer);

            await AdvanceApplication(offer.JobApplicationId, ApplicationStatus.OfferExtended, ct);
            await _uow.Commit(ct);

            try { await _notify.NotifyOfferExtendedAsync(offer.Id, ct); } catch { /* best-effort */ }

            return await _med.Send(new OfferByIdQry { Id = offer.Id }, ct) ?? new OfferListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task AdvanceApplication(Guid jobApplicationId, ApplicationStatus status, CancellationToken ct)
    {
        if (jobApplicationId == Guid.Empty) return;
        var app = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == jobApplicationId, ct);
        if (app == null) return;
        app.Status = BoolToStr.EnumToString(status);
        app.DateMod = DateTime.UtcNow;
        await _uow.Update(app);
    }
}

public class OfferRespondHandler : IRequestHandler<OfferRespondCmd, OfferListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IRecruitNotificationService _notify;
    public OfferRespondHandler(IUnitOfWork uow, IMediator med, IRecruitNotificationService notify) { _uow = uow; _med = med; _notify = notify; }

    public async Task<OfferListDto> Handle(OfferRespondCmd request, CancellationToken ct)
    {
        var accepted = string.Equals(request.Dto.Status, "Accepted", StringComparison.OrdinalIgnoreCase);
        await _uow.Begin(ct);
        try
        {
            var offer = await _uow.Set<JobOffer>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct);
            if (offer == null) throw new DomainException($"OFFER with id [{request.Dto.Id}] NOT FOUND.");

            offer.Status = accepted ? "Accepted" : "Rejected";
            if (!string.IsNullOrWhiteSpace(request.Dto.Comment)) offer.Notes = request.Dto.Comment;
            offer.DateMod = DateTime.UtcNow;
            await _uow.Update(offer);

            if (offer.JobApplicationId != Guid.Empty)
            {
                var app = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == offer.JobApplicationId, ct);
                if (app != null)
                {
                    app.Status = BoolToStr.EnumToString(accepted ? ApplicationStatus.OfferAccepted : ApplicationStatus.OfferRejected);
                    app.DateMod = DateTime.UtcNow;
                    await _uow.Update(app);
                }
            }
            await _uow.Commit(ct);

            try
            {
                if (accepted) await _notify.NotifyOfferAcceptedAsync(offer.Id, ct);
                else await _notify.NotifyOfferRejectedAsync(offer.Id, ct);
            }
            catch { /* best-effort */ }

            return await _med.Send(new OfferByIdQry { Id = offer.Id }, ct) ?? new OfferListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}
