using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MassTransit;
using MediatR;
using RST.Cont;

namespace Cor.App.Commands.Comp;

public class UpdateCompCmd : IRequest<CompListDto>
{
    public EditCompDto EditCompDto { get; set; } = default!;
}

public class UpdateCompCmdHandler : IRequestHandler<UpdateCompCmd, CompListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _iPubEndpoint;

    public UpdateCompCmdHandler(IUnitOfWork unitOfWork, IPublishEndpoint iPubEndpoint)
    {
        _unitOfWork = unitOfWork;
        _iPubEndpoint = iPubEndpoint;
    }

    public async Task<CompListDto> Handle(UpdateCompCmd request, CancellationToken cancellationToken)
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
            var braO = await _unitOfWork.Repository<Branch>().Find(b => b.CompId == comp.Id);
            var puComp = new CompUpdated
            {
                Id = comp.Id,
                Name = comp.Name,
                NameAm = comp.NameAm,
                IsDeleted = comp.IsDeleted,
                BranchCount = braO.Count(),
                DateAdd = comp.DateAdd,
                DateMod = comp.DateMod,
                RowVersion = Convert.ToBase64String(comp.RowVersion)
            };
            await _iPubEndpoint.Publish(puComp, cancellationToken);

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
