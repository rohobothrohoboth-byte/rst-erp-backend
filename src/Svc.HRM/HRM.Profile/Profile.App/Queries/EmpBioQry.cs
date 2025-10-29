using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpBioByIdQry : IRequest<EmpBioListDto?> { public Guid Id { get; set; } }

public class EmpBioByIdQryHandler : IRequestHandler<EmpBioByIdQry, EmpBioListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpBioByIdQryHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _unitOfWork = unitOfWork;
        _med = med;
    }

    public async Task<EmpBioListDto?> Handle(EmpBioByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpBio>().GetFoD(e=>e.EmployeeId == request.Id);
        if (data == null) { return null; }
        var add = await _med.Send(new AddressNameByIdQry { Id = data.AddressId }, cancellationToken);

        var c = new EmpBioListDto
        {
            Id = data.Id,
            MaritalStatus = data.MaritalStatus,
            AddressId = data.AddressId,
            EmployeeId = data.EmployeeId,
            BirthLocation = data.BirthLocation,
            MotherFullName = data.MotherFullName,
            HasBirthCert = data.HasBirthCert,
            HasMarriageCert = data.HasMarriageCert,
            HasBirthCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasBirthCert)).ToDisplayName(),
            HasMarriageCertStr = ((YesNo)Enum.Parse(typeof(YesNo), data.HasMarriageCert)).ToDisplayName(),
            MaritalStatusStr = ((MaritalStat)Enum.Parse(typeof(MaritalStat), data.MaritalStatus)).ToDisplayName(),
            Address = add != null ? add.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}