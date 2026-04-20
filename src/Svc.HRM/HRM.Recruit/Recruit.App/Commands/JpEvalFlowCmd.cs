using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JpEvalFlowAddCmd : IRequest<JpEvalFlowListDto> { public JpEvalFlowAddDto AddDto { get; set; } = default!; }
public class JpEvalFlowModCmd : IRequest<JpEvalFlowListDto> { public JpEvalFlowModDto ModDto { get; set; } = default!; }
public class JpEvalFlowDelCmd : IRequest { public Guid Id { get; set; } = default!; }



public class JpJpEvalFlowAddHandler : IRequestHandler<JpEvalFlowAddCmd, JpEvalFlowListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JpJpEvalFlowAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JpEvalFlowListDto> Handle(JpEvalFlowAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new JobPostEvalFlow
            {
                JobPostingId = request.AddDto.JobPostingId,
                EvaluationFlowId = request.AddDto.EvaluationFlowId,
                EffectiveFrom = request.AddDto.EffectiveFrom
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new JpEvalFlowListDto();
            var response = await _med.Send(new JpEvalFlowByIdQry { Id = data.Id }, ct);
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

public class JpEvalFlowModHandler : IRequestHandler<JpEvalFlowModCmd, JpEvalFlowListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JpEvalFlowModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JpEvalFlowListDto> Handle(JpEvalFlowModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobPostEvalFlow>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"JOB POST'S EVALUATION FLOW with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.EvaluationFlowId = request.ModDto.EvaluationFlowId;
            oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new JpEvalFlowListDto();
            var response = await _med.Send(new JpEvalFlowByIdQry { Id = request.ModDto.Id }, ct);
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

public class JpEvalFlowDelHandler : IRequestHandler<JpEvalFlowDelCmd>
{
    private readonly IUnitOfWork _uow;
    public JpEvalFlowDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(JpEvalFlowDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JobPostEvalFlow>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"JOB POST'S EVALUATION FLOW with id [{request.Id}] NOT FOUND."); }
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