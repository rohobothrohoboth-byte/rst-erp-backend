using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobAppIntAddCmd : IRequest<JobAppListDto> { public JobAppIntAddDto AddDto { get; set; } = default!; }
public class JobAppIntModCmd : IRequest<JobAppListDto> { public JobAppIntModDto ModDto { get; set; } = default!; }
//public class JobAppExtAddCmd : IRequest<JobAppListDto> { public JobAppExtAddDto AddDto { get; set; } = default!; }
//public class JobAppExtModCmd : IRequest<JobAppListDto> { public JobAppExtModDto ModDto { get; set; } = default!; }
public class JobAppDelCmd : IRequest { public Guid Id { get; set; } }



public class JobAppIntAddHandler : IRequestHandler<JobAppIntAddCmd, JobAppListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobAppIntAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobAppListDto> Handle(JobAppIntAddCmd request, CancellationToken ct)
    {
        if (request.AddDto.File == null) { throw new DomainException($"Application RESUME NOT UPLOADED."); }
        await _uow.Begin(ct);
        try
        {
            var jPost = await _uow.Set<JobPosting>().FirstOrDefaultAsync(x => x.Id == request.AddDto.JobPostingId, ct);
            var stat = BoolToStr.EnumToString(ApplicationStatus.Applied);
            var data = new JobApplication
            {
                Status = stat,
                PostType = jPost!.PostType,
                EmployeeId = request.AddDto.EmployeeId,
                AppliedDate = DateTime.UtcNow,
                JobPostingId = request.AddDto.JobPostingId
            };
            await _uow.Add(data, ct);

            var jAppId = data.Id;
            var cLetter = new CoverLetter
            {
                JobAppId = jAppId,
                Content = request.AddDto.CoverLetter
            };

            var mData = new Resume
            {
                FileName = request.AddDto.File.FileName,
                ContentType = request.AddDto.File.ContentType,
                FileSize = request.AddDto.File.Length,
                JobAppId = jAppId
            };
            await _uow.Add(mData,ct);

            using var ms = new MemoryStream();
            await request.AddDto.File.CopyToAsync(ms, ct);
            ms.Position = 0;
            var pBlob = new ResumeBlob
            {
                ResumeId = mData.Id,
                Data = ms.ToArray()
            };
            await _uow.Add(pBlob,ct);
            await _uow.Commit(ct);

            var res = new JobAppListDto();
            var response = await _med.Send(new JobAppByIdQry { Id = data.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
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

    public JobAppIntModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobAppListDto> Handle(JobAppIntModCmd request, CancellationToken ct)
    {
        if (request.ModDto.File == null) { throw new DomainException($"Application RESUME NOT UPLOADED."); }
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"JOB APPLICATION with Id {request.ModDto.Id} NOT FOUND."); }

            var stat = BoolToStr.EnumToString(ApplicationStatus.Applied);
            oldData.Status = stat;
            oldData.AppliedDate = DateTime.UtcNow;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);

            var cLetter = await _uow.Set<CoverLetter>().FirstOrDefaultAsync(c => c.JobAppId == request.ModDto.Id, ct);
            if (cLetter != null)
            {
                cLetter.Content = request.ModDto.CoverLetter;
                await _uow.Update(cLetter);
            }
            else
            {
                var newCLetter = new CoverLetter
                {
                    JobAppId = request.ModDto.Id,
                    Content = request.ModDto.CoverLetter
                };
                await _uow.Add(newCLetter, ct);
            }

            var mData = await _uow.Set<Resume>().FirstOrDefaultAsync(r => r.JobAppId == request.ModDto.Id, ct);
            if (mData != null)
            {
                mData.FileName = request.ModDto.File.FileName;
                mData.ContentType = request.ModDto.File.ContentType;
                mData.FileSize = request.ModDto.File.Length;
                await _uow.Update(mData);

                var pBlob = await _uow.Set<ResumeBlob>().FirstOrDefaultAsync(b => b.ResumeId == mData.Id, ct);
                if (pBlob != null)
                {
                    using var ms = new MemoryStream();
                    await request.ModDto.File.CopyToAsync(ms, ct);
                    ms.Position = 0;
                    pBlob.Data = ms.ToArray();
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
                        Data = ms.ToArray()
                    };
                    await _uow.Add(pBlobN, ct);
                }
            }
            else
            {
                var rData = new Resume
                {
                    FileName = request.ModDto.File.FileName,
                    ContentType = request.ModDto.File.ContentType,
                    FileSize = request.ModDto.File.Length,
                    JobAppId = request.ModDto.Id
                };
                await _uow.Add(rData, ct);

                using var ms = new MemoryStream();
                await request.ModDto.File.CopyToAsync(ms, ct);
                ms.Position = 0;
                var pBlob = new ResumeBlob
                {
                    ResumeId = rData.Id,
                    Data = ms.ToArray()
                };
                await _uow.Add(pBlob, ct);
            }

            await _uow.Commit(ct);
            var res = new JobAppListDto();
            var response = await _med.Send(new JobAppByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobAppDelHandler : IRequestHandler<JobAppDelCmd>
{
    private readonly IUnitOfWork _uow;
    public JobAppDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(JobAppDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"JOB APPLICATION with id [{request.Id}] NOT FOUND."); }
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