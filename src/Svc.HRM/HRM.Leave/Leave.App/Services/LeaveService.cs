using Leave.App.Interfaces;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public class LeaveService
{
    private readonly IUnitOfWork _unitOfWork;

    public LeaveService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<double> GetBalance(Guid employeeId, Guid leaveTypeId)
    {
        var dbData = (await _unitOfWork.Repository<LeaveLedger>().Find(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId)).ToList();
        return dbData.Sum(x => x.Amount);
    }




}