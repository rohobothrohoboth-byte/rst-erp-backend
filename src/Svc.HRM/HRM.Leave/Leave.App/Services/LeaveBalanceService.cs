using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public interface ILeaveBalanceService
{
    Task<double> GetBalanceAsync(Guid employeeId, Guid leaveTypeId);
    Task EnsureSufficientAsync(Guid employeeId, Guid leaveTypeId, double days);
}

public sealed class LeaveBalanceService : ILeaveBalanceService
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveBalanceService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<double> GetBalanceAsync(Guid employeeId, Guid leaveTypeId)
    {
        var lvLedger = (await _unitOfWork.Repository<LeaveLedger>().Find(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId)).ToList();
        return lvLedger.Sum(x => x.Amount);
    }

    public async Task EnsureSufficientAsync(Guid employeeId, Guid leaveTypeId, double days)
    {
        var balance = await GetBalanceAsync(employeeId, leaveTypeId);
        if (balance < days) { throw new DomainException("Insufficient leave balance"); }
    }
}