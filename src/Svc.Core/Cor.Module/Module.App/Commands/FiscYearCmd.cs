using MediatR;
using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using Module.Domain.Enums;

namespace Module.App.Commands;

public class AddFiscalYearCmd : IRequest<FiscYearListDto> { public AddFiscYearDto AddFiscYearDto { get; set; } = default!; }

public class ModFiscalYearCmd : IRequest<FiscYearListDto> { public EditFiscYearDto EditFiscYearDto { get; set; } = default!; }

public class DelFiscalYearCmd : IRequest { public Guid Id { get; set; } }

public class AddFiscalYearCmdHandler : IRequestHandler<AddFiscalYearCmd, FiscYearListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddFiscalYearCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<FiscYearListDto> Handle(AddFiscalYearCmd request, CancellationToken cancellationToken)
    {
        var fYear = new FiscalYear
        {
            Name = request.AddFiscYearDto.Name,
            DateStart = request.AddFiscYearDto.DateStart,
            DateEnd = request.AddFiscYearDto.DateEnd,
            IsActive = request.AddFiscYearDto.IsActive
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<FiscalYear>().Add(fYear);
            await _unitOfWork.Commit();

            var yearFisc = await _unitOfWork.Repository<FiscalYear>().GetById(fYear.Id);
            var res = new FiscYearListDto();
            if (yearFisc == null) return res;
            res.Id = yearFisc.Id;
            res.Name = yearFisc.Name;
            res.DateStart = yearFisc.DateStart;
            res.DateEnd = yearFisc.DateEnd;
            res.IsActive = ((YesNo)Enum.Parse(typeof(YesNo), yearFisc.IsActive)).ToDisplayName();
            res.IsDeleted = yearFisc.IsDeleted;
            res.DateAdd = yearFisc.DateAdd;
            res.DateMod = yearFisc.DateMod;
            res.RowVersion = Convert.ToBase64String(yearFisc.RowVersion);
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

    public ModFiscalYearCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<FiscYearListDto> Handle(ModFiscalYearCmd request, CancellationToken cancellationToken)
    {
        var oldYear = await _unitOfWork.Repository<FiscalYear>().GetById(request.EditFiscYearDto.Id);
        if (oldYear == null)
        {
            throw new KeyNotFoundException($"FISCAL YEAR with Id {request.EditFiscYearDto.Id} NOT FOUND.");
        }

        oldYear.Name = request.EditFiscYearDto.Name;
        oldYear.DateStart = request.EditFiscYearDto.DateEnd;
        oldYear.DateEnd = request.EditFiscYearDto.DateEnd;
        oldYear.IsActive = request.EditFiscYearDto.IsActive;

        await _unitOfWork.Begin();

        try
        {
            var fYear = await _unitOfWork.Repository<FiscalYear>().Update(oldYear);
            await _unitOfWork.Commit();

            var yearFisc = await _unitOfWork.Repository<FiscalYear>().GetById(fYear.Id);
            var res = new FiscYearListDto();
            if (yearFisc == null) return res;
            res.Id = yearFisc.Id;
            res.Name = yearFisc.Name;
            res.DateStart = yearFisc.DateStart;
            res.DateEnd = yearFisc.DateEnd;
            res.IsActive = ((YesNo)Enum.Parse(typeof(YesNo), yearFisc.IsActive)).ToDisplayName();
            res.IsDeleted = yearFisc.IsDeleted;
            res.DateAdd = yearFisc.DateAdd;
            res.DateMod = yearFisc.DateMod;
            res.RowVersion = Convert.ToBase64String(yearFisc.RowVersion);
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