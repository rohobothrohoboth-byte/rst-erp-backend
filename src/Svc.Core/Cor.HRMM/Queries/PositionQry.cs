using Common;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Helpers;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionAllQry : IRequest<List<PositionListDto>> { }
public class PositionByIdQry : IRequest<PositionListDto?> { public Guid Id { get; set; } }

public class PositionAllQryHandler : IRequestHandler<PositionAllQry, List<PositionListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _gRPC;

    public PositionAllQryHandler(IUnitOfWork unitOfWork, ICorModClient gRPC)
    {
        _unitOfWork = unitOfWork;
        _gRPC = gRPC;
    }

    public async Task<List<PositionListDto>> Handle(PositionAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Position>().GetAll();
        var dataL = new List<PositionListDto>();
        var deptL = await _gRPC.GetListDept(cancellationToken);

        foreach (var data in dbData)
        {
            var dept = deptL.Res.FirstOrDefault(t => t.Id == data.DepartmentId.ToString());
            var c = new PositionListDto
            {
                Id = data.Id,
                DepartmentId = data.DepartmentId,
                IsVacant = data.IsVacant,
                Name = data.Name,
                NameAm = data.NameAm,
                NoOfPosition = data.NoOfPosition,
                IsVacantStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsVacant)).ToDisplayName(),
                Department = dept!.Name != null ? dept.Name : "NOT AVAILABLE",
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
    private readonly ICorModClient _gRPC;

    public PositionByIdQryHandler(IUnitOfWork unitOfWork, ICorModClient gRPC)
    {
        _unitOfWork = unitOfWork;
        _gRPC = gRPC;
    }

    public async Task<PositionListDto?> Handle(PositionByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Position>().GetById(request.Id);
        if (data == null) { return null; }
        var dept = await _gRPC.GetDept(data.DepartmentId.ToString(), cancellationToken);

        var c = new PositionListDto
        {
            Id = data.Id,
            DepartmentId = data.DepartmentId,
            IsVacant = data.IsVacant,
            Name = data.Name,
            NameAm = data.NameAm,
            NoOfPosition = data.NoOfPosition,
            IsVacantStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsVacant)).ToDisplayName(),
            Department = dept.Res.Name != null ? dept.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}