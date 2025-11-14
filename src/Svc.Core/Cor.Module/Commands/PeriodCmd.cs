using Cor.Module.Helpers;
using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using MediatR;

namespace Cor.Module.Commands;

public class AddPeriodCmd : IRequest<PeriodListDto> { public AddPeriodDto AddPeriodDto { get; set; } = default!; }
public class ModPeriodCmd : IRequest<PeriodListDto> { public EditPeriodDto EditPeriodDto { get; set; } = default!; }
public class DelPeriodCmd : IRequest { public Guid Id { get; set; } }

public class AddPeriodCmdHandler : IRequestHandler<AddPeriodCmd, PeriodListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddPeriodCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PeriodListDto> Handle(AddPeriodCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var period = new Period
            {
                Name = request.AddPeriodDto.Name,
                DateStart = request.AddPeriodDto.DateStart,
                DateEnd = request.AddPeriodDto.DateEnd,
                IsActive = "0",
                Quarter = request.AddPeriodDto.Quarter,
                FiscalYearId = request.AddPeriodDto.FiscalYearId
            };
            await _unitOfWork.Repository<Period>().Add(period);
            await _unitOfWork.Commit();

            var res = new PeriodListDto();
            var response = await _med.Send(new PeriodByIdQry { Id = period.Id }, cancellationToken);
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

public class ModPeriodCmdHandler : IRequestHandler<ModPeriodCmd, PeriodListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ModPeriodCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PeriodListDto> Handle(ModPeriodCmd request, CancellationToken cancellationToken)
    {
        var oldPeriod = await _unitOfWork.Repository<Period>().GetById(request.EditPeriodDto.Id);
        if (oldPeriod == null) { throw new DomainException($"PERIOD with id [{request.EditPeriodDto.Id}] NOT FOUND."); }
        
        await _unitOfWork.Begin();
        try
        {
            oldPeriod.Name = request.EditPeriodDto.Name;
            oldPeriod.DateStart = request.EditPeriodDto.DateStart;
            oldPeriod.DateEnd = request.EditPeriodDto.DateEnd;
            oldPeriod.IsActive = request.EditPeriodDto.IsActive;
            oldPeriod.Quarter = request.EditPeriodDto.Quarter;
            oldPeriod.FiscalYearId = request.EditPeriodDto.FiscalYearId;
            var nPeriod = await _unitOfWork.Repository<Period>().Update(oldPeriod);
            await _unitOfWork.Commit();

            var res = new PeriodListDto();
            var response = await _med.Send(new PeriodByIdQry { Id = nPeriod.Id }, cancellationToken);
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

public class DelPeriodCmdHandler : IRequestHandler<DelPeriodCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelPeriodCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelPeriodCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<Holiday>().GetById(request.Id);
            if (data == null) { throw new DomainException($"PERIOD with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<Period>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}