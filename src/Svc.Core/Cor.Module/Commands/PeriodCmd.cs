using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Module.Commands;

public class AddPeriodCmd : IRequest<PeriodListDto> { public AddPeriodDto AddDto { get; set; } = default!; }
public class ModPeriodCmd : IRequest<PeriodListDto> { public EditPeriodDto ModDto { get; set; } = default!; }
public class DelPeriodCmd : IRequest { public Guid Id { get; set; } }



public class AddPeriodCmdHandler : IRequestHandler<AddPeriodCmd, PeriodListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public AddPeriodCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PeriodListDto> Handle(AddPeriodCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var period = new Period
            {
                Name = request.AddDto.Name,
                DateStart = request.AddDto.DateStart,
                DateEnd = request.AddDto.DateEnd,
                IsActive = BoolToStr.EnumToString(YesNo.Yes),
                Quarter = request.AddDto.Quarter,
                FiscalYearId = request.AddDto.FiscalYearId
            };
            await _uow.Add(period, ct);
            await _uow.Commit(ct);

            var res = new PeriodListDto();
            var response = await _med.Send(new PeriodByIdQry { Id = period.Id }, ct);
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

public class ModPeriodCmdHandler : IRequestHandler<ModPeriodCmd, PeriodListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public ModPeriodCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PeriodListDto> Handle(ModPeriodCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Period>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"PERIOD with id [{request.ModDto.Id}] NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.DateStart = request.ModDto.DateStart;
            oldData.DateEnd = request.ModDto.DateEnd;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.Quarter = request.ModDto.Quarter;
            oldData.FiscalYearId = request.ModDto.FiscalYearId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PeriodListDto();
            var response = await _med.Send(new PeriodByIdQry { Id = request.ModDto.Id }, ct);
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

public class DelPeriodCmdHandler : IRequestHandler<DelPeriodCmd>
{
    private readonly IUnitOfWork _uow;
    public DelPeriodCmdHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(DelPeriodCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Period>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"PERIOD with id [{request.Id}] NOT FOUND."); }
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