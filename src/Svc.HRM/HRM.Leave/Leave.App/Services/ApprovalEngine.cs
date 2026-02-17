using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public interface IApprovalEngine
{
    Task<LeaveAppRes> InitApproval(Guid id);
}

public class ApprovalEngine : IApprovalEngine
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeaveLedgerService _leaveLedgerService;

    public ApprovalEngine(IUnitOfWork unitOfWork, ILeaveLedgerService leaveLedgerService)
    {
        _unitOfWork = unitOfWork;
        _leaveLedgerService = leaveLedgerService;
    }

    public async Task<LeaveAppRes> InitApproval(Guid id)
    {
        var result = new LeaveAppRes();
        var req = await _unitOfWork.Repository<LeaveRequest>().GetById(id);
        if (req == null)
        {
            result.AddError("Leave request NOT FOUND.");
            return result;
        }

        var empId = req.EmployeeId;
        var leaveTypeId = req.LeaveTypeId;
        var requestDate = req.StartDate;
        var empLeavePolicy = await GetActiveEmpLeavePolicy(empId, leaveTypeId, requestDate);
        if (empLeavePolicy == null)
        {
            result.AddError("NO ACTIVE LEAVE POLICY FOUND for this employee and leave type.");
            return result;
        }

        var approvalSteps = await GetApprovalSteps(id);

        if (!approvalSteps.Any())
        {
            // No approval required - auto-approve
            await AutoAppLeaveReq(req);
            result.Status = "Approved";
            result.Message = "Leave request auto-approved (no approval chain configured)";
            result.RequiresApproval = false;
            return result;
        }

        // Set to pending and start at first step
        req.Status = BoolToStr.EnumToString(Status.Pending);
        req.CurrentAppStep = 1;
        await _unitOfWork.Repository<LeaveRequest>().Update(req);

        result.Status = "Pending";
        result.CurrentStep = 1;
        result.TotalSteps = approvalSteps.Count;
        result.NextApprover = await GetNextAppInfo(req.Id, 1);
        result.RequiresApproval = true;
        result.Message = $"Leave request submitted for approval (Step 1 of {approvalSteps.Count})";

        return result;
    }





    private async Task<EmpLeavePolicy?> GetActiveEmpLeavePolicy(Guid empId, Guid leaveTypeId, DateTime requestDate)
    {
        var ePolicyL = (await _unitOfWork.Repository<EmpLeavePolicy>().Find(elp => elp.EmployeeId == empId && elp.LeaveTypeId == leaveTypeId && elp.EffectiveFrom <= requestDate)).ToList();
        if (ePolicyL.Count <= 0) { return null; }
        var ePolicy = ePolicyL.FirstOrDefault(e => e.IsActive(requestDate));
        return ePolicy;
    }

    public async Task<LeaveAppChain?> GetActiveApprovalChain(Guid leavePolicyId, DateTime effectiveDate)
    {
        var aChainL = (await _unitOfWork.Repository<LeaveAppChain>().Find(lac => lac.LeavePolicyId == leavePolicyId && lac.IsActive && lac.EffectiveFrom <= effectiveDate)).ToList();
        var aChain = aChainL.FirstOrDefault(lac => lac.EffectiveTo == null || lac.EffectiveTo >= effectiveDate);
        return aChain;
    }

    public async Task<List<LeaveAppStep>> GetApprovalSteps(Guid id)
    {
        var req = await _unitOfWork.Repository<LeaveRequest>().GetById(id) ?? throw new DomainException("Leave request NOT FOUND.");
        var eLp = await GetActiveEmpLeavePolicy(req.EmployeeId, req.LeaveTypeId, req.StartDate) ?? throw new DomainException("NO ACTIVE leave policy found.");
        var aChain = await GetActiveApprovalChain(eLp.LeavePolicyId, req.StartDate);
        return aChain?.Steps.OrderBy(s => s.StepOrder).ToList() ?? [];
    }

    private async Task AutoAppLeaveReq(LeaveRequest leaveRequest)
    {
        leaveRequest.Status = BoolToStr.EnumToString(Status.Approved);
        leaveRequest.DateApproved = DateTime.UtcNow;
        await _unitOfWork.Repository<LeaveRequest>().Update(leaveRequest);

        var dto = new LedgerEntryDto
        {
            EmployeeId = leaveRequest.EmployeeId,
            LeaveTypeId = leaveRequest.LeaveTypeId,
            LeavePolicyId = null,
            Amount = -leaveRequest.DaysRequested,
            EntryType = BoolToStr.EnumToString(LedgerEntryType.Debit),
            SourceType = BoolToStr.EnumToString(LedgerSource.LeaveReq),
            ReferenceId = leaveRequest.Id
        };

        await _leaveLedgerService.Credit(dto);
    }

    private async Task<ApproverInfo?> GetNextAppInfo(Guid leaveRequestId, int stepOrder)
    {
        var approvalSteps = await GetApprovalSteps(leaveRequestId);
        var nextStep = approvalSteps.FirstOrDefault(s => s.StepOrder == stepOrder);

        if (nextStep == null) { return null; }

        return new ApproverInfo
        {
            StepOrder = nextStep.StepOrder,
            StepName = nextStep.StepName,
            Role = nextStep.Role,
            SpecificEmployeeId = nextStep.EmployeeId,
            IsFinalStep = nextStep.IsFinal
        };
    }

    public async Task<LeaveAppRes> ProcessApproval(Guid id, Guid approvedById, string action, string? comment = null)
    {
        var result = new LeaveAppRes { LeaveRequestId = id };
        var actRej = BoolToStr.EnumToString(Status.Rejected);
        var actApp = BoolToStr.EnumToString(Status.Approved);
        var actPen = BoolToStr.EnumToString(Status.Pending);

        if (action != actRej && action != actApp)
        {
            result.AddError("Invalid action. Must be 'Approve' or 'Reject'");
            return result;
        }

        var lReq = await _unitOfWork.Repository<LeaveRequest>().GetById(id);
        if (lReq == null)
        {
            result.AddError("Leave request not found");
            return result;
        }

        if (lReq.Status != actPen)
        {
            result.AddError($"Cannot process approval. Current status: {lReq.Status}");
            return result;
        }

        var approvalSteps = await GetApprovalSteps(id);
        var currentStep = approvalSteps.FirstOrDefault(s => s.StepOrder == lReq.CurrentAppStep);

        if (currentStep == null)
        {
            result.AddError("Invalid approval step");
            return result;
        }

        var hasPermission = await VerifyAppPer(approvedById, currentStep);
        if (!hasPermission)
        {
            result.AddError("You do not have permission to approve at this step");
            return result;
        }

        var appAction = new LeaveAppAction
        {
            StepOrder = currentStep.StepOrder,
            Role = currentStep.Role,
            Action = action,
            Comment = comment,
            ActionAt = DateTime.UtcNow,
            LeaveRequestId = id,
            ApprovedById = approvedById,
        };
        await _unitOfWork.Repository<LeaveAppAction>().Add(appAction);

        // Process based on action
        if (action == actRej)
        {
            lReq.Status = actRej;
            await _unitOfWork.Repository<LeaveRequest>().Update(lReq);

            result.Status = "Rejected";
            result.Message = "Leave request has been rejected";
            result.CurrentStep = currentStep.StepOrder;
            result.TotalSteps = approvalSteps.Count;
        }
        else // Approved
        {
            if (currentStep.IsFinal || currentStep.StepOrder == approvalSteps.Count)
            {
                await ApproveLeaveRequest(lReq, appAction);
                result.Status = "Approved";
                result.Message = "Leave request has been fully approved";
                result.CurrentStep = currentStep.StepOrder;
                result.TotalSteps = approvalSteps.Count;
                result.IsFinalApproval = true;
            }
            else
            {
                lReq.CurrentAppStep++;
                await _unitOfWork.Repository<LeaveRequest>().Update(lReq);

                result.Status = "Pending";
                result.CurrentStep = lReq.CurrentAppStep;
                result.TotalSteps = approvalSteps.Count;
                result.NextApprover = await GetNextAppInfo(id, lReq.CurrentAppStep);
                result.Message = $"Approved at step {currentStep.StepOrder}. Moved to step {lReq.CurrentAppStep}";
            }
        }

        return result;
    }

    private async Task ApproveLeaveRequest(LeaveRequest lReq, LeaveAppAction finalAppAction)
    {
        var actApp = BoolToStr.EnumToString(Status.Approved);
        lReq.Status = actApp;
        lReq.DateApproved = DateTime.UtcNow;
        lReq.ApprovedById = finalAppAction.ApprovedById;
        await _unitOfWork.Repository<LeaveRequest>().Update(lReq);

        var dto = new LedgerEntryDto
        {
            EmployeeId = lReq.EmployeeId,
            LeaveTypeId = lReq.LeaveTypeId,
            LeavePolicyId = null,
            Amount = -lReq.DaysRequested,
            EntryType = BoolToStr.EnumToString(LedgerEntryType.Debit),
            SourceType = BoolToStr.EnumToString(LedgerSource.LeaveReq),
            ReferenceId = lReq.Id
        };

        await _leaveLedgerService.Credit(dto);
    }

    private async Task<bool> VerifyAppPer(Guid approvedById, LeaveAppStep step)
    {
        // If specific employee is set, must be that employee
        if (step.EmployeeId.HasValue)
        {
            return step.EmployeeId.Value == approvedById;
        }

        // Otherwise, check if approver has the required role
        // This would integrate with your role/permission system
        // For now, return true (implement your role check here)
        return true;
    }
}