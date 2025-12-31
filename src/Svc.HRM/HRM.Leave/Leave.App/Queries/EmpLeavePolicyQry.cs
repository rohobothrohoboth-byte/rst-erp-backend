using Common;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class EmpLeavePolicyAllQry : IRequest<List<EmpLeavePolicyListDto>> { }
public class EmpLeavePolicyByIdQry : IRequest<EmpLeavePolicyListDto?> { public Guid Id { get; set; } }

public class EmpLeavePolicyAllQryHandler : IRequestHandler<EmpLeavePolicyAllQry, List<EmpLeavePolicyListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;

    public EmpLeavePolicyAllQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<List<EmpLeavePolicyListDto>> Handle(EmpLeavePolicyAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmpLeavePolicy>().GetAll();
        var dataL = new List<EmpLeavePolicyListDto>();
        var empL = await _hrmPro.GetListEmp(cancellationToken);
        var leaP = await _unitOfWork.Repository<LeavePolicy>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL.Res.FirstOrDefault(t => t.Id == data.EmployeeId.ToString());
            var lea = leaP.FirstOrDefault(t => t.Id == data.LeavePolicyId);
            var c = new EmpLeavePolicyListDto
            {
                Id = data.Id,
                EmployeeId = data.EmployeeId,
                LeavePolicyId = data.LeavePolicyId,
                EmployeeName = emp != null ? emp.Name : "NOT AVAILABLE",
                LeavePolicy = lea != null ? lea.Name : "NOT AVAILABLE",
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

public class EmpLeavePolicyByIdQryHandler : IRequestHandler<EmpLeavePolicyByIdQry, EmpLeavePolicyListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;

    public EmpLeavePolicyByIdQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<EmpLeavePolicyListDto?> Handle(EmpLeavePolicyByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpLeavePolicy>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _hrmPro.GetEmp(data.EmployeeId.ToString(), cancellationToken);
        var lea = await _unitOfWork.Repository<LeavePolicy>().GetById(data.LeavePolicyId);

        var c = new EmpLeavePolicyListDto
        {
            Id = data.Id,
            EmployeeId = data.EmployeeId,
            LeavePolicyId = data.LeavePolicyId,
            EmployeeName = emp.Res.Name != null ? emp.Res.Name : "NOT AVAILABLE",
            LeavePolicy = lea != null ? lea.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}