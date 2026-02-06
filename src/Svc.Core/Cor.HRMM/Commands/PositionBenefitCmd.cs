using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;

namespace Cor.HRMM.Commands;

public class PositionBenefitAddCmd : IRequest<PositionBenefitListDto> { public PositionBenefitAddDto AddDto { get; set; } = default!; }
public class PositionBenefitModCmd : IRequest<PositionBenefitListDto> { public PositionBenefitModDto ModDto { get; set; } = default!; }
public class PositionBenefitDelCmd : IRequest { public Guid Id { get; set; } }

public class PositionBenefitAddCmdHandler : IRequestHandler<PositionBenefitAddCmd, PositionBenefitListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionBenefitAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionBenefitListDto> Handle(PositionBenefitAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new PositionBenefit
            {
                BenefitSettingId = request.AddDto.BenefitSettingId,
                PositionId = request.AddDto.PositionId
            };
            await _unitOfWork.Repository<PositionBenefit>().Add(data);
            await _unitOfWork.Commit();

            var res = new PositionBenefitListDto();
            var response = await _med.Send(new PositionBenefitByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionBenefitModCmdHandler : IRequestHandler<PositionBenefitModCmd, PositionBenefitListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionBenefitModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionBenefitListDto> Handle(PositionBenefitModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PositionBenefit>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"POSITION BENEFIT with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.BenefitSettingId = request.ModDto.BenefitSettingId;
            oldData.PositionId = request.ModDto.PositionId;
            var data = await _unitOfWork.Repository<PositionBenefit>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PositionBenefitListDto();
            var response = await _med.Send(new PositionBenefitByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionBenefitDelCmdHandler : IRequestHandler<PositionBenefitDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionBenefitDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PositionBenefitDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<PositionBenefit>().GetById(request.Id);
            if (data == null) { throw new DomainException($"POSITION BENEFIT with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<PositionBenefit>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}