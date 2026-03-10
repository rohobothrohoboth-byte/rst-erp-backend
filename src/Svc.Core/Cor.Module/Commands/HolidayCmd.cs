using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.Module.Commands;

public class AddHolidayCmd : IRequest<HolidayListDto> { public AddHolidayDto AddDto { get; set; } = default!; }
public class ModHolidayCmd : IRequest<HolidayListDto> { public EditHolidayDto ModDto { get; set; } = default!; }
public class DelHolidayCmd : IRequest { public Guid Id { get; set; } }



public class AddHolidayCmdHandler : IRequestHandler<AddHolidayCmd, HolidayListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public AddHolidayCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<HolidayListDto> Handle(AddHolidayCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var holiday = new Holiday
            {
                Name = request.AddDto.Name,
                Date = request.AddDto.Date,
                IsPublic = request.AddDto.IsPublic,
                FiscalYearId = request.AddDto.FiscalYearId
            };
            await _uow.Add(holiday, ct);
            await _uow.Commit(ct);
            var res = new HolidayListDto();
            var response = await _med.Send(new HolidayByIdQry { Id = holiday.Id }, ct);
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

public class ModHolidayCmdHandler : IRequestHandler<ModHolidayCmd, HolidayListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public ModHolidayCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<HolidayListDto> Handle(ModHolidayCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Holiday>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"HOLIDAY with id [{request.ModDto.Id}] NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.Date = request.ModDto.Date;
            oldData.IsPublic = request.ModDto.IsPublic;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit();

            var res = new HolidayListDto();
            var response = await _med.Send(new HolidayByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch (DBConcurrencyException)
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class DelHolidayCmdHandler : IRequestHandler<DelHolidayCmd>
{
    private readonly IUnitOfWork _uow;
    public DelHolidayCmdHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(DelHolidayCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Holiday>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"HOLIDAY with id [{request.Id}] NOT FOUND."); }
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