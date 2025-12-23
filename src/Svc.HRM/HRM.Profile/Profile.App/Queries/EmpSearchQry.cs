using Common;
using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class SearchByCodeQry : IRequest<EmpSearchRes?> { public string Code { get; set; } = default!; }

public class SearchByCodeQryHandler : IRequestHandler<SearchByCodeQry, EmpSearchRes?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public SearchByCodeQryHandler(IUnitOfWork unitOfWork, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }


    public async Task<EmpSearchRes?> Handle(SearchByCodeQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetFoD(e => e.Code == request.Code);
        if (data == null) { throw new DomainException($"EMPLOYEE with Code [{request.Code}] NOT FOUND."); }
        var dept = await _corMod.GetDept(data.DepartmentId.ToString(), cancellationToken);
        var pos = await _corHRMM.GetPosition(data.PositionId.ToString(), cancellationToken);
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var ePhoto = await _unitOfWork.Repository<EmpPhoto>().GetFoD(b => b.EmployeeId == data.Id);
        var photo = "";

        if (ePhoto != null)
        {
            var ePhotoB = await _unitOfWork.Repository<EmpPhotoBlob>().GetFoD(t => t.FileMetaDataId == ePhoto.FileMetaDataId);
            photo = Convert.ToBase64String(ePhotoB!.Data);
        }

        var c = new EmpSearchRes
        {
            Id = data.Id,
            Photo = photo,
            FullName = $"{per!.FirstName} {per.MiddleName} {per.LastName}",
            FullNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
            Code = data.Code,
            Gender = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Position = pos.Res.Name != null ? pos.Res.Name : "NOT AVAILABLE",
            Dept = dept.Res.Name != null ? dept.Res.Name : "NOT AVAILABLE"
        };
        return c;
    }
}