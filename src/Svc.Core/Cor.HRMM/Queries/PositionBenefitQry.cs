using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Models.Enums;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionBenefitAllQry : IRequest<List<PositionBenefitListDto>> { public Guid Id { get; set; } }

public class PositionBenefitByIdQry : IRequest<PositionBenefitListDto?> { public Guid Id { get; set; } }

public class PositionBenefitAllQryHandler : IRequestHandler<PositionBenefitAllQry, List<PositionBenefitListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionBenefitAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PositionBenefitListDto>> Handle(PositionBenefitAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PositionBenefit>().Find(c => c.PositionId == request.Id);
        var dataL = new List<PositionBenefitListDto>();
        var nData = dbData.ToList();
        if (nData.Count <= 0) return dataL;
        var benL = await _unitOfWork.Repository<BenefitSetting>().GetAll();

        foreach (var data in dbData)
        {
            var ben = benL.FirstOrDefault(t => t.Id == data.BenefitSettingId);
            var c = new PositionBenefitListDto
            {
                Id = data.Id,
                BenefitSettingId = data.BenefitSettingId,
                PositionId = data.PositionId,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };

            if (ben != null)
            {
                c.BenefitName = ben.Name;
                c.Benefit = $"{ben.BenefitValue:#,##0.##}";
                c.PerStr = ((Per)Enum.Parse(typeof(Per), ben.Per)).ToDisplayName();
            }
            else
            {
                c.BenefitName = "BENEFIT NOT AVAILABLE";
                c.Benefit = "";
                c.PerStr = "";
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
        var ben = await _unitOfWork.Repository<BenefitSetting>().GetById(nData.BenefitSettingId);

        var c = new PositionBenefitListDto
        {
            Id = nData.Id,
            BenefitSettingId = nData.BenefitSettingId,
            PositionId = nData.PositionId,
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };

        if (ben != null)
        {
            c.BenefitName = ben.Name;
            c.Benefit = $"{ben.BenefitValue:#,##0.##}";
            c.PerStr = ((Per)Enum.Parse(typeof(Per), ben.Per)).ToDisplayName();
        }
        else
        {
            c.BenefitName = "BENEFIT NOT AVAILABLE";
            c.Benefit = "";
            c.PerStr = "";
        }
        return c;
    }
}