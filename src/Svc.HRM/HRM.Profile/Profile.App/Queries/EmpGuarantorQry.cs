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
    private readonly ICorHRMM _corHRMM;
    private readonly ILup _lup;

    public EmpGuarantorByIdQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _lup = lup;
    }

    public async Task<EmpGuarantorListDto?> Handle(EmpGuarantorByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmergencyContact>().GetFoD(e => e.EmployeeId == request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var add = await _corHRMM.Address(data.AddressId, cancellationToken);
        var re = await _lup.Relation(data.RelationId, cancellationToken);

        var c = new EmpGuarantorListDto
        {
            Id = data.Id,
            PersonId = data.PersonId,
            AddressId = data.AddressId,
            RelationId = data.RelationId,
            EmployeeId = data.EmployeeId,
            Gender = per!.Gender,
            Nationality = per.Nationality,
            GuarantorName = per.FullName,
            GuarantorNameAm = per.FullNameAm,
            GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Address = add != null ? add.Name : "NOT AVAILABLE",
            Relation = re != null ? re.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}