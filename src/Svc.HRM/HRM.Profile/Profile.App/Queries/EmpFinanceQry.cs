using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpFinanceByIdQry : IRequest<EmpFinanceListDto?> { public Guid Id { get; set; } }

public class EmpFinanceByIdQryHandler : IRequestHandler<EmpFinanceByIdQry, EmpFinanceListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpFinanceByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpFinanceListDto?> Handle(EmpFinanceByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpFinance>().GetFoD(e => e.EmployeeId == request.Id);
        if (data == null) { return null; }

        var c = new EmpFinanceListDto
        {
            Id = data.Id,
            EmployeeId = data.EmployeeId,
            Tin = data.Tin,
            BankAccountNo = data.BankAccountNo,
            PensionNumber = data.PensionNumber,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}