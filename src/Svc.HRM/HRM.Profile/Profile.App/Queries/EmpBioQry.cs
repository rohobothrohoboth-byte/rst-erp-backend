using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpBioAllQry : IRequest<List<EmpBioListDto>> { }

public class EmpBioByIdQry : IRequest<EmpBioListDto?> { public Guid Id { get; set; } }

public class EmpBioAllQryHandler : IRequestHandler<EmpBioAllQry, List<EmpBioListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILup _lup;

    public EmpBioAllQryHandler(IUnitOfWork unitOfWork, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _lup = lup;
    }

    public async Task<List<EmpBioListDto>> Handle(EmpBioAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmpBio>().GetAll();
        var dataL = new List<EmpBioListDto>();
        var empL = await _unitOfWork.Repository<Employee>().GetAll();
        var marL = await _lup.MaritalStatusList(cancellationToken);

        foreach (var data in dbData)
        {
            var mar = marL!.FirstOrDefault(t => t.Id == data.MaritalStatusId);
            var emp = empL!.FirstOrDefault(t => t.Id == data.EmployeeId);
            var c = new EmpBioListDto
            {
                Id = data.Id,
                MaritalStatusId = data.MaritalStatusId,
                EmployeeId = data.EmployeeId,
                BirthLocation = data.BirthLocation,
                MotherFullName = data.MotherFullName,
                HasBirthCert = data.HasBirthCert,
                HasMarriageCert = data.HasMarriageCert,
                HasBirthCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasBirthCert)).ToDisplayName(),
                HasMarriageCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasMarriageCert)).ToDisplayName(),
                MaritalStatus = mar != null ? mar.Name : "MARITAL STATUS NOT AVAILABLE",
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

public class EmpBioByIdQryHandler : IRequestHandler<EmpBioByIdQry, EmpBioListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILup _lup;

    public EmpBioByIdQryHandler(IUnitOfWork unitOfWork, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _lup = lup;
    }

    public async Task<EmpBioListDto?> Handle(EmpBioByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpBio>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _unitOfWork.Repository<Employee>().GetById(data.EmployeeId);
        var mar = await _lup.MaritalStatus(data.MaritalStatusId, cancellationToken);

        var c = new EmpBioListDto
        {
            Id = data.Id,
            MaritalStatusId = data.MaritalStatusId,
            EmployeeId = data.EmployeeId,
            BirthLocation = data.BirthLocation,
            MotherFullName = data.MotherFullName,
            HasBirthCert = data.HasBirthCert,
            HasMarriageCert = data.HasMarriageCert,
            HasBirthCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasBirthCert)).ToDisplayName(),
            HasMarriageCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasMarriageCert)).ToDisplayName(),
            MaritalStatus = mar != null ? mar.Name : "MARITAL STATUS NOT AVAILABLE",
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