using Common;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionEduAllQry : IRequest<List<PositionEduListDto>> { public Guid Id { get; set; } }

public class PositionEduByIdQry : IRequest<PositionEduListDto?> { public Guid Id { get; set; } }

public class PositionEduAllQryHandler : IRequestHandler<PositionEduAllQry, List<PositionEduListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _gRPC;

    public PositionEduAllQryHandler(IUnitOfWork unitOfWork, ILupClient gRPC)
    {
        _unitOfWork = unitOfWork;
        _gRPC = gRPC;
    }

    public async Task<List<PositionEduListDto>> Handle(PositionEduAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PositionEducation>().Find(c => c.PositionId == request.Id);
        var dataL = new List<PositionEduListDto>();
        var nData = dbData.ToList();
        if (nData.Count <= 0) return dataL;

        var eduLevelL = await _gRPC.GetListEduLevel(cancellationToken);
        var eduQualL = await _unitOfWork.Repository<EducationQual>().GetAll();

        foreach (var data in dbData)
        {
            var eduLevel = eduLevelL.Res.FirstOrDefault(t => t.Id == data.EducationLevelId.ToString());
            var eduQual = eduQualL.FirstOrDefault(t => t.Id == data.EducationQualId);
            var c = new PositionEduListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                EducationQualId = data.EducationQualId,
                EducationLevelId = data.EducationLevelId,
                EducationQual = eduQual != null ? eduQual.Name : "NOT AVAILABLE",
                EducationLevel = eduLevel.Name != null ? eduLevel.Name : "NOT AVAILABLE",
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

public class PositionEduByIdQryHandler : IRequestHandler<PositionEduByIdQry, PositionEduListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _gRPC;

    public PositionEduByIdQryHandler(IUnitOfWork unitOfWork, ILupClient gRPC)
    {
        _unitOfWork = unitOfWork;
        _gRPC = gRPC;
    }

    public async Task<PositionEduListDto?> Handle(PositionEduByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PositionEducation>().GetById(request.Id);
        if (data == null) { return null; }
        var eduLevel = await _gRPC.GetEduLevel(data.EducationLevelId.ToString(), cancellationToken);
        var eduQual = await _unitOfWork.Repository<EducationQual>().GetById(data.EducationQualId);

        var c = new PositionEduListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            EducationQualId = data.EducationQualId,
            EducationLevelId = data.EducationLevelId,
            EducationQual = eduQual != null ? eduQual.Name : "NOT AVAILABLE",
            EducationLevel = eduLevel.Res.Name != null ? eduLevel.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion),
        };
        return c;
    }
}