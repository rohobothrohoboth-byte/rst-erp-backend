using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpFamilyAllQry : IRequest<List<EmpFamilyListDto>> { public Guid Id { get; set; } }

public class EmpFamilyByIdQry : IRequest<EmpFamilyListDto?> { public Guid Id { get; set; } }

public class EmpFamilyAllQryHandler : IRequestHandler<EmpFamilyAllQry, List<EmpFamilyListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILup _lup;

    public EmpFamilyAllQryHandler(IUnitOfWork unitOfWork, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _lup = lup;
    }

    public async Task<List<EmpFamilyListDto>> Handle(EmpFamilyAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmpFamily>().Find(e => e.EmployeeId == request.Id);
        var dataL = new List<EmpFamilyListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var reL = await _lup.RelationList(cancellationToken);

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var re = reL!.FirstOrDefault(t => t.Id == data.RelationId);
            var c = new EmpFamilyListDto
            {
                Id = data.Id,
                PersonId = data.PersonId,
                RelationId = data.RelationId,
                EmployeeId = data.EmployeeId,
                Gender = per!.Gender,
                Nationality = per.Nationality,
                FamilyName = per.FullName,
                FamilyNameAm = per.FullNameAm,
                GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
                Relation = re != null ? re.Name : "NOT AVAILABLE",
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
    private readonly ILup _lup;

    public EmpFamilyByIdQryHandler(IUnitOfWork unitOfWork, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _lup = lup;
    }

    public async Task<EmpFamilyListDto?> Handle(EmpFamilyByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpFamily>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var emp = await _unitOfWork.Repository<Employee>().GetById(data.EmployeeId);
        var re = await _lup.Relation(data.RelationId, cancellationToken);

        var c = new EmpFamilyListDto
        {
            Id = data.Id,
            PersonId = data.PersonId,
            RelationId = data.RelationId,
            EmployeeId = data.EmployeeId,
            Gender = per!.Gender,
            Nationality = per.Nationality,
            FamilyName = per.FullName,
            FamilyNameAm = per.FullNameAm,
            GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Relation = re != null ? re.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}