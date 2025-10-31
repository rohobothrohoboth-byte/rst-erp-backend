using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmployeeAllQry : IRequest<List<EmployeeListDto>> { }
public class EmployeeByIdQry : IRequest<EmployeeListDto?> { public Guid Id { get; set; } }

public class EmployeeAllQryHandler : IRequestHandler<EmployeeAllQry, List<EmployeeListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHRMM _corHRMM;
    private readonly ICorMod _corMod;

    public EmployeeAllQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<List<EmployeeListDto>> Handle(EmployeeAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Employee>().GetAll();
        var dataL = new List<EmployeeListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var deL = await _corMod.DeptList(cancellationToken);
        var jgL = await _corHRMM.JobGradeList(cancellationToken);
        var poL = await _corHRMM.PositionList(cancellationToken);
        var ePhotoL = await _unitOfWork.Repository<EmpPhoto>().GetAll();

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var dept = deL!.FirstOrDefault(t => t.Id == data.DepartmentId);
            var jg = jgL!.FirstOrDefault(t => t.Id == data.JobGradeId);
            var pos = poL!.FirstOrDefault(t => t.Id == data.PositionId);
            var photo = "";
            if (ePhotoL.Any())
            {
                var ePhoto = ePhotoL.FirstOrDefault(t => t.EmployeeId == data.Id);
                if (ePhoto != null)
                {
                    var ePhotoB = await _unitOfWork.Repository<EmpPhotoThumbnail>().GetFoD(t => t.FileMetaDataId == ePhoto!.ThumbnailId);
                    photo = Convert.ToBase64String(ePhotoB!.Data);
                }
            }
            
            var c = new EmployeeListDto
            {
                Id = data.Id,
                EmpFullName = per != null ? $"{per.FirstName} {per.MiddleName} {per.LastName}" : "NOT AVAILABLE",
                EmpFullNameAm = per != null ? $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}" : "NOT AVAILABLE",
                Code = data.Code,
                Gender = ((Gender)Enum.Parse(typeof(Gender), per!.Gender)).ToDisplayName(),
                Branch = dept != null ? dept.NameAm : "NOT AVAILABLE",
                Department = dept != null ? dept.Name : "NOT AVAILABLE",
                Position = pos != null ? pos.Name : "NOT AVAILABLE",
                JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
                EmpType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
                EmpNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName(),
                Photo = photo,
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

public class EmployeeByIdQryHandler : IRequestHandler<EmployeeByIdQry, EmployeeListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHRMM _corHRMM;
    private readonly ICorMod _corMod;

    public EmployeeByIdQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<EmployeeListDto?> Handle(EmployeeByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var dept = await _corMod.Dept(data.DepartmentId, cancellationToken);
        var jg = await _corHRMM.JobGrade(data.JobGradeId, cancellationToken);
        var pos = await _corHRMM.Position(data.PositionId, cancellationToken);
        var ePhoto = await _unitOfWork.Repository<EmpPhoto>().GetFoD(t => t.EmployeeId == request.Id);
        var ePhotoB = await _unitOfWork.Repository<EmpPhotoThumbnail>().GetFoD(t => t.FileMetaDataId == ePhoto!.FileMetaDataId);

        var c = new EmployeeListDto
        {
            Id = data.Id,
            EmpFullName = $"{per!.FirstName} {per.MiddleName} {per.LastName}",
            EmpFullNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
            Code = data.Code,
            Gender = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Branch = dept != null ? dept.NameAm : "NOT AVAILABLE",
            Department = dept != null ? dept.Name : "NOT AVAILABLE",
            Position = pos != null ? pos.Name : "NOT AVAILABLE",
            JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
            EmpType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
            EmpNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName(),
            Photo = Convert.ToBase64String(ePhotoB!.Data),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}