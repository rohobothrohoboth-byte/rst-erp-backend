using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Models.Enums;
using Cor.HRMM.Services;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionAllQry : IRequest<List<PositionListDto>> { }

public class PositionByIdQry : IRequest<PositionListDto?> { public Guid Id { get; set; } }

public class PositionAllQryHandler : IRequestHandler<PositionAllQry, List<PositionListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICoreModuleClient _lupClient;

    public PositionAllQryHandler(IUnitOfWork unitOfWork, ICoreModuleClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<List<PositionListDto>> Handle(PositionAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Position>().GetAll();
        var dataL = new List<PositionListDto>();
        var deptL = await _lupClient.DepartmentList(cancellationToken);

        foreach (var data in dbData)
        {
            var dept = deptL!.FirstOrDefault(t => t.Id == data.DepartmentId);
            var c = new PositionListDto
            {
                Id = data.Id,
                DepartmentId = data.DepartmentId,
                IsVacant = data.IsVacant,
                Name = data.Name,
                NameAm = data.NameAm,
                NoOfPosition = data.NoOfPosition,
                IsVacantStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsVacant)).ToDisplayName(),
                Department = dept != null ? dept.Name : "DEPARTMENT NOT AVAILABLE",
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

public class PositionByIdQryHandler : IRequestHandler<PositionByIdQry, PositionListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICoreModuleClient _lupClient;

    public PositionByIdQryHandler(IUnitOfWork unitOfWork, ICoreModuleClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<PositionListDto?> Handle(PositionByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Position>().GetById(request.Id);
        if (data == null) { return null; }
        var dept = await _lupClient.Department(data.DepartmentId, cancellationToken);

        var c = new PositionListDto
        {
            Id = data.Id,
            DepartmentId = data.DepartmentId,
            IsVacant = data.IsVacant,
            Name = data.Name,
            NameAm = data.NameAm,
            NoOfPosition = data.NoOfPosition,
            IsVacantStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsVacant)).ToDisplayName(),
            Department = dept != null ? dept.Name : "DEPARTMENT NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}