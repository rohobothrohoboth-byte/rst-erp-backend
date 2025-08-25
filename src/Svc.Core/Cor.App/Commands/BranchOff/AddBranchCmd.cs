using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using Cor.Domain.Enums;
using MediatR;

namespace Cor.App.Commands.BranchOff;

public class AddBranchCmd : IRequest<BranchListDto>
{
    public AddBranchDto AddBranchDto { get; set; } = default!;
}

public class AddBranchCmdHandler : IRequestHandler<AddBranchCmd, BranchListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddBranchCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<BranchListDto> Handle(AddBranchCmd request, CancellationToken cancellationToken)
    {
        var bra = new Branch
        {
            Name = request.AddBranchDto.Name,
            NameAm = request.AddBranchDto.NameAm,
            Code = request.AddBranchDto.Code,
            Location = request.AddBranchDto.Location,
            BranchType = request.AddBranchDto.BranchType,
            BranchStat = request.AddBranchDto.BranchStat,
            OpenDate = request.AddBranchDto.DateOpened,
            CompId = request.AddBranchDto.CompId
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Branch>().Add(bra);
            await _unitOfWork.Commit();

            var nBra = await _unitOfWork.Repository<Branch>().GetById(bra.Id);
            var res = new BranchListDto();
            if (nBra == null) return res;

            var comp = await _unitOfWork.Repository<Company>().GetById(nBra.CompId);
            if (comp == null) { return res;}
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