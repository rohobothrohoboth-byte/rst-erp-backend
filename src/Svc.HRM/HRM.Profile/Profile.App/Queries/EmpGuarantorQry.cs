using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpGuarantorAllQry : IRequest<List<EmpGuarantorListDto>> { }

public class EmpGuarantorByIdQry : IRequest<EmpGuarantorListDto?> { public Guid Id { get; set; } }

public class EmpGuarantorAllQryHandler : IRequestHandler<EmpGuarantorAllQry, List<EmpGuarantorListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHRMM _corHRMM;
    private readonly ILup _lup;

    public EmpGuarantorAllQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _lup = lup;
    }

    public async Task<List<EmpGuarantorListDto>> Handle(EmpGuarantorAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmergencyContact>().GetAll();
        var dataL = new List<EmpGuarantorListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var empL = await _unitOfWork.Repository<Employee>().GetAll();
        var addL = await _corHRMM.AddressList(cancellationToken);
        var reL = await _lup.RelationList(cancellationToken);

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var add = addL!.FirstOrDefault(t => t.Id == data.AddressId);
            var re = reL!.FirstOrDefault(t => t.Id == data.RelationId);
            var emp = empL!.FirstOrDefault(t => t.Id == data.EmployeeId);
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
                Address = add != null ? add.Telephone : "ADDRESS NOT AVAILABLE",
                Relation = re != null ? re.Name : "RELATION NOT AVAILABLE",
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
        var data = await _unitOfWork.Repository<EmergencyContact>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var emp = await _unitOfWork.Repository<Employee>().GetById(data.EmployeeId);
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
            Address = add != null ? add.Telephone : "ADDRESS NOT AVAILABLE",
            Relation = re != null ? re.Name : "RELATION NOT AVAILABLE",
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