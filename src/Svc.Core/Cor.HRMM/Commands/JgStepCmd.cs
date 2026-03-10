using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class JgStepAddCmd : IRequest<JgStepListDto> { public JgStepAddDto AddDto { get; set; } = default!; }
public class JgStepModCmd : IRequest<JgStepListDto> { public JgStepModDto ModDto { get; set; } = default!; }
public class JgStepDelCmd : IRequest { public Guid Id { get; set; } }



public class JgStepAddHandler : IRequestHandler<JgStepAddCmd, JgStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public JgStepAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<JgStepListDto> Handle(JgStepAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new JgStep
            {
                JobGradeId = request.AddDto.JobGradeId,
                Name = request.AddDto.Name,
                Salary = request.AddDto.Salary
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new JgStepListDto();
            var response = await _med.Send(new JgStepByIdQry { Id = data.Id }, ct);
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

public class JgStepModHandler : IRequestHandler<JgStepModCmd, JgStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public JgStepModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<JgStepListDto> Handle(JgStepModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JgStep>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, cancellationToken: ct);
            if (oldData == null) { throw new DomainException($"JOB GRADE STEP with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.JobGradeId = request.ModDto.JobGradeId;
            oldData.Name = request.ModDto.Name;
            oldData.Salary = request.ModDto.Salary;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new JgStepListDto();
            var response = await _med.Send(new JgStepByIdQry { Id = request.ModDto.Id }, ct);
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

public class JgStepDelCmdHandler : IRequestHandler<JgStepDelCmd>
{
    private readonly IUnitOfWork _uow;
    public JgStepDelCmdHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(JgStepDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JgStep>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"JOB GRADE STEP with id [{request.Id}] NOT FOUND."); }
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