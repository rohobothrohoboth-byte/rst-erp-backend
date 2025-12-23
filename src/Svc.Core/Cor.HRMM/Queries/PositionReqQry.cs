using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Models.Enums;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionReqAllQry : IRequest<List<PositionReqListDto>> { public Guid Id { get; set; } }

public class PositionReqByIdQry : IRequest<PositionReqListDto?> { public Guid Id { get; set; } }

public class PositionReqAllQryHandler : IRequestHandler<PositionReqAllQry, List<PositionReqListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionReqAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PositionReqListDto>> Handle(PositionReqAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PositionReq>().Find(c => c.PositionId == request.Id);
        var dataL = new List<PositionReqListDto>();
        var nData = dbData.ToList();
        if (nData.Count <= 0) return dataL;

        foreach (var data in dbData)
        {
            var c = new PositionReqListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                ProfessionType = data.ProfessionType,
                Gender = data.Gender,
                SaturdayWorkOption = data.SaturdayWorkOption,
                SundayWorkOption = data.SundayWorkOption,
                GenderStr = ((PositionGender)Enum.Parse(typeof(PositionGender), data.Gender)).ToDisplayName(),
                SaturdayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SaturdayWorkOption)).ToDisplayName(),
                SundayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SundayWorkOption)).ToDisplayName(),
                ProfessionTypeStr = ((ProfessionType)Enum.Parse(typeof(ProfessionType), data.ProfessionType)).ToDisplayName(),
                WorkingHours = data.WorkingHours,
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

public class PositionReqByIdQryHandler : IRequestHandler<PositionReqByIdQry, PositionReqListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public PositionReqByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PositionReqListDto?> Handle(PositionReqByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PositionReq>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new PositionReqListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            ProfessionType = data.ProfessionType,
            Gender = data.Gender,
            SaturdayWorkOption = data.SaturdayWorkOption,
            SundayWorkOption = data.SundayWorkOption,
            GenderStr = ((PositionGender)Enum.Parse(typeof(PositionGender), data.Gender)).ToDisplayName(),
            SaturdayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SaturdayWorkOption)).ToDisplayName(),
            SundayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SundayWorkOption)).ToDisplayName(),
            ProfessionTypeStr = ((ProfessionType)Enum.Parse(typeof(ProfessionType), data.ProfessionType)).ToDisplayName(),
            WorkingHours = data.WorkingHours,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}