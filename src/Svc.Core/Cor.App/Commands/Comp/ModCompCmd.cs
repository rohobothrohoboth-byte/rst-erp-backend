using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Commands.Comp;

public class ModCompCmd : IRequest<CompListDto>
{
    public EditCompDto EditCompDto { get; set; } = default!;
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
                BranchCount = bra.Count(),
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
