using Common;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpFamilyAllQry : IRequest<List<EmpFamilyListDto>> { public Guid Id { get; set; } }

public class EmpFamilyByIdQry : IRequest<EmpFamilyListDto?> { public Guid Id { get; set; } }

public class EmpFamilyAllQryHandler : IRequestHandler<EmpFamilyAllQry, List<EmpFamilyListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _gRPC;

    public EmpFamilyAllQryHandler(IUnitOfWork unitOfWork, ILupClient gRPC)
    {
        _unitOfWork = unitOfWork;
        _gRPC = gRPC;
    }

    public async Task<List<EmpFamilyListDto>> Handle(EmpFamilyAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmpFamily>().Find(e => e.EmployeeId == request.Id);
        var dataL = new List<EmpFamilyListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var rel = await _gRPC.GetRelList(cancellationToken);

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var reV = rel.Res.FirstOrDefault(r => r.Id == data.RelationId.ToString());
            var re = "NOT AVAILABLE";
            if (reV.Id != null) { re = reV.Name; }
            var c = new EmpFamilyListDto
            {
                Id = data.Id,
                PersonId = data.PersonId,
                RelationId = data.RelationId,
                EmployeeId = data.EmployeeId,
                Gender = per!.Gender,
                Nationality = per.Nationality,
                FamilyName = $"{per.FirstName} {per.MiddleName} {per.LastName}",
                FamilyNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
                GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
                Relation = re,
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

public class EmpFamilyByIdQryHandler : IRequestHandler<EmpFamilyByIdQry, EmpFamilyListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _gRPC;

    public EmpFamilyByIdQryHandler(IUnitOfWork unitOfWork, ILupClient gRPC)
    {
        _unitOfWork = unitOfWork;
        _gRPC = gRPC;
    }

    public async Task<EmpFamilyListDto?> Handle(EmpFamilyByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpFamily>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var emp = await _unitOfWork.Repository<Employee>().GetById(data.EmployeeId);
        var re = await _gRPC.GetRel(data.RelationId.ToString(), cancellationToken);

        var c = new EmpFamilyListDto
        {
            Id = data.Id,
            PersonId = data.PersonId,
            RelationId = data.RelationId,
            EmployeeId = data.EmployeeId,
            Gender = per!.Gender,
            Nationality = per.Nationality,
            FamilyName = $"{per.FirstName} {per.MiddleName} {per.LastName}",
            FamilyNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
            GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Relation = re.Res.Name != null ? re.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}