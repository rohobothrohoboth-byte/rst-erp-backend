using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Module.Commands;

public class AddFiscalYearCmd : IRequest<FiscYearListDto> { public AddFiscYearDto AddDto { get; set; } = default!; }
public class ModFiscalYearCmd : IRequest<FiscYearListDto> { public EditFiscYearDto ModDto { get; set; } = default!; }
public class DelFiscalYearCmd : IRequest { public Guid Id { get; set; } }



public class AddFiscalYearCmdHandler : IRequestHandler<AddFiscalYearCmd, FiscYearListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public AddFiscalYearCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<FiscYearListDto> Handle(AddFiscalYearCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var fYear = new FiscalYear
            {
                Name = request.AddDto.Name,
                DateStart = request.AddDto.DateStart,
                DateEnd = request.AddDto.DateEnd,
                IsActive = BoolToStr.EnumToString(YesNo.Yes),
            };
            await _uow.Add(fYear, ct);
            await _uow.Commit(ct);

            var res = new FiscYearListDto();
            var response = await _med.Send(new FiscalYearByIdQry { Id = fYear.Id }, ct);
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

public class ModFiscalYearCmdHandler : IRequestHandler<ModFiscalYearCmd, FiscYearListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public ModFiscalYearCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<FiscYearListDto> Handle(ModFiscalYearCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<FiscalYear>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"FISCAL YEAR with id [{request.ModDto.Id}] NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.DateStart = request.ModDto.DateStart;
            oldData.DateEnd = request.ModDto.DateEnd;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new FiscYearListDto();
            var response = await _med.Send(new FiscalYearByIdQry { Id = request.ModDto.Id }, ct);
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

public class DelFiscalYearCmdHandler : IRequestHandler<DelFiscalYearCmd>
{
    private readonly IUnitOfWork _uow;
    public DelFiscalYearCmdHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(DelFiscalYearCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<FiscalYear>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"FISCAL YEAR with id [{request.Id}] NOT FOUND."); }
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