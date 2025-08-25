using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Commands.Comp;

public class AddCompCmd : IRequest<CompListDto>
{
    public AddCompDto AddCompDto { get; set; } = default!;
}

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
            res.BranchCount = 0;
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
