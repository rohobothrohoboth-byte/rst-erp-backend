using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MassTransit;
using MediatR;
using RST.Cont;

namespace Cor.App.Commands.Comp;

public class AddCompCmd : IRequest<CompListDto>
{
    public AddCompDto AddCompDto { get; set; } = default!;
}

public class AddCompCmdHandler : IRequestHandler<AddCompCmd, CompListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _iPubEndpoint;

    public AddCompCmdHandler(IUnitOfWork unitOfWork, IPublishEndpoint iPubEndpoint)
    {
        _unitOfWork = unitOfWork;
        _iPubEndpoint = iPubEndpoint;
    }

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
            var puComp = new CompAdded
            {
                Id = com.Id,
                Name = com.Name,
                NameAm = com.NameAm,
                IsDeleted = com.IsDeleted,
                BranchCount = 0,
                DateAdd = com.DateAdd,
                DateMod = com.DateMod,
                RowVersion = Convert.ToBase64String(com.RowVersion)
            };
            await _iPubEndpoint.Publish(puComp, cancellationToken);
            await _unitOfWork.Commit();

            var comp = await _unitOfWork.Repository<Company>().GetById(com.Id);
            var res = new CompListDto();
            if (comp == null) return res;
            res.Id = puComp.Id;
            res.Name = puComp.Name;
            res.NameAm = puComp.NameAm;
            res.IsDeleted = puComp.IsDeleted;
            res.BranchCount = 0;
            res.DateAdd = puComp.DateAdd;
            res.DateMod = puComp.DateMod;
            res.RowVersion = puComp.RowVersion;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
