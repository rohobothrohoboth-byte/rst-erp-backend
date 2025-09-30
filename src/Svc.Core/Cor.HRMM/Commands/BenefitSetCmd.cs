using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using MediatR;

namespace Cor.HRMM.Commands;

public class BenefitSetAddCmd : IRequest<BenefitSetListDto> { public BenefitSetAddDto AddDto { get; set; } = default!; }

public class BenefitSetModCmd : IRequest<BenefitSetListDto> { public BenefitSetModDto ModDto { get; set; } = default!; }

public class BenefitSetDelCmd : IRequest { public Guid Id { get; set; } }

public class BenefitSetAddCmdHandler : IRequestHandler<BenefitSetAddCmd, BenefitSetListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public BenefitSetAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<BenefitSetListDto> Handle(BenefitSetAddCmd request, CancellationToken cancellationToken)
    {
        var data = new BenefitSetting
        {
            Name = request.AddDto.Name,
            BenefitValue = request.AddDto.BenefitValue
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<BenefitSetting>().Add(data);
            await _unitOfWork.Commit();

            var res = new BenefitSetListDto();
            var response = await _med.Send(new BenefitSetByIdQry { Id = data.Id }, cancellationToken);
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

public class BenefitSetModCmdHandler : IRequestHandler<BenefitSetModCmd, BenefitSetListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public BenefitSetModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<BenefitSetListDto> Handle(BenefitSetModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<BenefitSetting>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"BENEFIT SETTING with Id {request.ModDto.Id} NOT FOUND."); }

        oldData.Name = request.ModDto.Name;
        oldData.BenefitValue = request.ModDto.BenefitValue;

        await _unitOfWork.Begin();

        try
        {
            var data = await _unitOfWork.Repository<BenefitSetting>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new BenefitSetListDto();
            var response = await _med.Send(new BenefitSetByIdQry { Id = data.Id }, cancellationToken);
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

public class BenefitSetDelCmdHandler : IRequestHandler<BenefitSetDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public BenefitSetDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(BenefitSetDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<BenefitSetting>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}