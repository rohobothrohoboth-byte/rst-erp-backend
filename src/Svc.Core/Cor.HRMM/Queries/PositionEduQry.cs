using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Services;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionEduAllQry : IRequest<List<PositionEduListDto>> { }

public class PositionEduByIdQry : IRequest<PositionEduListDto?> { public Guid Id { get; set; } }

public class PositionEduAllQryHandler : IRequestHandler<PositionEduAllQry, List<PositionEduListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public PositionEduAllQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<List<PositionEduListDto>> Handle(PositionEduAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PositionEducation>().GetAll();
        var dataL = new List<PositionEduListDto>();
        var eduLevelL = await _lupClient.EducationLevelList(cancellationToken);
        var posL = await _unitOfWork.Repository<Position>().GetAll();
        var eduQualL = await _unitOfWork.Repository<EducationQual>().GetAll();

        foreach (var data in dbData)
        {
            var eduLevel = eduLevelL!.FirstOrDefault(t => t.Id == data.EducationLevelId);
            var pos = posL.FirstOrDefault(t => t.Id == data.PositionId);
            var eduQual = eduQualL.FirstOrDefault(t => t.Id == data.EducationQualId);
            var c = new PositionEduListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                EducationQualId = data.EducationQualId,
                EducationLevelId = data.EducationLevelId,
                EducationQual = eduQual != null ? eduQual.Name : "EDUCATION QUALIFICATION NOT AVAILABLE",
                EducationLevel = eduLevel != null ? eduLevel.Name : "EDUCATION LEVEL NOT AVAILABLE",
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

public class PositionEduByIdQryHandler : IRequestHandler<PositionEduByIdQry, PositionEduListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public PositionEduByIdQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<PositionEduListDto?> Handle(PositionEduByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PositionEducation>().GetById(request.Id);
        if (data == null) { return null; }
        var eduLevel = await _lupClient.EducationLevel(data.EducationLevelId, cancellationToken);
        var pos = await _unitOfWork.Repository<Position>().GetById(data.PositionId);
        var eduQual = await _unitOfWork.Repository<EducationQual>().GetById(data.EducationQualId);

        var c = new PositionEduListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            EducationQualId = data.EducationQualId,
            EducationLevelId = data.EducationLevelId,
            EducationQual = eduQual != null ? eduQual.Name : "EDUCATION QUALIFICATION NOT AVAILABLE",
            EducationLevel = eduLevel != null ? eduLevel.Name : "EDUCATION LEVEL NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion),
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