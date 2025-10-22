using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpBioByIdQry : IRequest<EmpBioListDto?> { public Guid Id { get; set; } }

public class EmpBioByIdQryHandler : IRequestHandler<EmpBioByIdQry, EmpBioListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHRMM _corHRMM;
    private readonly ILup _lup;

    public EmpBioByIdQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _lup = lup;
    }

    public async Task<EmpBioListDto?> Handle(EmpBioByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpBio>().GetFoD(e=>e.EmployeeId == request.Id);
        if (data == null) { return null; }
        var add = await _corHRMM.Address(data.AddressId, cancellationToken);
        var mar = await _lup.MaritalStatus(data.MaritalStatusId, cancellationToken);

        var c = new EmpBioListDto
        {
            Id = data.Id,
            MaritalStatusId = data.MaritalStatusId,
            AddressId = data.AddressId,
            EmployeeId = data.EmployeeId,
            BirthLocation = data.BirthLocation,
            MotherFullName = data.MotherFullName,
            HasBirthCert = data.HasBirthCert,
            HasMarriageCert = data.HasMarriageCert,
            HasBirthCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasBirthCert)).ToDisplayName(),
            HasMarriageCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasMarriageCert)).ToDisplayName(),
            Address = add != null ? add.Name : "NOT AVAILABLE",
            MaritalStatus = mar != null ? mar.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}