using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using MediatR;

namespace Cor.Module.Commands;

public class AddFiscalYearCmd : IRequest<FiscYearListDto> { public AddFiscYearDto AddFiscYearDto { get; set; } = default!; }
public class ModFiscalYearCmd : IRequest<FiscYearListDto> { public EditFiscYearDto EditFiscYearDto { get; set; } = default!; }
public class DelFiscalYearCmd : IRequest { public Guid Id { get; set; } }

public class AddFiscalYearCmdHandler : IRequestHandler<AddFiscalYearCmd, FiscYearListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddFiscalYearCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<FiscYearListDto> Handle(AddFiscalYearCmd request, CancellationToken cancellationToken)
    {
        var fYear = new FiscalYear
        {
            Name = request.AddFiscYearDto.Name,
            DateStart = request.AddFiscYearDto.DateStart,
            DateEnd = request.AddFiscYearDto.DateEnd,
            IsActive = "0",
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<FiscalYear>().Add(fYear);
            await _unitOfWork.Commit();

            var res = new FiscYearListDto();
            var response = await _med.Send(new FiscalYearByIdQry { Id = fYear.Id }, cancellationToken);
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

public class ModFiscalYearCmdHandler : IRequestHandler<ModFiscalYearCmd, FiscYearListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ModFiscalYearCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<FiscYearListDto> Handle(ModFiscalYearCmd request, CancellationToken cancellationToken)
    {
        var oldYear = await _unitOfWork.Repository<FiscalYear>().GetById(request.EditFiscYearDto.Id);
        if (oldYear == null)
        {
            throw new KeyNotFoundException($"FISCAL YEAR with Id {request.EditFiscYearDto.Id} NOT FOUND.");
        }

        oldYear.Name = request.EditFiscYearDto.Name;
        oldYear.DateStart = request.EditFiscYearDto.DateStart;
        oldYear.DateEnd = request.EditFiscYearDto.DateEnd;
        oldYear.IsActive = request.EditFiscYearDto.IsActive;

        await _unitOfWork.Begin();

        try
        {
            var fYear = await _unitOfWork.Repository<FiscalYear>().Update(oldYear);
            await _unitOfWork.Commit();

            var res = new FiscYearListDto();
            var response = await _med.Send(new FiscalYearByIdQry { Id = fYear.Id }, cancellationToken);
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

public class DelFiscalYearCmdHandler : IRequestHandler<DelFiscalYearCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelFiscalYearCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelFiscalYearCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<FiscalYear>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}