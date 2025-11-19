using Leave.App.Interfaces;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeaveBalanceAllQry : IRequest<List<LeaveBalanceListDto>> { }
public class LeaveBalanceByIdQry : IRequest<LeaveBalanceListDto?> { public Guid Id { get; set; } }

public class LeaveBalanceAllQryHandler : IRequestHandler<LeaveBalanceAllQry, List<LeaveBalanceListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfile _hrmPro;
    private readonly ICorMod _corMod;

    public LeaveBalanceAllQryHandler(IUnitOfWork unitOfWork, IHrmProfile hrmPro, ICorMod corMod)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
        _corMod = corMod;
    }

    public async Task<List<LeaveBalanceListDto>> Handle(LeaveBalanceAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeaveBalance>().GetAll();
        var dataL = new List<LeaveBalanceListDto>();
        var empL = await _hrmPro.EmpList(cancellationToken);
        var fYrL = await _corMod.FiscalYearList(cancellationToken);
        var lPoL = await _unitOfWork.Repository<LeavePolicy>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL!.FirstOrDefault(t => t.Id == data.EmployeeId);
            var fyr = fYrL!.FirstOrDefault(t => t.Id == data.FiscalYearId);
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
    private readonly IHrmProfile _hrmPro;
    private readonly ICorMod _corMod;

    public LeaveBalanceByIdQryHandler(IUnitOfWork unitOfWork, IHrmProfile hrmPro, ICorMod corMod)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
        _corMod = corMod;
    }

    public async Task<LeaveBalanceListDto?> Handle(LeaveBalanceByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveBalance>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _hrmPro.Emp(data.EmployeeId, cancellationToken);
        var fyr = await _corMod.FiscalYear(data.FiscalYearId, cancellationToken);
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
            EmployeeName = emp != null ? emp.Name : "NOT AVAILABLE",
            FiscalYear = fyr != null ? fyr.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

