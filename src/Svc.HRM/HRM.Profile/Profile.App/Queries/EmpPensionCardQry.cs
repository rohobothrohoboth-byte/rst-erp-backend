using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpPensionCardAllQry : IRequest<List<EmpPensionCardListDto>> { }

public class EmpPensionCardByIdQry : IRequest<EmpPensionCardListDto?> { public Guid Id { get; set; } }

public class EmpPensionCardAllQryHandler : IRequestHandler<EmpPensionCardAllQry, List<EmpPensionCardListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpPensionCardAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EmpPensionCardListDto>> Handle(EmpPensionCardAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmpPensionCard>().GetAll();
        var dataL = new List<EmpPensionCardListDto>();
        var empL = await _unitOfWork.Repository<Employee>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL.FirstOrDefault(t => t.Id == data.EmployeeId);
            var c = new EmpPensionCardListDto
            {
                Id = data.Id,
                EmployeeId = data.EmployeeId,
                RegistrationDate = data.RegistrationDate,
                SentDate = data.SentDate,
                ReceivedDate = data.ReceivedDate,
                IsReceived = data.IsReceived,
                IsSent = data.IsSent,
                EmpFullName = emp != null ? emp!.Person.FullName : "NOT AVAILABLE",
                EmpFullNameAm = emp != null ? emp!.Person.FullNameAm : "የመረጃ ማግኘት አልተቻለም",
                IsReceivedStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsReceived)).ToDisplayName(),
                IsSentStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsSent)).ToDisplayName(),
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

public class EmpPensionCardByIdQryHandler : IRequestHandler<EmpPensionCardByIdQry, EmpPensionCardListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpPensionCardByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpPensionCardListDto?> Handle(EmpPensionCardByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpPensionCard>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _unitOfWork.Repository<Employee>().GetById(data.EmployeeId);

        var c = new EmpPensionCardListDto
        {
            Id = data.Id,
            EmployeeId = data.EmployeeId,
            RegistrationDate = data.RegistrationDate,
            SentDate = data.SentDate,
            ReceivedDate = data.ReceivedDate,
            IsReceived = data.IsReceived,
            IsSent = data.IsSent,
            EmpFullName = emp != null ? emp!.Person.FullName : "NOT AVAILABLE",
            EmpFullNameAm = emp != null ? emp!.Person.FullNameAm : "የመረጃ ማግኘት አልተቻለም",
            IsReceivedStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsReceived)).ToDisplayName(),
            IsSentStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsSent)).ToDisplayName(),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}