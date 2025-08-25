using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using Cor.Domain.Enums;
using MediatR;

namespace Cor.App.Commands.BranchOff;

public class ModBranchCmd : IRequest<BranchListDto>
{
    public EditBranchDto EditBranchDto { get; set; } = default!;
}

public class ModBranchCmdHandler : IRequestHandler<ModBranchCmd, BranchListDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ModBranchCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<BranchListDto> Handle(ModBranchCmd request, CancellationToken cancellationToken)
    {
        var oldBra = await _unitOfWork.Repository<Branch>().GetById(request.EditBranchDto.Id);
        if (oldBra == null)
        {
            throw new KeyNotFoundException($"BRANCH with Id {request.EditBranchDto.Id} NOT FOUND.");
        }

        oldBra.Name = request.EditBranchDto.Name;
        oldBra.NameAm = request.EditBranchDto.NameAm;
        oldBra.Code = request.EditBranchDto.NameAm;
        oldBra.Location = request.EditBranchDto.NameAm;
        oldBra.BranchType = request.EditBranchDto.BranchType;
        oldBra.BranchStat = request.EditBranchDto.BranchStat;
        oldBra.OpenDate = request.EditBranchDto.DateOpened;
        oldBra.CompId = request.EditBranchDto.CompId;

        await _unitOfWork.Begin();

        try
        {
            var nBra = await _unitOfWork.Repository<Branch>().Update(oldBra);
            await _unitOfWork.Commit();

            var res = new BranchListDto();
            var comp = await _unitOfWork.Repository<Company>().GetById(nBra.CompId);
            if (comp == null) { return res; }
            res.Id = nBra.Id;
            res.Name = nBra.Name;
            res.NameAm = nBra.NameAm;
            res.Code = nBra.Code;
            res.Location = nBra.Location;
            res.BranchType = ((BranchType)Enum.Parse(typeof(BranchType), nBra.BranchType)).ToDisplayName();
            res.BranchStat = ((BranchStat)Enum.Parse(typeof(BranchStat), nBra.BranchStat)).ToDisplayName();
            res.Comp = comp.Name;
            res.CompAm = comp.Name;
            res.OpenDate = nBra.OpenDate;
            res.IsDeleted = nBra.IsDeleted;
            res.DateAdd = nBra.DateAdd;
            res.DateMod = nBra.DateMod;
            res.RowVersion = Convert.ToBase64String(nBra.RowVersion);
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
