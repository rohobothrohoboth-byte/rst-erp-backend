using MediatR;
using Module.App.Interfaces;
using Module.App.Queries;
using Module.Domain.DTOs;
using Module.Domain.Entities;

namespace Module.App.Commands;

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
        var period = new Period
        {
            Name = request.AddPeriodDto.Name,
            DateStart = request.AddPeriodDto.DateStart,
            DateEnd = request.AddPeriodDto.DateEnd,
            IsActive = request.AddPeriodDto.IsActive,
            QuarterId = request.AddPeriodDto.QuarterId,
            FiscalYearId = request.AddPeriodDto.FiscalYearId
        };

        await _unitOfWork.Begin();
        try
        {
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
        if (oldPeriod == null)
        {
            throw new KeyNotFoundException($"PERIOD with Id {request.EditPeriodDto.Id} NOT FOUND.");
        }

        oldPeriod.Name = request.EditPeriodDto.Name;
        oldPeriod.DateStart = request.EditPeriodDto.DateStart;
        oldPeriod.DateEnd = request.EditPeriodDto.DateEnd;
        oldPeriod.IsActive = request.EditPeriodDto.IsActive;
        oldPeriod.QuarterId = request.EditPeriodDto.QuarterId;
        oldPeriod.FiscalYearId = request.EditPeriodDto.FiscalYearId;

        await _unitOfWork.Begin();

        try
        {
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
