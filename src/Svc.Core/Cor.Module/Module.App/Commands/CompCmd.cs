using MediatR;
using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;

namespace Module.App.Commands;

public class AddCompCmd : IRequest<CompListDto> { public AddCompDto AddCompDto { get; set; } = default!; }

public class ModCompCmd : IRequest<CompListDto> { public EditCompDto EditCompDto { get; set; } = default!; }

public class DelCompCmd : IRequest { public Guid Id { get; set; } }

public class AddCompCmdHandler : IRequestHandler<AddCompCmd, CompListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddCompCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<CompListDto> Handle(AddCompCmd request, CancellationToken cancellationToken)
    {
        var com = new Company
        {
            Name = request.AddCompDto.Name,
            NameAm = request.AddCompDto.NameAm
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Company>().Add(com);
            await _unitOfWork.Commit();

            var comp = await _unitOfWork.Repository<Company>().GetById(com.Id);
            var res = new CompListDto();
            if (comp == null) return res;
            res.Id = comp.Id;
            res.Name = comp.Name;
            res.NameAm = comp.NameAm;
            res.IsDeleted = comp.IsDeleted;
            res.BranchCount = "0";
            res.DateAdd = comp.DateAdd;
            res.DateMod = comp.DateMod;
            res.RowVersion = Convert.ToBase64String(comp.RowVersion);
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class ModCompCmdHandler : IRequestHandler<ModCompCmd, CompListDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ModCompCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<CompListDto> Handle(ModCompCmd request, CancellationToken cancellationToken)
    {
        var oldComp = await _unitOfWork.Repository<Company>().GetById(request.EditCompDto.Id);

        if (oldComp == null)
        {
            throw new KeyNotFoundException($"Company with Id {request.EditCompDto.Id} not found.");
        }

        oldComp.Name = request.EditCompDto.Name;
        oldComp.NameAm = request.EditCompDto.NameAm;

        await _unitOfWork.Begin();

        try
        {
            var comp = await _unitOfWork.Repository<Company>().Update(oldComp);
            await _unitOfWork.Commit();

            var bra = await _unitOfWork.Repository<Branch>().Find(b => b.CompId == comp.Id);
            var result = new CompListDto
            {
                Id = comp.Id,
                Name = comp.Name,
                NameAm = comp.NameAm,
                IsDeleted = comp.IsDeleted,
                BranchCount = $"{bra.Count()}",
                DateAdd = comp.DateAdd,
                DateMod = comp.DateMod,
                RowVersion = Convert.ToBase64String(comp.RowVersion),
            };

            return result;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class DelCompCmdHandler : IRequestHandler<DelCompCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelCompCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelCompCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Company>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}