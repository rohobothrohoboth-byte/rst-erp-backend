using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.HRMM.Commands;

public class JgStepAddCmd : IRequest<JgStepListDto> { public JgStepAddDto AddDto { get; set; } = default!; }
public class JgStepModCmd : IRequest<JgStepListDto> { public JgStepModDto ModDto { get; set; } = default!; }
public class JgStepDelCmd : IRequest { public Guid Id { get; set; } }

public class JgStepAddHandler : IRequestHandler<JgStepAddCmd, JgStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JgStepAddHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<JgStepListDto> Handle(JgStepAddCmd request, CancellationToken ct)
    {
        JgStep data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = new JgStep
            {
                JobGradeId = request.AddDto.JobGradeId,
                Name = request.AddDto.Name,
                Salary = request.AddDto.Salary,
                Currency = request.AddDto.Currency,
                SalaryPayFreq = request.AddDto.SalaryPayFreq
            };
            await _uow.AddAsync(data, token);
        }, ct: ct);

        var response = await _med.Send(new JgStepByIdQry { Id = data.Id }, ct);
        return response ?? new JgStepListDto();
    }
}

public class JgStepModHandler : IRequestHandler<JgStepModCmd, JgStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JgStepModHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<JgStepListDto> Handle(JgStepModCmd request, CancellationToken ct)
    {
        JgStep? oldData = null!;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<JgStep>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException($"JOB GRADE STEP with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.JobGradeId = request.ModDto.JobGradeId;
            oldData.Name = request.ModDto.Name;
            oldData.Salary = request.ModDto.Salary;
            oldData.Currency = request.ModDto.Currency;
            oldData.SalaryPayFreq = request.ModDto.SalaryPayFreq;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            _uow.Update(oldData);
        }, ct: ct);

        var response = await _med.Send(new JgStepByIdQry { Id = request.ModDto.Id }, ct);
        return response ?? new JgStepListDto();
    }
}

public class JgStepDelCmdHandler : IRequestHandler<JgStepDelCmd>
{
    private readonly IUnitOfWork _uow;

    public JgStepDelCmdHandler(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task Handle(JgStepDelCmd request, CancellationToken ct)
    {
        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<JgStep>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException($"JOB GRADE STEP with id [{request.Id}] NOT FOUND.");
            }

            _uow.Delete(data);
        }, ct: ct);
    }
}