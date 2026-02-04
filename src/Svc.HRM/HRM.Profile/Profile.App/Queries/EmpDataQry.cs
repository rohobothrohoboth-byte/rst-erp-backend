using Common;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class GetPosEmpQry : IRequest<EmpPosResDto?> { public Guid Id { get; set; } }

public class GetPosEmpHandler : IRequestHandler<GetPosEmpQry, EmpPosResDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHrmmClient _corHRMM;

    public GetPosEmpHandler(IUnitOfWork unitOfWork, ICorHrmmClient corHRMM)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
    }

    public async Task<EmpPosResDto?> Handle(GetPosEmpQry request, CancellationToken cancellationToken)
    {
        var emp = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (emp == null) { return null; }

        var posReq = await _corHRMM.GetPosReq(emp.PositionId.ToString(), cancellationToken);
        if (posReq.Id == null) { return null; }
        var res = new EmpPosResDto
        {
            Id = Guid.Parse(posReq.Id),
            PositionId = emp.PositionId,
            SaturdayWorkOption = posReq.SaturdayWorkOption,
            SundayWorkOption = posReq.SundayWorkOption
        };

        return res;
    }
}
