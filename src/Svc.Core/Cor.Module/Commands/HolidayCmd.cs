using Cor.Module.Helpers;
using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using MediatR;
using System.Data;

namespace Cor.Module.Commands;

public class AddHolidayCmd : IRequest<HolidayListDto> { public AddHolidayDto AddDto { get; set; } = default!; }
public class ModHolidayCmd : IRequest<HolidayListDto> { public EditHolidayDto ModDto { get; set; } = default!; }
public class DelHolidayCmd : IRequest { public Guid Id { get; set; } }

public class AddHolidayCmdHandler : IRequestHandler<AddHolidayCmd, HolidayListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddHolidayCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<HolidayListDto> Handle(AddHolidayCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var holiday = new Holiday
            {
                Name = request.AddDto.Name,
                Date = request.AddDto.Date,
                IsPublic = request.AddDto.IsPublic,
                FiscalYearId = request.AddDto.FiscalYearId
            };
            await _unitOfWork.Repository<Holiday>().Add(holiday);
            await _unitOfWork.Commit();
            var res = new HolidayListDto();
            var response = await _med.Send(new HolidayByIdQry { Id = holiday.Id }, cancellationToken);
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

public class ModHolidayCmdHandler : IRequestHandler<ModHolidayCmd, HolidayListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ModHolidayCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<HolidayListDto> Handle(ModHolidayCmd request, CancellationToken cancellationToken)
    {
        var oldHoliday = await _unitOfWork.Repository<Holiday>().GetById(request.ModDto.Id);
        if (oldHoliday == null) { throw new DomainException($"HOLIDAY with id [{request.ModDto.Id}] NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldHoliday.Name = request.ModDto.Name;
            oldHoliday.Date = request.ModDto.Date;
            oldHoliday.IsPublic = request.ModDto.IsPublic;
            var nHoliday = await _unitOfWork.Repository<Holiday>().Update(oldHoliday);
            await _unitOfWork.Commit();

            var res = new HolidayListDto();
            var response = await _med.Send(new HolidayByIdQry { Id = nHoliday.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch (DBConcurrencyException)
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class DelHolidayCmdHandler : IRequestHandler<DelHolidayCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelHolidayCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelHolidayCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<Holiday>().GetById(request.Id);
            if (data == null) { throw new DomainException($"HOLIDAY with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<Holiday>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}