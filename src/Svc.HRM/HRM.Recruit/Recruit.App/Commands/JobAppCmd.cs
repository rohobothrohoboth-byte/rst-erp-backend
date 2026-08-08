// Recruit.App/Commands/ApplicantCmd.cs

using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;
using Recruit.App.Services;
namespace Recruit.App.Commands;

// ============================================
// COMMANDS
// ============================================

public class JobAppIntAddCmd : IRequest<JobAppListDto>
{
    public JobAppIntAddDto AddDto { get; set; } = default!;
}

public class JobAppIntModCmd : IRequest<JobAppListDto>
{
    public JobAppIntModDto ModDto { get; set; } = default!;
}

public class JobAppDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class UpdateApplicantStatusCmd : IRequest<JobAppListDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public string? Reason { get; set; }
}


// ============================================
// HANDLERS
// ============================================

// --------------------------------------------
// JobAppIntAddHandler - Create Internal Application
// --------------------------------------------
public class JobAppIntAddHandler : IRequestHandler<JobAppIntAddCmd, JobAppListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobAppIntAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobAppListDto> Handle(JobAppIntAddCmd request, CancellationToken ct)
    {
        if (request.AddDto.File == null)
        {
            throw new DomainException("Application RESUME NOT UPLOADED.");
        }

        await _uow.Begin(ct);
        try
        {
            // Get Job Posting
            var jPost = await _uow.Set<JobPosting>()
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.JobPostingId && !x.IsDeleted, ct);

            if (jPost == null)
                throw new DomainException($"Job Posting with Id {request.AddDto.JobPostingId} NOT FOUND.");

            // Create Job Application
            var stat = BoolToStr.EnumToString(ApplicationStatus.Applied);
            var data = new JobApplication
            {
                Status = stat,
                PostType = jPost.PostType,
                EmployeeId = request.AddDto.EmployeeId,
                AppliedDate = DateTime.UtcNow,
                JobPostingId = request.AddDto.JobPostingId,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(data, ct);

            var jAppId = data.Id;

            // Create Cover Letter
            var cLetter = new CoverLetter
            {
                JobAppId = jAppId,
                Content = request.AddDto.CoverLetter,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(cLetter, ct);

            // Create Resume
            var mData = new Resume
            {
                FileName = request.AddDto.File.FileName,
                ContentType = request.AddDto.File.ContentType,
                FileSize = request.AddDto.File.Length,
                JobAppId = jAppId,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(mData, ct);

            // Create Resume Blob
            using var ms = new MemoryStream();
            await request.AddDto.File.CopyToAsync(ms, ct);
            ms.Position = 0;
            var pBlob = new ResumeBlob
            {
                ResumeId = mData.Id,
                Data = ms.ToArray(),
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(pBlob, ct);

            await _uow.Commit(ct);

            // Get the created application
            var response = await _med.Send(new JobAppByIdQry { Id = data.Id }, ct);
            return response ?? new JobAppListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobAppIntModHandler : IRequestHandler<JobAppIntModCmd, JobAppListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobAppIntModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobAppListDto> Handle(JobAppIntModCmd request, CancellationToken ct)
    {
        if (request.ModDto.File == null)
        {
            throw new DomainException("Application RESUME NOT UPLOADED.");
        }

        await _uow.Begin(ct);
        try
        {
            // Find existing Job Application
            var oldData = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && !x.IsDeleted, ct);

            if (oldData == null)
            {
                throw new DomainException($"JOB APPLICATION with Id {request.ModDto.Id} NOT FOUND.");
            }

            // Update Job Application
            var stat = BoolToStr.EnumToString(ApplicationStatus.Applied);
            oldData.Status = stat;
            oldData.AppliedDate = DateTime.UtcNow;
            oldData.DateMod = DateTime.UtcNow;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);

            // Update Cover Letter
            var cLetter = await _uow.Set<CoverLetter>()
                .FirstOrDefaultAsync(c => c.JobAppId == request.ModDto.Id && !c.IsDeleted, ct);

            if (cLetter != null)
            {
                cLetter.Content = request.ModDto.CoverLetter;
                cLetter.DateMod = DateTime.UtcNow;
                await _uow.Update(cLetter);
            }
            else
            {
                var newCLetter = new CoverLetter
                {
                    JobAppId = request.ModDto.Id,
                    Content = request.ModDto.CoverLetter,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };
                await _uow.Add(newCLetter, ct);
            }

            // Update Resume
            var mData = await _uow.Set<Resume>()
                .FirstOrDefaultAsync(r => r.JobAppId == request.ModDto.Id && !r.IsDeleted, ct);

            if (mData != null)
            {
                mData.FileName = request.ModDto.File.FileName;
                mData.ContentType = request.ModDto.File.ContentType;
                mData.FileSize = request.ModDto.File.Length;
                mData.DateMod = DateTime.UtcNow;
                await _uow.Update(mData);

                // Update Resume Blob
                var pBlob = await _uow.Set<ResumeBlob>()
                    .FirstOrDefaultAsync(b => b.ResumeId == mData.Id && !b.IsDeleted, ct);

                if (pBlob != null)
                {
                    using var ms = new MemoryStream();
                    await request.ModDto.File.CopyToAsync(ms, ct);
                    ms.Position = 0;
                    pBlob.Data = ms.ToArray();
                    pBlob.DateMod = DateTime.UtcNow;
                    await _uow.Update(pBlob);
                }
                else
                {
                    using var ms = new MemoryStream();
                    await request.ModDto.File.CopyToAsync(ms, ct);
                    ms.Position = 0;
                    var pBlobN = new ResumeBlob
                    {
                        ResumeId = mData.Id,
                        Data = ms.ToArray(),
                        DateAdd = DateTime.UtcNow,
                        DateMod = null,
                        IsDeleted = false
                    };
                    await _uow.Add(pBlobN, ct);
                }
            }
            else
            {
                // Create new Resume
                var rData = new Resume
                {
                    FileName = request.ModDto.File.FileName,
                    ContentType = request.ModDto.File.ContentType,
                    FileSize = request.ModDto.File.Length,
                    JobAppId = request.ModDto.Id,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };
                await _uow.Add(rData, ct);

                // Create new Resume Blob
                using var ms = new MemoryStream();
                await request.ModDto.File.CopyToAsync(ms, ct);
                ms.Position = 0;
                var pBlob = new ResumeBlob
                {
                    ResumeId = rData.Id,
                    Data = ms.ToArray(),
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };
                await _uow.Add(pBlob, ct);
            }

            await _uow.Commit(ct);

            // Get the updated application
            var response = await _med.Send(new JobAppByIdQry { Id = request.ModDto.Id }, ct);
            return response ?? new JobAppListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// --------------------------------------------
// JobAppDelHandler - Delete Application
// --------------------------------------------
public class JobAppDelHandler : IRequestHandler<JobAppDelCmd>
{
    private readonly IUnitOfWork _uow;

    public JobAppDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(JobAppDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (data == null)
            {
                throw new DomainException($"JOB APPLICATION with id [{request.Id}] NOT FOUND.");
            }

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
// Recruit.App/Commands/ApplicantCmd.cs - UpdateApplicantStatusHandler
// Recruit.App/Commands/ApplicantCmd.cs - Updated UpdateApplicantStatusHandler

public class UpdateApplicantStatusHandler : IRequestHandler<UpdateApplicantStatusCmd, JobAppListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IRecruitNotificationService _notificationService;

    public UpdateApplicantStatusHandler(
        IUnitOfWork uow,
        IMediator med,
        IRecruitNotificationService notificationService)
    {
        _uow = uow;
        _med = med;
        _notificationService = notificationService;
    }

    public async Task<JobAppListDto> Handle(UpdateApplicantStatusCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jobApp = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (jobApp == null)
                throw new DomainException($"Job application with Id {request.Id} NOT FOUND.");

            var oldStatus = jobApp.Status;

            // Update status
            jobApp.Status = request.Status;
            jobApp.DateMod = DateTime.UtcNow;
         //   jobApp.Reason = request.Reason;

            await _uow.Update(jobApp);
            await _uow.Commit(ct);

            // ? Send notification
            await _notificationService.NotifyStatusChangeAsync(jobApp.Id, oldStatus, request.Status, ct);

            // Get updated applicant data
            var response = await _med.Send(new JobAppByIdQry { Id = request.Id }, ct);
            return response ?? new JobAppListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}