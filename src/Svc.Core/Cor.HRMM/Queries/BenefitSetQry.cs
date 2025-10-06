using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class BenefitSetAllQry : IRequest<List<BenefitSetListDto>> { }

public class BenefitSetByIdQry : IRequest<BenefitSetListDto?> { public Guid Id { get; set; } }

public class BenefitSetAllQryHandler : IRequestHandler<BenefitSetAllQry, List<BenefitSetListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public BenefitSetAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<BenefitSetListDto>> Handle(BenefitSetAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<BenefitSetting>().GetAll();
        var dataL = new List<BenefitSetListDto>();

        foreach (var data in dbData)
        {
            var c = new BenefitSetListDto
            {
                Id = data.Id,
                Name = data.Name,
                Benefit = data.BenefitValue,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class BenefitSetByIdQryHandler : IRequestHandler<BenefitSetByIdQry, BenefitSetListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public BenefitSetByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<BenefitSetListDto?> Handle(BenefitSetByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<BenefitSetting>().GetById(request.Id);
        if (nData == null) { return null; }

        var c = new BenefitSetListDto
        {
            Id = nData.Id,
            Name = nData.Name,
            Benefit = nData.BenefitValue,
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };
        return c;
    }
}