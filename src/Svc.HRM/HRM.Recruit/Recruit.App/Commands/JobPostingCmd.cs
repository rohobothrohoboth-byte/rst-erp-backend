using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobPostingAddCmd : IRequest<JobPostingListDto>
{
    public JobPostingAddDto AddDto { get; set; } = default!;
}

public class JobPostingAddAllCmd : IRequest<List<JobPostingListDto>>
{
    public JobPostingAddDto AddDto { get; set; } = default!;
}

public class JobPostingModCmd : IRequest<JobPostingListDto>
{
    public JobPostingModDto ModDto { get; set; } = default!;
}

public class JobPostingDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ? Helper class for mapping
public static class JobPostingMapping
{
    public static JobPostingListDto MapViewToListDto(JobPostingViewDto viewDto)
    {
        if (viewDto == null) return new JobPostingListDto();

        // Parse status for proper display using Helpers service
        var status = JobPostingStatusService.ParseStatus(viewDto.Status);
        var statusDisplay = JobPostingStatusService.GetDisplayName(status);

        return new JobPostingListDto
        {
            Id = viewDto.Id,
            PostNumber = viewDto.PostNumber,
            ReqNumber = viewDto.ReqNumber,
            Status = viewDto.Status,  // Raw status value
            StatusStr = statusDisplay,  // Formatted display name
            PostType = viewDto.PostType,
            PostTypeStr = viewDto.PostTypeStr,
            ReqAppQuan = $"{viewDto.ReqQuantity ?? 0} | {viewDto.AppQuantity ?? 0}",
            IsDeleted = viewDto.IsDeleted,
            RowVersion = viewDto.RowVersion,
        };
    }
}

public class JobPostingAddHandler : IRequestHandler<JobPostingAddCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobPostingListDto> Handle(JobPostingAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // ? Check if posting already exists for this requisition
            var existingPosting = await _uow.Set<JobPosting>()
                .FirstOrDefaultAsync(x => x.JobReqId == request.AddDto.Id && !x.IsDeleted, ct);

            if (existingPosting != null)
                throw new DomainException("A job posting already exists for this requisition.");

            // ? Generate PostNumber
            var postNumber = await GeneratePostNumber(ct);

            var data = new JobPosting
            {
                Id = Guid.CreateVersion7(),
                PostNumber = postNumber,
                Status = JobPostingStatusService.Draft,  // ? Start as Draft
                PostType = request.AddDto.PostType,
                PublishedDate = DateTime.UtcNow,
                DeadlineDate = request.AddDto.DeadlineDate,
                JobReqId = request.AddDto.Id,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false,
                ClosedDate = null
            };

            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            // ? Use JobPostingViewQry instead of JobPostingByIdQry
            var response = await _med.Send(new JobPostingViewQry { Id = data.Id }, ct);
            if (response == null) { return new JobPostingListDto(); }

            // ? Map ViewDto to ListDto
            return JobPostingMapping.MapViewToListDto(response);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    // ? Helper method to generate PostNumber
    private async Task<string> GeneratePostNumber(CancellationToken ct)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _uow.Set<JobPosting>()
            .Where(x => x.PostNumber.StartsWith($"JOB-{year}-"))
            .CountAsync(ct);
        var number = (count + 1).ToString("D3");
        return $"JOB-{year}-{number}";
    }
}

public class JobPostingAddAllHandler : IRequestHandler<JobPostingAddAllCmd, List<JobPostingListDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingAddAllHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<List<JobPostingListDto>> Handle(JobPostingAddAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Approved);
            var jReqL = await _uow.Set<JobRequisition>()
                .Where(r => r.WorkforcePlanId == request.AddDto.Id && r.Status == stat && !r.IsDeleted)
                .ToListAsync(ct);

            if (jReqL.Count <= 0)
                throw new DomainException($"APPROVED JOB REQUISITIONS NOT FOUND for selected work force plan.");

            var resultList = new List<JobPostingListDto>();

            foreach (var jReq in jReqL)
            {
                // ? Check if posting already exists
                var existingPosting = await _uow.Set<JobPosting>()
                    .FirstOrDefaultAsync(x => x.JobReqId == jReq.Id && !x.IsDeleted, ct);

                if (existingPosting != null) continue;

                // ? Generate PostNumber for each
                var postNumber = await GeneratePostNumber(ct);

                var data = new JobPosting
                {
                    Id = Guid.CreateVersion7(),
                    PostNumber = postNumber,
                    Status = JobPostingStatusService.Draft,  // ? Start as Draft
                    PostType = request.AddDto.PostType,
                    PublishedDate = DateTime.UtcNow,
                    DeadlineDate = request.AddDto.DeadlineDate,
                    JobReqId = jReq.Id,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false,
                    ClosedDate = null
                };

                await _uow.Add(data, ct);

                // ? Get the created posting as ListDto
                var response = await _med.Send(new JobPostingViewQry { Id = data.Id }, ct);
                if (response != null)
                {
                    resultList.Add(JobPostingMapping.MapViewToListDto(response));
                }
            }

            await _uow.Commit(ct);
            return resultList;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    // ? Helper method to generate PostNumber
    private async Task<string> GeneratePostNumber(CancellationToken ct)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _uow.Set<JobPosting>()
            .Where(x => x.PostNumber.StartsWith($"JOB-{year}-"))
            .CountAsync(ct);
        var number = (count + 1).ToString("D3");
        return $"JOB-{year}-{number}";
    }
}

public class JobPostingModHandler : IRequestHandler<JobPostingModCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobPostingListDto> Handle(JobPostingModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobPosting>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && !x.IsDeleted, ct);

            if (oldData == null)
                throw new DomainException($"JOB POSTING with Id {request.ModDto.Id} NOT FOUND.");

            // ? Validate if modification is allowed using Helpers service
            JobPostingStatusService.ValidateModify(oldData.Status);

            // ? Handle status update if provided
            if (!string.IsNullOrEmpty(request.ModDto.Status))
            {
                var newStatus = JobPostingStatusService.ParseStatus(request.ModDto.Status);

                // ? Prevent setting to Published or Closed through modification endpoint
                if (newStatus == PostingStatus.Published)
                {
                    throw new DomainException(
                        "Cannot set status to 'Published' through modification endpoint. " +
                        "Use the Publish endpoint instead."
                    );
                }

                if (newStatus == PostingStatus.Closed)
                {
                    throw new DomainException(
                        "Cannot set status to 'Closed' through modification endpoint. " +
                        "Use the Close endpoint instead."
                    );
                }

                // ? Validate transition using Helpers service
                JobPostingStatusService.ValidateTransition(oldData.Status, request.ModDto.Status);
                oldData.Status = newStatus.ToString();
            }

            // ? Update other fields
            oldData.PostType = request.ModDto.PostType;
            oldData.DeadlineDate = request.ModDto.DeadlineDate;
            oldData.DateMod = DateTime.UtcNow;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

            await _uow.Update(oldData);
            await _uow.Commit(ct);

            // ? Use JobPostingViewQry instead of JobPostingByIdQry
            var response = await _med.Send(new JobPostingViewQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return new JobPostingListDto(); }

            // ? Map ViewDto to ListDto
            return JobPostingMapping.MapViewToListDto(response);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobPostingDelHandler : IRequestHandler<JobPostingDelCmd>
{
    private readonly IUnitOfWork _uow;

    public JobPostingDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(JobPostingDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JobPosting>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (data == null)
                throw new DomainException($"JOB POSTING with id [{request.Id}] NOT FOUND.");

            // ? Validate if can be deleted using Helpers service
            JobPostingStatusService.ValidateDelete(data.Status);

            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}