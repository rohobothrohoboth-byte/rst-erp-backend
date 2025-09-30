using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class EducationQualAllQry : IRequest<List<EducationQualListDto>> { }

public class EducationQualByIdQry : IRequest<EducationQualListDto?> { public Guid Id { get; set; } }

public class EducationQualAllQryHandler : IRequestHandler<EducationQualAllQry, List<EducationQualListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EducationQualAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EducationQualListDto>> Handle(EducationQualAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EducationQual>().GetAll();
        var dataL = new List<EducationQualListDto>();

        foreach (var data in dbData)
        {
            var c = new EducationQualListDto
            {
                Id = data.Id,
                Name = data.Name,
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

public class EducationQualByIdQryHandler : IRequestHandler<EducationQualByIdQry, EducationQualListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EducationQualByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EducationQualListDto?> Handle(EducationQualByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<EducationQual>().GetById(request.Id);
        if (nData == null) { return null; }

        var c = new EducationQualListDto
        {
            Id = nData.Id,
            Name = nData.Name,
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };
        return c;
    }
}