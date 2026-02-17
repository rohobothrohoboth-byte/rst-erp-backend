using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class WorkforcePlanAllQry : IRequest<List<WorkforcePlanListDto>> { public Guid Id { get; set; } }
public class WorkforcePlanByIdQry : IRequest<WorkforcePlanListDto?> { public Guid Id { get; set; } }

public class WorkforcePlanAllHandler : IRequestHandler<WorkforcePlanAllQry, List<WorkforcePlanListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;
    private readonly IHrmProfileClient _hrmProfileClient;

    public WorkforcePlanAllHandler(IUnitOfWork unitOfWork, ICorModClient corModClient, IHrmProfileClient hrmProfileClient)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
        _hrmProfileClient = hrmProfileClient;
    }

    public async Task<List<WorkforcePlanListDto>> Handle(WorkforcePlanAllQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<WorkforcePlan>().GetAll()).ToList();
        var dataL = new List<WorkforcePlanListDto>();
        if (dbData.Count <= 0) { return dataL; }
        var deptL = await _corModClient.GetListDept(cancellationToken);
        var perL = await _corModClient.GetListPeriod(cancellationToken);
        var empL = await _hrmProfileClient.GetListEmp(cancellationToken);

        foreach (var data in dbData)
        {
            var perV = perL.Res.FirstOrDefault(t => t.Id == data.PeriodId.ToString());
            var empV = empL.Res.FirstOrDefault(t => t.Id == data.RequistionById.ToString());
            var deptV = deptL.Res.FirstOrDefault(r => r.Id == data.DepartmentId.ToString());

            var per = "NOT AVAILABLE";
            var emp = "NOT AVAILABLE";
            var dept = "NOT AVAILABLE";
            if (perV != null) { per = perV.Name; }
            if (empV != null) { emp = empV.Name; }
            if (deptV != null) { dept = deptV.Name; }
            var c = new WorkforcePlanListDto
            {
                Id = data.Id,
                PeriodId = data.PeriodId,
                PlanCode = data.PlanCode,
                Title = data.Title,
                Desc = data.Desc,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                TotalPositions = data.TotalPositions,
                AppPositions = data.AppPositions,
                Status = data.Status,
                StatusStr = ((ReqStatus)Enum.Parse(typeof(ReqStatus), data.Status)).ToDisplayName(),
                Department = dept,
                Period = per,
                RequistionBy = emp,
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

public class WorkforcePlanByIdHandler : IRequestHandler<WorkforcePlanByIdQry, WorkforcePlanListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorModClient _corModClient;
    private readonly IHrmProfileClient _hrmProfileClient;

    public WorkforcePlanByIdHandler(IUnitOfWork unitOfWork, ICorModClient corModClient, IHrmProfileClient hrmProfileClient)
    {
        _unitOfWork = unitOfWork;
        _corModClient = corModClient;
        _hrmProfileClient = hrmProfileClient;
    }

    public async Task<WorkforcePlanListDto?> Handle(WorkforcePlanByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<WorkforcePlan>().GetById(request.Id);
        if (data == null) { return null; }
        var dept = await _corModClient.GetDept(data.DepartmentId.ToString(), cancellationToken);
        var emp = await _hrmProfileClient.GetEmp(data.RequistionById.ToString(), cancellationToken);
        var per = "NOT AVAILABLE";
        if (data.PeriodId != null)
        {
            var peRes = await _corModClient.GetPeriod(data.PeriodId.ToString()!, cancellationToken);
            if (peRes.Id != null) { per = peRes.Name; }
        }

        var c = new WorkforcePlanListDto
        {
            Id = data.Id,
            PeriodId = data.PeriodId,
            PlanCode = data.PlanCode,
            Title = data.Title,
            Desc = data.Desc,
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            TotalPositions = data.TotalPositions,
            AppPositions = data.AppPositions,
            Status = data.Status,
            StatusStr = ((ReqStatus)Enum.Parse(typeof(ReqStatus), data.Status)).ToDisplayName(),
            Department = dept.Res.Name ?? "NOT AVAILABLE",
            Period = per,
            RequistionBy = emp.Res.Name ?? "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}