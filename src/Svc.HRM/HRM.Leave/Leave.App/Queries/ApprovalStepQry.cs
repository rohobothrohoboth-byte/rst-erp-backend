using Common;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class ApprovalStepAllQry : IRequest<List<ApprovalStepListDto>> { }
public class ApprovalStepByIdQry : IRequest<ApprovalStepListDto?> { public Guid Id { get; set; } }

public class ApprovalStepAllQryHandler : IRequestHandler<ApprovalStepAllQry, List<ApprovalStepListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;

    public ApprovalStepAllQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<List<ApprovalStepListDto>> Handle(ApprovalStepAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<ApprovalStep>().GetAll();
        var dataL = new List<ApprovalStepListDto>();
        var empL = await _hrmPro.GetListEmp(cancellationToken);

        foreach (var data in dbData)
        {
            var emp = empL.Res.FirstOrDefault(t => t.Id == data.ApprovedById.ToString());
            var c = new ApprovalStepListDto
            {
                Id = data.Id,
                ApprovedById = data.ApprovedById,
                LeaveRequestId = data.LeaveRequestId,
                StepOrder = data.StepOrder,
                IsApproved = data.IsApproved,
                Date = data.Date,
                Comments = data.Comments,
                IsApprovedStr = data.IsApproved.ToString(),
                ApprovedBy = emp != null ? emp.Name : "NOT AVAILABLE",
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

public class ApprovalStepByIdQryHandler : IRequestHandler<ApprovalStepByIdQry, ApprovalStepListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;

    public ApprovalStepByIdQryHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    public async Task<ApprovalStepListDto?> Handle(ApprovalStepByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<ApprovalStep>().GetById(request.Id);
        if (data == null) { return null; }
        var emp = await _hrmPro.GetEmp(data.ApprovedById.ToString(), cancellationToken);

        var c = new ApprovalStepListDto
        {
            Id = data.Id,
            ApprovedById = data.ApprovedById,
            LeaveRequestId = data.LeaveRequestId,
            StepOrder = data.StepOrder,
            IsApproved = data.IsApproved,
            Date = data.Date,
            Comments = data.Comments,
            IsApprovedStr = data.IsApproved.ToString(),
            ApprovedBy = emp.Res.Name != null ? emp.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}