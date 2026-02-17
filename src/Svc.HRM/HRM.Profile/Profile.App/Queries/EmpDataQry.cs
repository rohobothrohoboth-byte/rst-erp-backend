using Common;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class GetPosEmpQry : IRequest<EmpPosResDto?> { public Guid Id { get; set; } }
public class GetEmpIdByIdQry : IRequest<HrmEmpId?> { public Guid Id { get; set; } }
public class GetEmpIdAllQry : IRequest<List<HrmEmpId>> { }



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

public class GetEmpIdByIdHandler : IRequestHandler<GetEmpIdByIdQry, HrmEmpId?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corMod;

    public GetEmpIdByIdHandler(IUnitOfWork unitOfWork, ICorModClient corMod)
    {
        _unitOfWork = unitOfWork;
        _corMod = corMod;
    }

    public async Task<HrmEmpId?> Handle(GetEmpIdByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        var dept = await _corMod.GetDbc(data.DepartmentId.ToString(), cancellationToken);

        var c = new HrmEmpId
        {
            Id = data.Id,
            PositionId = data.PositionId,
            DeptId = data.DepartmentId,
            BranchId = dept != null ? Guid.Parse(dept!.BranchId) : Guid.Empty,
            CompanyId = dept != null ? Guid.Parse(dept!.CompId) : Guid.Empty,
            JgStepId = data.JobGradeId,
            JgId = data.JobGradeId
        };
        return c;
    }
}

public class GetEmpIdAllHandler : IRequestHandler<GetEmpIdAllQry, List<HrmEmpId>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corMod;

    public GetEmpIdAllHandler(IUnitOfWork unitOfWork, ICorModClient corMod)
    {
        _unitOfWork = unitOfWork;
        _corMod = corMod;
    }

    public async Task<List<HrmEmpId>> Handle(GetEmpIdAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Employee>().GetAll();
        var dataL = new List<HrmEmpId>();
        var deL = await _corMod.GetListDbc(cancellationToken);

        foreach (var data in dbData)
        {

            var dept = deL.Res.FirstOrDefault(t => t.DeptId == data.DepartmentId.ToString());
            var c = new HrmEmpId
            {
                Id = data.Id,
                PositionId = data.PositionId,
                DeptId = data.DepartmentId,
                BranchId = dept != null ? Guid.Parse(dept!.BranchId) : Guid.Empty,
                CompanyId = dept != null ? Guid.Parse(dept!.CompId) : Guid.Empty,
                JgStepId = data.JobGradeId,
                JgId = data.JobGradeId
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

