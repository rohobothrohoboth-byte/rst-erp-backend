using Leave.App.Interfaces;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class LeaveLedgerAllQry : IRequest<List<LeaveLedgerListDto>> { }
public class LeaveLedgerByIdQry : IRequest<LeaveLedgerListDto?> { public Guid Id { get; set; } }

public class LeaveLedgerAllQryHandler : IRequestHandler<LeaveLedgerAllQry, List<LeaveLedgerListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfile _hrmPro;

    public LeaveLedgerAllQryHandler(IUnitOfWork unitOfWork, IHrmProfile hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<List<LeaveLedgerListDto>> Handle(LeaveLedgerAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeaveLedger>().GetAll();
        var dataL = new List<LeaveLedgerListDto>();
        var empL = await _hrmPro.EmpList(cancellationToken);
        var lpoL = await _unitOfWork.Repository<LeavePolicy>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL!.FirstOrDefault(t => t.Id == data.EmployeeId);
            var lpo = lpoL.FirstOrDefault(t => t.Id == data.LeavePolicyId);

            var c = new LeaveLedgerListDto
            {
                Id = data.Id,
                Date = data.Date,
                Amount = $"{data.Amount:#,##0.##} days",
                EntryType = ((LedgerEntryType)Enum.Parse(typeof(LedgerEntryType), data.EntryType)).ToDisplayName(),
                SourceType = ((LedgerSourceType)Enum.Parse(typeof(LedgerSourceType), data.SourceType)).ToDisplayName(),
                BalanceAfter = $"{data.BalanceAfter:#,##0.##} days",
                EmployeeName = emp != null ? emp.Name : "NOT AVAILABLE",
                LeavePolicy = lpo != null ? lpo.Name : "NOT AVAILABLE",
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

public class LeaveLedgerByIdQryHandler : IRequestHandler<LeaveLedgerByIdQry, LeaveLedgerListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfile _hrmPro;

    public LeaveLedgerByIdQryHandler(IUnitOfWork unitOfWork, IHrmProfile hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<LeaveLedgerListDto?> Handle(LeaveLedgerByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveLedger>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _hrmPro.Emp(data.EmployeeId, cancellationToken);
        var lpo = await _unitOfWork.Repository<LeavePolicy>().GetById(data.LeavePolicyId);

        var c = new LeaveLedgerListDto
        {
            Id = data.Id,
            Date = data.Date,
            Amount = $"{data.Amount:#,##0.##} days",
            EntryType = ((LedgerEntryType)Enum.Parse(typeof(LedgerEntryType), data.EntryType)).ToDisplayName(),
            SourceType = ((LedgerSourceType)Enum.Parse(typeof(LedgerSourceType), data.SourceType)).ToDisplayName(),
            BalanceAfter = $"{data.BalanceAfter:#,##0.##} days",
            EmployeeName = emp != null ? emp.Name : "NOT AVAILABLE",
            LeavePolicy = lpo != null ? lpo.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}