using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;

namespace Cor.HRMM.Commands;

public class JgStepAddCmd : IRequest<JgStepListDto> { public JgStepAddDto AddDto { get; set; } = default!; }
public class JgStepModCmd : IRequest<JgStepListDto> { public JgStepModDto ModDto { get; set; } = default!; }
public class JgStepDelCmd : IRequest { public Guid Id { get; set; } }

public class JgStepAddCmdHandler : IRequestHandler<JgStepAddCmd, JgStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JgStepAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JgStepListDto> Handle(JgStepAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new JgStep
            {
                JobGradeId = request.AddDto.JobGradeId,
                Name = request.AddDto.Name,
                Salary = request.AddDto.Salary
            };
            await _unitOfWork.Repository<JgStep>().Add(data);
            await _unitOfWork.Commit();

            var res = new JgStepListDto();
            var response = await _med.Send(new JgStepByIdQry { Id = data.Id }, cancellationToken);
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

public class JgStepModCmdHandler : IRequestHandler<JgStepModCmd, JgStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JgStepModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JgStepListDto> Handle(JgStepModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<JgStep>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"JOB GRADE STEP with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.JobGradeId = request.ModDto.JobGradeId;
            oldData.Name = request.ModDto.Name;
            oldData.Salary = request.ModDto.Salary;
            var data = await _unitOfWork.Repository<JgStep>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new JgStepListDto();
            var response = await _med.Send(new JgStepByIdQry { Id = data.Id }, cancellationToken);
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

public class JgStepDelCmdHandler : IRequestHandler<JgStepDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public JgStepDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(JgStepDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<JgStep>().GetById(request.Id);
            if (data == null) { throw new DomainException($"JOB GRADE STEP with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<JgStep>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}