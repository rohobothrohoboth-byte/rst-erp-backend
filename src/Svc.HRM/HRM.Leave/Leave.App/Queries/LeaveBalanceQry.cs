using Common;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveBalanceAllQry : IRequest<List<LeaveBalanceListDto>> { }
public class LeaveBalanceByIdQry : IRequest<LeaveBalanceListDto?> { public Guid Id { get; set; } }

public class LeaveBalanceAllQryHandler : IRequestHandler<LeaveBalanceAllQry, List<LeaveBalanceListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;
    private readonly ICorModClient _corMod;

    public LeaveBalanceAllQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro, ICorModClient corMod)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
        _corMod = corMod;
    }

    public async Task<List<LeaveBalanceListDto>> Handle(LeaveBalanceAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeaveBalance>().GetAll();
        var dataL = new List<LeaveBalanceListDto>();
        var empL = await _hrmPro.GetListEmp(cancellationToken);
        var fYrL = await _corMod.GetListFiscalYear(cancellationToken);
        var lPoL = await _unitOfWork.Repository<LeavePolicy>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL.Res.FirstOrDefault(t => t.Id == data.EmployeeId.ToString());
            var fyr = fYrL.Res.FirstOrDefault(t => t.Id == data.FiscalYearId.ToString());
            var lpo = lPoL.FirstOrDefault(t => t.Id == data.LeavePolicyId);
            var c = new LeaveBalanceListDto
            {
                Id = data.Id,
                EmployeeId = data.EmployeeId,
                FiscalYearId = data.FiscalYearId,
                LeavePolicyId = data.LeavePolicyId,
                Balance = data.Balance,
                Carried = data.Carried,
                CarriedExpireDate = data.CarriedExpireDate,
                LeavePolicy = lpo != null ? lpo.Name : "NOT AVAILABLE",
                EmployeeName = emp != null ? emp.Name : "NOT AVAILABLE",
                FiscalYear = fyr != null ? fyr.Name : "NOT AVAILABLE",
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

public class LeaveBalanceByIdQryHandler : IRequestHandler<LeaveBalanceByIdQry, LeaveBalanceListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;
    private readonly ICorModClient _corMod;

    public LeaveBalanceByIdQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro, ICorModClient corMod)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
        _corMod = corMod;
    }

    public async Task<LeaveBalanceListDto?> Handle(LeaveBalanceByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveBalance>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _hrmPro.GetEmp(data.EmployeeId.ToString(), cancellationToken);
        var fyr = await _corMod.GetFiscalYear(data.FiscalYearId.ToString(), cancellationToken);
        var lpo = await _unitOfWork.Repository<LeavePolicy>().GetById(data.LeavePolicyId);

        var c = new LeaveBalanceListDto
        {
            Id = data.Id,
            EmployeeId = data.EmployeeId,
            FiscalYearId = data.FiscalYearId,
            LeavePolicyId = data.LeavePolicyId,
            Balance = data.Balance,
            Carried = data.Carried,
            CarriedExpireDate = data.CarriedExpireDate,
            LeavePolicy = lpo != null ? lpo.Name : "NOT AVAILABLE",
            EmployeeName = emp.Res.Name != null ? emp.Res.Name : "NOT AVAILABLE",
            FiscalYear = fyr.Res.Name != null ? fyr.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

