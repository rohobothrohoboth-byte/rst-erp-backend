using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Services;

public interface IApprovalEngine
{
    Task<LeaveAppRes> InitApproval(Guid id, CancellationToken ct);
}



public class ApprovalEngine : IApprovalEngine
{
    private readonly IUnitOfWork _uow;
    private readonly ILeaveLedgerService _leaveLedgerService;

    public ApprovalEngine(IUnitOfWork uow, ILeaveLedgerService leaveLedgerService)
    {
        _uow = uow;
        _leaveLedgerService = leaveLedgerService;
    }

    public async Task<LeaveAppRes> InitApproval(Guid id, CancellationToken ct)
    {
        var result = new LeaveAppRes();
        var req = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (req == null)
        {
            result.AddError("Leave request NOT FOUND.");
            return result;
        }

        var empId = req.EmployeeId;
        var leaveTypeId = req.LeaveTypeId;
        var requestDate = req.StartDate;
        var empLeavePolicy = GetActiveEmpLeavePolicy(empId, leaveTypeId, requestDate);
        if (empLeavePolicy == null)
        {
            result.AddError("NO ACTIVE LEAVE POLICY FOUND for this employee and leave type.");
            return result;
        }

        var approvalSteps = await GetApprovalSteps(id, ct);

        if (!approvalSteps.Any())
        {
            // No approval required - auto-approve
            await AutoAppLeaveReq(req, ct);
            result.Status = "Approved";
            result.Message = "Leave request auto-approved (no approval chain configured)";
            result.RequiresApproval = false;
            return result;
        }

        // Set to pending and start at first step
        req.Status = BoolToStr.EnumToString(Status.Pending);
        req.CurrentAppStep = 1;
        await _uow.Update(req);

        result.Status = "Pending";
        result.CurrentStep = 1;
        result.TotalSteps = approvalSteps.Count;
        result.NextApprover = await GetNextAppInfo(req.Id, 1, ct);
        result.RequiresApproval = true;
        result.Message = $"Leave request submitted for approval (Step 1 of {approvalSteps.Count})";

        return result;
    }


    private EmpLeavePolicy? GetActiveEmpLeavePolicy(Guid empId, Guid leaveTypeId, DateTime requestDate)
    {
        var ePolicyL = _uow.Set<EmpLeavePolicy>().Where(elp => elp.EmployeeId == empId && elp.LeaveTypeId == leaveTypeId && elp.EffectiveFrom <= requestDate).ToList();
        if (ePolicyL.Count <= 0) { return null; }
        var ePolicy = ePolicyL.FirstOrDefault(e => e.IsActive(requestDate));
        return ePolicy;
    }

    public LeaveAppChain? GetActiveApprovalChain(Guid leavePolicyId, DateTime effectiveDate)
    {
        var aChainL = _uow.Set<LeaveAppChain>().Where(lac => lac.LeavePolicyId == leavePolicyId && lac.IsActive && lac.EffectiveFrom <= effectiveDate).ToList();
        var aChain = aChainL.FirstOrDefault(lac => lac.EffectiveTo == null || lac.EffectiveTo >= effectiveDate);
        return aChain;
    }

    public async Task<List<LeaveAppStep>> GetApprovalSteps(Guid id, CancellationToken ct)
    {
        var req = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new DomainException("Leave request NOT FOUND.");
        var eLp = GetActiveEmpLeavePolicy(req.EmployeeId, req.LeaveTypeId, req.StartDate) ?? throw new DomainException("NO ACTIVE leave policy found.");
        var aChain = GetActiveApprovalChain(eLp.LeavePolicyId, req.StartDate);
        return aChain?.Steps.OrderBy(s => s.StepOrder).ToList() ?? [];
    }

    private async Task AutoAppLeaveReq(LeaveRequest leaveRequest, CancellationToken ct)
    {
        leaveRequest.Status = BoolToStr.EnumToString(Status.Approved);
        leaveRequest.DateApproved = DateTime.UtcNow;
        await _uow.Update(leaveRequest);

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

        await _leaveLedgerService.Credit(dto, ct);
    }

    private async Task<ApproverInfo?> GetNextAppInfo(Guid leaveRequestId, int stepOrder, CancellationToken ct)
    {
        var approvalSteps = await GetApprovalSteps(leaveRequestId, ct);
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

    public async Task<LeaveAppRes> ProcessApproval(Guid id, Guid approvedById, string action, CancellationToken ct, string? comment = null)
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

        var lReq = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == id, ct);
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

        var approvalSteps = await GetApprovalSteps(id, ct);
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
        await _uow.Add(appAction, ct);

        // Process based on action
        if (action == actRej)
        {
            lReq.Status = actRej;
            await _uow.Update(lReq);

            result.Status = "Rejected";
            result.Message = "Leave request has been rejected";
            result.CurrentStep = currentStep.StepOrder;
            result.TotalSteps = approvalSteps.Count;
        }
        else // Approved
        {
            if (currentStep.IsFinal || currentStep.StepOrder == approvalSteps.Count)
            {
                await ApproveLeaveRequest(lReq, appAction, ct);
                result.Status = "Approved";
                result.Message = "Leave request has been fully approved";
                result.CurrentStep = currentStep.StepOrder;
                result.TotalSteps = approvalSteps.Count;
                result.IsFinalApproval = true;
            }
            else
            {
                lReq.CurrentAppStep++;
                await _uow.Update(lReq);

                result.Status = "Pending";
                result.CurrentStep = lReq.CurrentAppStep;
                result.TotalSteps = approvalSteps.Count;
                result.NextApprover = await GetNextAppInfo(id, lReq.CurrentAppStep, ct);
                result.Message = $"Approved at step {currentStep.StepOrder}. Moved to step {lReq.CurrentAppStep}";
            }
        }

        return result;
    }

    private async Task ApproveLeaveRequest(LeaveRequest lReq, LeaveAppAction finalAppAction, CancellationToken ct)
    {
        var actApp = BoolToStr.EnumToString(Status.Approved);
        lReq.Status = actApp;
        lReq.DateApproved = DateTime.UtcNow;
        lReq.ApprovedById = finalAppAction.ApprovedById;
        await _uow.Update(lReq);

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

        await _leaveLedgerService.Credit(dto, ct);
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