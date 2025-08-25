using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using Cor.Domain.Enums;
using MediatR;

namespace Cor.App.Commands.FiscYear;

public class ModFiscalYearCmd : IRequest<FiscYearListDto>
{
    public EditFiscYearDto EditFiscYearDto { get; set; } = default!;
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