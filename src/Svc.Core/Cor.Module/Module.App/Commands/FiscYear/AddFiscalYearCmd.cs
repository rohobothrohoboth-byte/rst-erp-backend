using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using Module.Domain.Enums;
using MediatR;

namespace Module.App.Commands.FiscYear;

public class AddFiscalYearCmd : IRequest<FiscYearListDto>
{
    public AddFiscYearDto AddFiscYearDto { get; set; } = default!;
}

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
