using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpFinanceAllQry : IRequest<List<EmpFinanceListDto>> { }

public class EmpFinanceByIdQry : IRequest<EmpFinanceListDto?> { public Guid Id { get; set; } }

public class EmpFinanceAllQryHandler : IRequestHandler<EmpFinanceAllQry, List<EmpFinanceListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpFinanceAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EmpFinanceListDto>> Handle(EmpFinanceAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmpFinance>().GetAll();
        var dataL = new List<EmpFinanceListDto>();
        var empL = await _unitOfWork.Repository<Employee>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL!.FirstOrDefault(t => t.Id == data.EmployeeId);
            var c = new EmpFinanceListDto
            {
                Id = data.Id,
                EmployeeId = data.EmployeeId,
                Tin = data.Tin,
                BankAccountNo = data.BankAccountNo,
                PensionNumber = data.PensionNumber,
                EmpFullName = emp != null ? emp!.Person.FullName : "EMPLOYEE NOT AVAILABLE",
                EmpFullNameAm = emp != null ? emp!.Person.FullNameAm : "የሰራተኛው መረጃ ማግኘት አልተቻለም",
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

public class EmpFinanceByIdQryHandler : IRequestHandler<EmpFinanceByIdQry, EmpFinanceListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpFinanceByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpFinanceListDto?> Handle(EmpFinanceByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpFinance>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _unitOfWork.Repository<Employee>().GetById(data.EmployeeId);

        var c = new EmpFinanceListDto
        {
            Id = data.Id,
            EmployeeId = data.EmployeeId,
            Tin = data.Tin,
            BankAccountNo = data.BankAccountNo,
            PensionNumber = data.PensionNumber,
            EmpFullName = emp != null ? emp!.Person.FullName : "EMPLOYEE NOT AVAILABLE",
            EmpFullNameAm = emp != null ? emp!.Person.FullNameAm : "የሰራተኛው መረጃ ማግኘት አልተቻለም",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}