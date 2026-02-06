using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class AppStepByChainIdQry : IRequest<List<LeaveAppStepListDto>> { public Guid Id { get; set; } }
public class LeaveAppStepByIdQry : IRequest<LeaveAppStepListDto?> { public Guid Id { get; set; } }

public class AppStepByChainIdHandler : IRequestHandler<AppStepByChainIdQry, List<LeaveAppStepListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmProfileClient;

    public AppStepByChainIdHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmProfileClient)
    {
        _unitOfWork = unitOfWork;
        _hrmProfileClient = hrmProfileClient;
    }

    public async Task<List<LeaveAppStepListDto>> Handle(AppStepByChainIdQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<LeaveAppStep>().Find(c => c.LeaveAppChainId == request.Id)).ToList();
        var dataL = new List<LeaveAppStepListDto>();
        if (dbData.Count <= 0) { return dataL; }
        var appC = await _unitOfWork.Repository<LeaveAppChain>().GetById(request.Id);
        var empL = await _hrmProfileClient.GetListEmp(cancellationToken);

        foreach (var data in dbData)
        {
            var empN = "NOT ASSIGNED";
            if (data.EmployeeId != null)
            {
                var emp = empL.Res.FirstOrDefault(f => f.Id == data.EmployeeId.ToString());
                empN = emp!.Name;
            }

            var c = new LeaveAppStepListDto
            {
                Id = data.Id,
                StepName = data.StepName,
                StepOrder = data.StepOrder,
                Role = data.Role,
                IsFinal = data.IsFinal,
                RoleStr = ((ApprovalRole)Enum.Parse(typeof(ApprovalRole), data.Role)).ToDisplayName(),
                IsFinalStr = BoolToStr.FormatBool(data.IsFinal),
                Employee = empN,
                LeaveAppChain = $"From : {appC!.EffectiveFrom:MMMM dd, yyyy}",
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

public class LeaveAppStepByIdHandler : IRequestHandler<LeaveAppStepByIdQry, LeaveAppStepListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmProfileClient;

    public LeaveAppStepByIdHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmProfileClient)
    {
        _unitOfWork = unitOfWork;
        _hrmProfileClient = hrmProfileClient;
    }

    public async Task<LeaveAppStepListDto?> Handle(LeaveAppStepByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeaveAppStep>().GetById(request.Id);
        if (data == null) { return null; }
        var appC = await _unitOfWork.Repository<LeaveAppChain>().GetById(data.LeaveAppChainId);

        var empN = "NOT ASSIGNED";
        if (data.EmployeeId != null)
        {
            var emp = await _hrmProfileClient.GetEmp(((Guid)data.EmployeeId).ToString(), cancellationToken);
            empN = emp.Res.Name;
        }

        var c = new LeaveAppStepListDto
        {
            Id = data.Id,
            StepName = data.StepName,
            StepOrder = data.StepOrder,
            Role = data.Role,
            IsFinal = data.IsFinal,
            RoleStr = ((ApprovalRole)Enum.Parse(typeof(ApprovalRole), data.Role)).ToDisplayName(),
            IsFinalStr = BoolToStr.FormatBool(data.IsFinal),
            Employee = empN,
            LeaveAppChain = $"From : {appC!.EffectiveFrom:MMMM dd, yyyy}",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}