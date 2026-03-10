using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class JobGradeAddCmd : IRequest<JobGradeListDto> { public JobGradeAddDto AddDto { get; set; } = default!; }
public class JobGradeModCmd : IRequest<JobGradeListDto> { public JobGradeModDto ModDto { get; set; } = default!; }
public class JobGradeDelCmd : IRequest { public Guid Id { get; set; } }



public class JobGradeAddHandler : IRequestHandler<JobGradeAddCmd, JobGradeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobGradeAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<JobGradeListDto> Handle(JobGradeAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new JobGrade
            {
                Name = request.AddDto.Name,
                StartSalary = request.AddDto.StartSalary,
                MaxSalary = request.AddDto.MaxSalary
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new JobGradeListDto();
            var response = await _med.Send(new JobGradeByIdQry { Id = data.Id }, ct);
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

public class JobGradeModHandler : IRequestHandler<JobGradeModCmd, JobGradeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public JobGradeModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<JobGradeListDto> Handle(JobGradeModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobGrade>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, cancellationToken: ct);
            if (oldData == null) { throw new DomainException($"JOB GRADE with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.StartSalary = request.ModDto.StartSalary;
            oldData.MaxSalary = request.ModDto.MaxSalary;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new JobGradeListDto();
            var response = await _med.Send(new JobGradeByIdQry { Id = request.ModDto.Id }, ct);
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

public class JobGradeDelHandler : IRequestHandler<JobGradeDelCmd>
{
    private readonly IUnitOfWork _uow;
    public JobGradeDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(JobGradeDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JobGrade>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"JOB GRADE with id [{request.Id}] NOT FOUND."); }
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