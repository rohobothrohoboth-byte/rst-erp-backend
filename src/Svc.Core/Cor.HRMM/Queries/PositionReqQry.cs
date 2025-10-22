using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Models.Enums;
using Cor.HRMM.Services;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionReqAllQry : IRequest<List<PositionReqListDto>> { public Guid Id { get; set; } }

public class PositionReqByIdQry : IRequest<PositionReqListDto?> { public Guid Id { get; set; } }

public class PositionReqAllQryHandler : IRequestHandler<PositionReqAllQry, List<PositionReqListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public PositionReqAllQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<List<PositionReqListDto>> Handle(PositionReqAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PositionReq>().Find(c => c.PositionId == request.Id);
        var dataL = new List<PositionReqListDto>();
        var nData = dbData.ToList();
        if (nData.Count <= 0) return dataL;
        var posTypeL = await _lupClient.ProfessionTypeList(cancellationToken);

        foreach (var data in dbData)
        {
            var posType = posTypeL!.FirstOrDefault(t => t.Id == data.ProfessionTypeId);
            var c = new PositionReqListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                ProfessionTypeId = data.ProfessionTypeId,
                Gender = data.Gender,
                SaturdayWorkOption = data.SaturdayWorkOption,
                SundayWorkOption = data.SundayWorkOption,
                GenderStr = ((PositionGender)Enum.Parse(typeof(PositionGender), data.Gender)).ToDisplayName(),
                SaturdayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SaturdayWorkOption)).ToDisplayName(),
                SundayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SundayWorkOption)).ToDisplayName(),
                WorkingHours = data.WorkingHours,
                ProfessionType = posType != null ? posType.Name : "PROFESSION TYPE NOT AVAILABLE",
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
    private readonly ILupClient _lupClient;

    public PositionReqByIdQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<PositionReqListDto?> Handle(PositionReqByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PositionReq>().GetById(request.Id);
        if (data == null) { return null; }
        var posType = await _lupClient.ProfessionType(data.ProfessionTypeId, cancellationToken);

        var c = new PositionReqListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            ProfessionTypeId = data.ProfessionTypeId,
            Gender = data.Gender,
            SaturdayWorkOption = data.SaturdayWorkOption,
            SundayWorkOption = data.SundayWorkOption,
            GenderStr = ((PositionGender)Enum.Parse(typeof(PositionGender), data.Gender)).ToDisplayName(),
            SaturdayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SaturdayWorkOption)).ToDisplayName(),
            SundayWorkOptionStr = ((WorkOption)Enum.Parse(typeof(WorkOption), data.SundayWorkOption)).ToDisplayName(),
            WorkingHours = data.WorkingHours,
            ProfessionType = posType != null ? posType.Name : "PROFESSION TYPE NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}