using Helpers;
using MediatR;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobAppIntAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobAppListDto> Handle(JobAppIntAddCmd request, CancellationToken cancellationToken)
    {
        if (request.AddDto.File == null) { throw new DomainException($"Application RESUME NOT UPLOADED."); }

        await _unitOfWork.Begin();
        try
        {
            var jPost = await _unitOfWork.Repository<JobPosting>().GetById(request.AddDto.JobPostingId);
            var stat = BoolToStr.EnumToString(ApplicationStatus.Applied);
            var data = new JobApplication
            {
                Status = stat,
                PostType = jPost!.PostType,
                EmployeeId = request.AddDto.EmployeeId,
                AppliedDate = DateTime.UtcNow,
                JobPostingId = request.AddDto.JobPostingId
            };
            await _unitOfWork.Repository<JobApplication>().Add(data);

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
            await _unitOfWork.Repository<Resume>().Add(mData);

            using var ms = new MemoryStream();
            await request.AddDto.File.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            var pBlob = new ResumeBlob
            {
                ResumeId = mData.Id,
                Data = ms.ToArray()
            };
            await _unitOfWork.Repository<ResumeBlob>().Add(pBlob);
            await _unitOfWork.Commit();

            var res = new JobAppListDto();
            var response = await _med.Send(new JobAppByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobAppIntModHandler : IRequestHandler<JobAppIntModCmd, JobAppListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobAppIntModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobAppListDto> Handle(JobAppIntModCmd request, CancellationToken cancellationToken)
    {
        if (request.ModDto.File == null) { throw new DomainException($"Application RESUME NOT UPLOADED."); }
        var oldData = await _unitOfWork.Repository<JobApplication>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"JOB APPLICATION with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            var stat = BoolToStr.EnumToString(ApplicationStatus.Applied);
            oldData.Status = stat;
            oldData.AppliedDate = DateTime.UtcNow;
            var data = await _unitOfWork.Repository<JobApplication>().Update(oldData);

            var cLetter = await _unitOfWork.Repository<CoverLetter>().GetFoD(c => c.JobAppId == request.ModDto.Id);
            if (cLetter != null)
            {
                cLetter.Content = request.ModDto.CoverLetter;
                await _unitOfWork.Repository<CoverLetter>().Update(cLetter);
            }
            else
            {
                var newCLetter = new CoverLetter
                {
                    JobAppId = request.ModDto.Id,
                    Content = request.ModDto.CoverLetter
                };
                await _unitOfWork.Repository<CoverLetter>().Add(newCLetter);
            }

            var mData = await _unitOfWork.Repository<Resume>().GetFoD(r => r.JobAppId == request.ModDto.Id);
            if (mData != null)
            {
                mData.FileName = request.ModDto.File.FileName;
                mData.ContentType = request.ModDto.File.ContentType;
                mData.FileSize = request.ModDto.File.Length;
                await _unitOfWork.Repository<Resume>().Update(mData);

                var pBlob = await _unitOfWork.Repository<ResumeBlob>().GetFoD(b => b.ResumeId == mData.Id);
                if (pBlob != null)
                {
                    using var ms = new MemoryStream();
                    await request.ModDto.File.CopyToAsync(ms, cancellationToken);
                    ms.Position = 0;
                    pBlob.Data = ms.ToArray();
                    await _unitOfWork.Repository<ResumeBlob>().Update(pBlob);
                }
                else
                {
                    using var ms = new MemoryStream();
                    await request.ModDto.File.CopyToAsync(ms, cancellationToken);
                    ms.Position = 0;
                    var pBlobN = new ResumeBlob
                    {
                        ResumeId = mData.Id,
                        Data = ms.ToArray()
                    };
                    await _unitOfWork.Repository<ResumeBlob>().Add(pBlobN);
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
                await _unitOfWork.Repository<Resume>().Add(rData);

                using var ms = new MemoryStream();
                await request.ModDto.File.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;
                var pBlob = new ResumeBlob
                {
                    ResumeId = rData.Id,
                    Data = ms.ToArray()
                };
                await _unitOfWork.Repository<ResumeBlob>().Add(pBlob);
            }

            await _unitOfWork.Commit();

            var res = new JobAppListDto();
            var response = await _med.Send(new JobAppByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobAppDelHandler : IRequestHandler<JobAppDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobAppDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(JobAppDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<JobApplication>().GetById(request.Id);
            if (data == null) { throw new DomainException($"JOB APPLICATION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<JobApplication>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}