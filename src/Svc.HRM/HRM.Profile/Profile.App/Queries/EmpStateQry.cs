using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmpStateByIdQry : IRequest<EmpStateListDto?> { public Guid Id { get; set; } }

public class EmpStateByIdQryHandler : IRequestHandler<EmpStateByIdQry, EmpStateListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public EmpStateByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpStateListDto?> Handle(EmpStateByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmpState>().GetFoD(e => e.EmployeeId == request.Id);
        if (data == null) { return null; }

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
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}