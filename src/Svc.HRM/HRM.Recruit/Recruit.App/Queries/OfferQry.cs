using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class OfferAllQry : IRequest<List<OfferListDto>> { }
public class OfferByIdQry : IRequest<OfferListDto?> { public Guid Id { get; set; } }
public class OfferByApplicantQry : IRequest<List<OfferListDto>> { public Guid ApplicantId { get; set; } }
public class OfferByJobPostingQry : IRequest<List<OfferListDto>> { public Guid JobPostingId { get; set; } }

// Shared read/enrichment. Applicant name/email + posting number come from local joins
// (Recruit DB), so they work even when the Core-HRMM gRPC (position/department) is down.
internal static class OfferProjection
{
    public static async Task<List<OfferListDto>> Load(IUnitOfWork uow, Func<IQueryable<JobOffer>, IQueryable<JobOffer>> filter, CancellationToken ct)
    {
        var offers = await filter(uow.Set<JobOffer>().Where(x => !x.IsDeleted))
            .OrderByDescending(x => x.DateAdd)
            .ToListAsync(ct);
        if (offers.Count == 0) return new List<OfferListDto>();

        var applicantIds = offers.Select(o => o.ApplicantId).Distinct().ToList();
        var postingIds = offers.Select(o => o.JobPostingId).Distinct().ToList();

        var applicants = await uow.Set<Applicant>().Where(a => applicantIds.Contains(a.Id)).ToListAsync(ct);
        var personIds = applicants.Select(a => a.PersonId).Distinct().ToList();
        var contactIds = applicants.Select(a => a.ContactId).Distinct().ToList();
        var persons = await uow.Set<ApplicantPerson>().Where(p => personIds.Contains(p.Id)).ToListAsync(ct);
        var contacts = await uow.Set<ApplicantContact>().Where(c => contactIds.Contains(c.Id)).ToListAsync(ct);
        var postings = await uow.Set<JobPosting>().Where(jp => postingIds.Contains(jp.Id)).ToListAsync(ct);

        var applicantById = applicants.ToDictionary(a => a.Id);
        var personById = persons.ToDictionary(p => p.Id);
        var contactById = contacts.ToDictionary(c => c.Id);
        var postingById = postings.ToDictionary(jp => jp.Id);

        string F(DateTime d) => d == default ? "" : d.ToString("MMMM dd, yyyy");

        return offers.Select(o =>
        {
            applicantById.TryGetValue(o.ApplicantId, out var a);
            ApplicantPerson? per = a != null && personById.TryGetValue(a.PersonId, out var p) ? p : null;
            ApplicantContact? con = a != null && contactById.TryGetValue(a.ContactId, out var c) ? c : null;
            postingById.TryGetValue(o.JobPostingId, out var jp);

            return new OfferListDto
            {
                Id = o.Id,
                ApplicantId = o.ApplicantId,
                ApplicantName = per != null ? $"{per.FirstName} {per.MiddleName} {per.LastName}".Trim() : "",
                ApplicantEmail = con?.Email ?? "",
                JobPostingId = o.JobPostingId,
                JobPostingNumber = jp?.PostNumber ?? "",
                Position = "",
                Department = "",
                OfferDate = o.OfferDate,
                OfferDateStr = F(o.OfferDate),
                Salary = o.Salary,
                Currency = o.Currency,
                Benefits = o.Benefits ?? "",
                StartDate = o.StartDate,
                StartDateStr = F(o.StartDate),
                ExpiryDate = o.ExpirationDate,
                ExpiryDateStr = F(o.ExpirationDate),
                Status = o.Status,
                Notes = o.Notes,
                CreatedDate = o.DateAdd,
                RowVersion = o.xmin.ToString()
            };
        }).ToList();
    }
}

public class OfferAllHandler(IUnitOfWork uow) : IRequestHandler<OfferAllQry, List<OfferListDto>>
{
    public Task<List<OfferListDto>> Handle(OfferAllQry request, CancellationToken ct)
        => OfferProjection.Load(uow, q => q, ct);
}

public class OfferByIdHandler(IUnitOfWork uow) : IRequestHandler<OfferByIdQry, OfferListDto?>
{
    public async Task<OfferListDto?> Handle(OfferByIdQry request, CancellationToken ct)
    {
        var list = await OfferProjection.Load(uow, q => q.Where(x => x.Id == request.Id), ct);
        return list.FirstOrDefault();
    }
}

public class OfferByApplicantHandler(IUnitOfWork uow) : IRequestHandler<OfferByApplicantQry, List<OfferListDto>>
{
    public Task<List<OfferListDto>> Handle(OfferByApplicantQry request, CancellationToken ct)
        => OfferProjection.Load(uow, q => q.Where(x => x.ApplicantId == request.ApplicantId), ct);
}

public class OfferByJobPostingHandler(IUnitOfWork uow) : IRequestHandler<OfferByJobPostingQry, List<OfferListDto>>
{
    public Task<List<OfferListDto>> Handle(OfferByJobPostingQry request, CancellationToken ct)
        => OfferProjection.Load(uow, q => q.Where(x => x.JobPostingId == request.JobPostingId), ct);
}
