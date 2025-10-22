using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpPensionCardByIdQry : IRequest<EmpPensionCardListDto?> { public Guid Id { get; set; } }

public class EmpPensionCardByIdQryHandler : IRequestHandler<EmpPensionCardByIdQry, EmpPensionCardListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpPensionCardByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpPensionCardListDto?> Handle(EmpPensionCardByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpPensionCard>().GetFoD(e => e.EmployeeId == request.Id);
        if (data == null) { return null; }

        var c = new EmpPensionCardListDto
        {
            Id = data.Id,
            EmployeeId = data.EmployeeId,
            RegistrationDate = data.RegistrationDate,
            SentDate = data.SentDate,
            ReceivedDate = data.ReceivedDate,
            IsReceived = data.IsReceived,
            IsSent = data.IsSent,
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