using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpStateAllQry : IRequest<List<EmpStateListDto>> { }

public class EmpStateByIdQry : IRequest<EmpStateListDto?> { public Guid Id { get; set; } }

public class EmpStateAllQryHandler : IRequestHandler<EmpStateAllQry, List<EmpStateListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpStateAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EmpStateListDto>> Handle(EmpStateAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmpState>().GetAll();
        var dataL = new List<EmpStateListDto>();
        var empL = await _unitOfWork.Repository<Employee>().GetAll();

        foreach (var data in dbData)
        {
            var emp = empL!.FirstOrDefault(t => t.Id == data.EmployeeId);
            var c = new EmpStateListDto
            {
                Id = data.Id,
                EmployeeId = data.EmployeeId,
                IsTerminated = data.IsTerminated,
                IsApproved = data.IsApproved,
                IsStandBy = data.IsStandBy,
                IsRetired = data.IsRetired,
                IsUnderProbation = data.IsUnderProbation,
                IsTerminatedStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsTerminated)).ToDisplayName(),
                IsApprovedStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsApproved)).ToDisplayName(),
                IsStandByStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsStandBy)).ToDisplayName(),
                IsRetiredStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsRetired)).ToDisplayName(),
                IsUnderProbationStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsUnderProbation)).ToDisplayName(),
                EmpFullName = emp != null ? emp!.Person.FullName : "NOT AVAILABLE",
                EmpFullNameAm = emp != null ? emp!.Person.FullNameAm : "የመረጃ ማግኘት አልተቻለም",
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

public class EmpStateByIdQryHandler : IRequestHandler<EmpStateByIdQry, EmpStateListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpStateByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpStateListDto?> Handle(EmpStateByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpState>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _unitOfWork.Repository<Employee>().GetById(data.EmployeeId);

        var c = new EmpStateListDto
        {
            Id = data.Id,
            EmployeeId = data.EmployeeId,
            IsTerminated = data.IsTerminated,
            IsApproved = data.IsApproved,
            IsStandBy = data.IsStandBy,
            IsRetired = data.IsRetired,
            IsUnderProbation = data.IsUnderProbation,
            IsTerminatedStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsTerminated)).ToDisplayName(),
            IsApprovedStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsApproved)).ToDisplayName(),
            IsStandByStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsStandBy)).ToDisplayName(),
            IsRetiredStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsRetired)).ToDisplayName(),
            IsUnderProbationStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsUnderProbation)).ToDisplayName(),
            EmpFullName = emp != null ? emp!.Person.FullName : "NOT AVAILABLE",
            EmpFullNameAm = emp != null ? emp!.Person.FullNameAm : "የመረጃ ማግኘት አልተቻለም",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}