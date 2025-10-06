using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionBenefitAllQry : IRequest<List<PositionBenefitListDto>> { }

public class PositionBenefitByIdQry : IRequest<PositionBenefitListDto?> { public Guid Id { get; set; } }

public class PositionBenefitAllQryHandler : IRequestHandler<PositionBenefitAllQry, List<PositionBenefitListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionBenefitAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PositionBenefitListDto>> Handle(PositionBenefitAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PositionBenefit>().GetAll();
        var dataL = new List<PositionBenefitListDto>();
        //var benefitL = await _unitOfWork.Repository<BenefitSetting>().GetAll();
        var posL = await _unitOfWork.Repository<Position>().GetAll();

        foreach (var data in dbData)
        {
            //var benefit = benefitL.FirstOrDefault(t => t.Id == data.BenefitSettingId);
            var pos = posL.FirstOrDefault(t => t.Id == data.PositionId);
            var c = new PositionBenefitListDto
            {
                Id = data.Id,
                BenefitSettingId = data.BenefitSettingId,
                PositionId = data.PositionId,
                //BenefitSetting = benefit != null ? benefit.Name : "BENEFIT SETTING NOT AVAILABLE",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            
            if (pos != null)
            {
                c.Position = pos.Name;
                c.PositionAm = pos.NameAm;
            }
            else
            {
                c.Position = "POSITION NOT AVAILABLE";
                c.PositionAm = "POSITION NOT AVAILABLE";
            }
            dataL.Add(c);
        }

        return dataL;
    }
}

public class PositionBenefitByIdQryHandler : IRequestHandler<PositionBenefitByIdQry, PositionBenefitListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionBenefitByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PositionBenefitListDto?> Handle(PositionBenefitByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<PositionBenefit>().GetById(request.Id);
        if (nData == null) { return null; }
        //var benefit = await _unitOfWork.Repository<BenefitSetting>().GetById(nData.BenefitSettingId);
        var pos = await _unitOfWork.Repository<Position>().GetById(nData.BenefitSettingId);

        var c = new PositionBenefitListDto
        {
            Id = nData.Id,
            BenefitSettingId = nData.BenefitSettingId,
            PositionId = nData.PositionId,
            //BenefitSetting = benefit != null ? benefit.Name : "BENEFIT SETTING NOT AVAILABLE",
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };

        if (pos != null)
        {
            c.Position = pos.Name;
            c.PositionAm = pos.NameAm;
        }
        else
        {
            c.Position = "POSITION NOT AVAILABLE";
            c.PositionAm = "POSITION NOT AVAILABLE";
        }
        return c;
    }
}