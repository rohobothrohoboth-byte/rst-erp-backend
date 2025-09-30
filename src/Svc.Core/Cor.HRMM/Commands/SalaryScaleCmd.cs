using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using MediatR;

namespace Cor.HRMM.Commands;

public class SalaryScaleAddCmd : IRequest<SalaryScaleListDto> { public SalaryScaleAddDto AddDto { get; set; } = default!; }

public class SalaryScaleModCmd : IRequest<SalaryScaleListDto> { public SalaryScaleModDto ModDto { get; set; } = default!; }

public class SalaryScaleDelCmd : IRequest { public Guid Id { get; set; } }

public class SalaryScaleAddCmdHandler : IRequestHandler<SalaryScaleAddCmd, SalaryScaleListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public SalaryScaleAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<SalaryScaleListDto> Handle(SalaryScaleAddCmd request, CancellationToken cancellationToken)
    {
        var data = new SalaryScale
        {
            JobGradeId = request.AddDto.JobGradeId,
            Salary = request.AddDto.Salary
        };
        await _unitOfWork.Begin();

        try
        {
            await _unitOfWork.Repository<SalaryScale>().Add(data);
            await _unitOfWork.Commit();

            var res = new SalaryScaleListDto();
            var response = await _med.Send(new SalaryScaleByIdQry { Id = data.Id }, cancellationToken);
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

public class SalaryScaleModCmdHandler : IRequestHandler<SalaryScaleModCmd, SalaryScaleListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public SalaryScaleModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<SalaryScaleListDto> Handle(SalaryScaleModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<SalaryScale>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"SalaryScale with Id {request.ModDto.Id} NOT FOUND."); }

        oldData.JobGradeId = request.ModDto.JobGradeId;
        oldData.Salary = request.ModDto.Salary;
        await _unitOfWork.Begin();

        try
        {
            var data = await _unitOfWork.Repository<SalaryScale>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new SalaryScaleListDto();
            var response = await _med.Send(new SalaryScaleByIdQry { Id = data.Id }, cancellationToken);
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

public class SalaryScaleDelCmdHandler : IRequestHandler<SalaryScaleDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public SalaryScaleDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(SalaryScaleDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<SalaryScale>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}