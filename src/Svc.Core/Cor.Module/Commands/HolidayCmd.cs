using Cor.Module.Interfaces;
using Cor.Module.Middlewares;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using MediatR;

namespace Cor.Module.Commands;

public class AddHolidayCmd : IRequest<HolidayListDto> { public AddHolidayDto AddHolidayDto { get; set; } = default!; }
public class ModHolidayCmd : IRequest<HolidayListDto> { public EditHolidayDto EditHolidayDto { get; set; } = default!; }
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
            var res = new HolidayListDto();
            var cfYear = await _unitOfWork.Repository<FiscalYear>().GetFoD(f => f.IsActive == "0" && f.DateEnd >= DateTime.UtcNow);
            if (cfYear == null)
            {
                throw new EntityNotFoundException("FiscalYear");
                //return res;
            }
            var holiday = new Holiday
            {
                Name = request.AddHolidayDto.Name,
                Date = request.AddHolidayDto.Date,
                IsPublic = request.AddHolidayDto.IsPublic,
                FiscalYearId = cfYear.Id
            };
            await _unitOfWork.Repository<Holiday>().Add(holiday);
            await _unitOfWork.Commit();
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
        var oldHoliday = await _unitOfWork.Repository<Holiday>().GetById(request.EditHolidayDto.Id);
        if (oldHoliday == null) { throw new EntityNotFoundException("Holiday", request.EditHolidayDto.Id); }

        await _unitOfWork.Begin();

        try
        {
            oldHoliday.Name = request.EditHolidayDto.Name;
            oldHoliday.Date = request.EditHolidayDto.Date;
            oldHoliday.IsPublic = request.EditHolidayDto.IsPublic;

            var nHoliday = await _unitOfWork.Repository<Holiday>().Update(oldHoliday);
            await _unitOfWork.Commit();

            var res = new HolidayListDto();
            var response = await _med.Send(new HolidayByIdQry { Id = nHoliday.Id }, cancellationToken);
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
            if (data == null) { throw new EntityNotFoundException("Holiday", request.Id); }

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