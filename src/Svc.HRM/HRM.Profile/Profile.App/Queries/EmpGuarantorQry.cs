using Common;
using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpGuarantorByIdQry : IRequest<EmpGuarantorListDto?> { public Guid Id { get; set; } }

public class EmpGuarantorByIdQryHandler : IRequestHandler<EmpGuarantorByIdQry, EmpGuarantorListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;
    private readonly IMediator _med;

    public EmpGuarantorByIdQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient, IMediator med)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
        _med = med;
    }

    public async Task<EmpGuarantorListDto?> Handle(EmpGuarantorByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmergencyContact>().GetFoD(e => e.EmployeeId == request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var add = await _med.Send(new AddressNameByIdQry { Id = data.AddressId }, cancellationToken);
        var re = await _lupClient.GetRel(data.RelationId.ToString(), cancellationToken);

        var c = new EmpGuarantorListDto
        {
            Id = data.Id,
            PersonId = data.PersonId,
            AddressId = data.AddressId,
            RelationId = data.RelationId,
            EmployeeId = data.EmployeeId,
            Gender = per!.Gender,
            Nationality = per.Nationality,
            GuarantorName = $"{per.FirstName} {per.MiddleName} {per.LastName}",
            GuarantorNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
            GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Address = add != null ? add.Name : "NOT AVAILABLE",
            Relation = re.Res.Name != null ? re.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}