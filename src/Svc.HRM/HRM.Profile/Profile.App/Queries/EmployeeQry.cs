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
    private readonly ILup _lup;

    public EmployeeAllQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
        _lup = lup;
    }

    public async Task<List<EmployeeListDto>> Handle(EmployeeAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Employee>().GetAll();
        var dataL = new List<EmployeeListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var deL = await _corMod.DeptList(cancellationToken);
        var jgL = await _corHRMM.JobGradeList(cancellationToken);
        var poL = await _corHRMM.PositionList(cancellationToken);
        var eTL = await _lup.EmploymentTypeList(cancellationToken);
        var eNL = await _lup.EmploymentNatureList(cancellationToken);

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var et = eTL!.FirstOrDefault(t => t.Id == data.EmploymentTypeId);
            var en = eNL!.FirstOrDefault(t => t.Id == data.EmploymentNatureId);
            var dept = deL!.FirstOrDefault(t => t.Id == data.DepartmentId);
            var jg = jgL!.FirstOrDefault(t => t.Id == data.JobGradeId);
            var pos = poL!.FirstOrDefault(t => t.Id == data.PositionId);
            var c = new EmployeeListDto
            {
                Id = data.Id,
                PersonId = data.PersonId,
                JobGradeId = data.JobGradeId,
                PositionId = data.PositionId,
                DepartmentId = data.DepartmentId,
                EmploymentTypeId = data.EmploymentTypeId,
                EmploymentNatureId = data.EmploymentNatureId,
                Gender = per!.Gender,
                Nationality = per.Nationality,
                Code = data.Code,
                EmploymentDate = data.EmploymentDate,
                JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
                Position = pos != null ? $"{pos.Name}({pos.NameAm})" : "NOT AVAILABLE",
                Department = dept != null ? $"{dept.Name}({dept.NameAm})" : "NOT AVAILABLE",
                EmploymentType = et != null ? et.Name : "NOT AVAILABLE",
                EmploymentNature = en != null ? en.Name : "NOT AVAILABLE",
                GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
                EmpFullName = per.FullName,
                EmpFullNameAm = per.FullNameAm,
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
    private readonly ILup _lup;

    public EmployeeByIdQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod, ILup lup)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
        _lup = lup;
    }

    public async Task<EmployeeListDto?> Handle(EmployeeByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var dept = await _corMod.Dept(data.DepartmentId, cancellationToken);
        var jg = await _corHRMM.JobGrade(data.JobGradeId, cancellationToken);
        var pos = await _corHRMM.Position(data.PositionId, cancellationToken);
        var et = await _lup.EmploymentType(data.EmploymentTypeId, cancellationToken);
        var en = await _lup.EmploymentNature(data.EmploymentNatureId, cancellationToken);

        var c = new EmployeeListDto
        {
            Id = data.Id,
            PersonId = data.PersonId,
            JobGradeId = data.JobGradeId,
            PositionId = data.PositionId,
            DepartmentId = data.DepartmentId,
            EmploymentTypeId = data.EmploymentTypeId,
            EmploymentNatureId = data.EmploymentNatureId,
            Gender = per!.Gender,
            Nationality = per.Nationality,
            Code = data.Code,
            EmploymentDate = data.EmploymentDate,
            JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
            Position = pos != null ? $"{pos.Name}({pos.NameAm})" : "NOT AVAILABLE",
            Department = dept != null ? $"{dept.Name}({dept.NameAm})" : "NOT AVAILABLE",
            EmploymentType = et != null ? et.Name : "NOT AVAILABLE",
            EmploymentNature = en != null ? en.Name : "NOT AVAILABLE",
            GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            EmpFullName = per.FullName,
            EmpFullNameAm = per.FullNameAm,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}