using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.Domain.Entities;
using Leave.Domain.Enums;

namespace Leave.App.Services;

public interface ILeaveValService
{
    Task<bool> CheckOverlap(Guid empId, DateTime start, DateTime end);
    //Task CheckEligibility(Guid employeeId, Guid leaveTypeId, double days);
}

public class LeaveValService : ILeaveValService
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveValService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<bool> CheckOverlap(Guid empId, DateTime start, DateTime end)
    {
        var stat = BoolToStr.EnumToString(Status.Approved);
        var lvLedger = await _unitOfWork.Repository<LeaveRequest>().GetFoD(x => x.EmployeeId == empId && x.Status == stat && x.StartDate <= end && x.EndDate >= start);
        return lvLedger != null;
    }
}
