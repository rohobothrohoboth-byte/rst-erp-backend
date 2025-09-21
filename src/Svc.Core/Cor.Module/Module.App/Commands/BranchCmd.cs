using MediatR;
using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using Module.Domain.Enums;

namespace Module.App.Commands;

public class AddBranchCmd : IRequest<BranchListDto> { public AddBranchDto AddBranchDto { get; set; } = default!; }

public class ModBranchCmd : IRequest<BranchListDto> { public EditBranchDto EditBranchDto { get; set; } = default!; }

public class DelBranchCmd : IRequest { public Guid Id { get; set; } }

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

public class DelBranchCmdHandler : IRequestHandler<DelBranchCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelBranchCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelBranchCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Branch>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
