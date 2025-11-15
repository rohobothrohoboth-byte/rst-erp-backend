using Cor.HRMM.Helpers;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using MediatR;

namespace Cor.HRMM.Commands;

public class JobGradeAddCmd : IRequest<JobGradeListDto> { public JobGradeAddDto AddDto { get; set; } = default!; }
public class JobGradeModCmd : IRequest<JobGradeListDto> { public JobGradeModDto ModDto { get; set; } = default!; }
public class JobGradeDelCmd : IRequest { public Guid Id { get; set; } }

public class JobGradeAddCmdHandler : IRequestHandler<JobGradeAddCmd, JobGradeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobGradeAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobGradeListDto> Handle(JobGradeAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new JobGrade
            {
                Name = request.AddDto.Name,
                StartSalary = request.AddDto.StartSalary,
                MaxSalary = request.AddDto.MaxSalary
            };
            await _unitOfWork.Repository<JobGrade>().Add(data);
            await _unitOfWork.Commit();

            var res = new JobGradeListDto();
            var response = await _med.Send(new JobGradeByIdQry { Id = data.Id }, cancellationToken);
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

public class JobGradeModCmdHandler : IRequestHandler<JobGradeModCmd, JobGradeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobGradeModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobGradeListDto> Handle(JobGradeModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<JobGrade>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"JOB GRADE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Name = request.ModDto.Name;
            oldData.StartSalary = request.ModDto.StartSalary;
            oldData.MaxSalary = request.ModDto.MaxSalary;
            var data = await _unitOfWork.Repository<JobGrade>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new JobGradeListDto();
            var response = await _med.Send(new JobGradeByIdQry { Id = data.Id }, cancellationToken);
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

public class JobGradeDelCmdHandler : IRequestHandler<JobGradeDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobGradeDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(JobGradeDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<JobGrade>().GetById(request.Id);
            if (data == null) { throw new DomainException($"JOB GRADE with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<JobGrade>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}