using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;

namespace Cor.Module.Commands;

public class AddFiscalYearCmd : IRequest<FiscYearListDto> { public AddFiscYearDto AddDto { get; set; } = default!; }
public class ModFiscalYearCmd : IRequest<FiscYearListDto> { public EditFiscYearDto ModDto { get; set; } = default!; }
public class DelFiscalYearCmd : IRequest { public Guid Id { get; set; } }

public class AddFiscalYearCmdHandler : IRequestHandler<AddFiscalYearCmd, FiscYearListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddFiscalYearCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<FiscYearListDto> Handle(AddFiscalYearCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var fYear = new FiscalYear
            {
                Name = request.AddDto.Name,
                DateStart = request.AddDto.DateStart,
                DateEnd = request.AddDto.DateEnd,
                IsActive = "0",
            };
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
        var oldYear = await _unitOfWork.Repository<FiscalYear>().GetById(request.ModDto.Id);
        if (oldYear == null) { throw new DomainException($"FISCAL YEAR with id [{request.ModDto.Id}] NOT FOUND."); }
        
        await _unitOfWork.Begin();
        try
        {
            oldYear.Name = request.ModDto.Name;
            oldYear.DateStart = request.ModDto.DateStart;
            oldYear.DateEnd = request.ModDto.DateEnd;
            oldYear.IsActive = request.ModDto.IsActive;
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
            var data = await _unitOfWork.Repository<FiscalYear>().GetById(request.Id);
            if (data == null) { throw new DomainException($"FISCAL YEAR with id [{request.Id}] NOT FOUND."); }
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