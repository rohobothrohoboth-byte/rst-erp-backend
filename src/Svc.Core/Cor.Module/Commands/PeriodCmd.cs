using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;

namespace Cor.Module.Commands;

public class AddPeriodCmd : IRequest<PeriodListDto> { public AddPeriodDto AddDto { get; set; } = default!; }
public class ModPeriodCmd : IRequest<PeriodListDto> { public EditPeriodDto ModDto { get; set; } = default!; }
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
                Name = request.AddDto.Name,
                DateStart = request.AddDto.DateStart,
                DateEnd = request.AddDto.DateEnd,
                IsActive = "0",
                Quarter = request.AddDto.Quarter,
                FiscalYearId = request.AddDto.FiscalYearId
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
        var oldPeriod = await _unitOfWork.Repository<Period>().GetById(request.ModDto.Id);
        if (oldPeriod == null) { throw new DomainException($"PERIOD with id [{request.ModDto.Id}] NOT FOUND."); }
        
        await _unitOfWork.Begin();
        try
        {
            oldPeriod.Name = request.ModDto.Name;
            oldPeriod.DateStart = request.ModDto.DateStart;
            oldPeriod.DateEnd = request.ModDto.DateEnd;
            oldPeriod.IsActive = request.ModDto.IsActive;
            oldPeriod.Quarter = request.ModDto.Quarter;
            oldPeriod.FiscalYearId = request.ModDto.FiscalYearId;
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
            var data = await _unitOfWork.Repository<Period>().GetById(request.Id);
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