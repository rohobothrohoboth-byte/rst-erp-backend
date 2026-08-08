using Common;
using Helpers;
using Leave.Domain.DTOs;
using Leave.App.Interfaces;
using Microsoft.EntityFrameworkCore;
using Leave.Domain.Entities;
namespace Leave.App.Services;

public interface ILvReqAppService
{
    Task<LeaveAppRes> InitApproval(AppReqDto req, CancellationToken ct);
    Task<LeaveAppRes> ProcessApp(ProcessAppDto dt, LeaveRequest req, CancellationToken ct);
    Task<List<MyPendLvApp>> AppSteps(AppStepQryDto req, CancellationToken ct);
}

public class LvReqAppService(IUnitOfWork _uow, ILeaveLedgerService _lgrSer, IHrmProfileClient _hrmPro) : ILvReqAppService
{
    public async Task<LeaveAppRes> InitApproval(AppReqDto req, CancellationToken ct)
    {
        var result = new LeaveAppRes();
        var empId = req.EmployeeId;
        var leaveTypeId = req.LeaveTypeId;
        var empLeavePolicy = await GetActiveEmpLeavePolicy(empId, leaveTypeId, ct);
        if (empLeavePolicy == null)
        {
            result.AddError("NO ACTIVE LEAVE POLICY FOUND for this employee and leave type.");
            return result;
        }

        var approvalSteps = await GetApprovalSteps(empId, leaveTypeId, ct);
        if (approvalSteps.Count == 0)
        {
            result.CurrentStep = 0;
            result.PerApp = 100;
            result.Status = BoolToStr.EnumToString(Status.Approved);
            result.Message = "Leave request AUTO-APPROVED (NO APPROVAL STEPS configured)";
            result.AutoApp = true;
            return result;
        }

        result.CurrentStep = 1;
        result.PerApp = 0;
        result.Status = BoolToStr.EnumToString(Status.Pending);
        result.AutoApp = false;
        result.Message = $"Leave request submitted for approval (Step 1 of {approvalSteps.Count})";
        return result;
    }

    public async Task<LeaveAppRes> ProcessApp(ProcessAppDto dt, LeaveRequest req, CancellationToken ct)
    {
        var id = dt.Id;
        var action = dt.Action;
        var result = new LeaveAppRes { LeaveRequestId = id };
        var actRej = BoolToStr.EnumToString(Status.Rejected);
        var actApp = BoolToStr.EnumToString(Status.Approved);
        var actPen = BoolToStr.EnumToString(Status.Pending);

        if (action != actRej && action != actApp)
        {
            result.AddError("Invalid action. Must be 'Approve' or 'Reject'");
            return result;
        }

        var lReq = req;
        if (lReq.Status != actPen)
        {
            result.AddError($"Cannot process approval. Current status: {lReq.Status}");
            return result;
        }

        var approvalSteps = await GetApprovalSteps(lReq.EmployeeId, lReq.LeaveTypeId, ct);
        var currentStep = approvalSteps.FirstOrDefault(s => s.StepOrder == lReq.CurrentAppStep);
        if (currentStep == null)
        {
            result.AddError("INVALID approval step.");
            return result;
        }

        var hasPermission = await VerifyAppPer(dt.ApprovedById, currentStep);
        if (!hasPermission)
        {
            result.AddError("You DON'T HAVE PERMISSION to approve at this step.");
            return result;
        }

        var dto = new LvAppActionDto
        {
            StepOrder = currentStep.StepOrder,
            Role = currentStep.Role,
            Action = action,
            Comment = dt.Comment,
            LeaveRequestId = id,
            ApprovedById = dt.ApprovedById,
            LeaveAppStepId = currentStep.Id
        };
        await ProcessAppAction(dto, ct);

        // Process based on action
        if (action == actRej)
        {
            lReq.Status = actRej;
            lReq.DateApp = DateTime.UtcNow;
            lReq.PerApp = 100;
            _uow.Update(lReq);

            result.Message = "Leave request has been REJECTED";
        }
        else // Approved
        {
            if (currentStep.IsFinal || currentStep.StepOrder == approvalSteps.Count)
            {
                await ApproveLeaveRequest(lReq, ct);
                result.Message = "Leave request has been fully APPROVED";
            }
            else
            {
                var per = CalPer(currentStep.StepOrder, approvalSteps.Count);
                lReq.CurrentAppStep++;
                lReq.PerApp = per;
                _uow.Update(lReq);

                result.Message = $"Approved at Step {currentStep.StepOrder}. Moved to Step {lReq.CurrentAppStep}";
            }
        }

        return result;
    }

    public async Task<List<MyPendLvApp>> AppSteps(AppStepQryDto req, CancellationToken ct)
    {
        var appStepsTask = GetApprovalSteps(req.EmpId, req.LeaveTypeId, ct);
        var empListTask = _hrmPro.GetEmpNameList(ct);
        await Task.WhenAll(appStepsTask, empListTask);

        var appSteps = appStepsTask.Result;
        if (appSteps.Count == 0) { return []; }

        var actionsTask = _uow.Set<LeaveAppAction>().AsNoTracking().Where(x => x.LeaveRequestId == req.Id).ToDictionaryAsync(x => x.LeaveAppStepId, ct);
        await actionsTask;

        var appActDict = actionsTask.Result;
        var empDict = empListTask.Result.Res.ToDictionary(x => Guid.Parse(x.Id));
        var workflowPending = appSteps.Count != appActDict.Count;
        var result = new List<MyPendLvApp>(appSteps.Count);

        foreach (var step in appSteps)
        {
           empDict.TryGetValue(step.EmployeeId.GetValueOrDefault(), out var emp);

            appActDict.TryGetValue(step.Id, out var act);
            result.Add(new MyPendLvApp
            {
                Step = $"Step {step.StepOrder}",
                AppBy = emp?.Name ?? "NOT ASSIGNED",
                IsFinal = step.IsFinal,
                IsCurrent = workflowPending && step.StepOrder == req.Cur,
                DateApp = act?.DateApp,
                Decision = act == null ? "Pending" : MyEnumHelper.FormatEnum<Status>(act.Action),
                Comment = act?.Comment ?? string.Empty
            });
        }

        return result;
    }



    // Helper Methods
    private static double CalPer(int currentAppStep, int totalSteps)
    {
        if (totalSteps <= 0) { return 100; }
        return Math.Round((double)currentAppStep * 100d / totalSteps, 2);
    }

    private async Task<EmpLeavePolicy?> GetActiveEmpLeavePolicy(Guid empId, Guid leaveTypeId, CancellationToken ct)
    {
        var ePolicy = await _uow.Set<EmpLeavePolicy>().FirstOrDefaultAsync(elp => elp.EmployeeId == empId && elp.LeaveTypeId == leaveTypeId, ct);
        if (ePolicy == null) { return null; }
        return ePolicy;
    }

    public async Task<List<LeaveAppStep>> GetApprovalSteps(Guid eId, Guid ltId, CancellationToken ct)
    {
        var eLp = await GetActiveEmpLeavePolicy(eId, ltId, ct) ?? throw new NotFoundExc("NO ACTIVE leave policy found.");
        var aStepsL = _uow.Set<LeaveAppStep>().Where(lac => lac.LeavePolicyId == eLp.LeavePolicyId).ToList();
        if (aStepsL.Count == 0) { return []; }
        return aStepsL;
    }

    private async Task ProcessAppAction(LvAppActionDto dt, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var addded = await _uow.Set<LeaveAppAction>().FirstOrDefaultAsync(x => x.LeaveRequestId == dt.LeaveRequestId && x.LeaveAppStepId == dt.LeaveAppStepId, ct);
        if (addded == null)
        {
            var appAction = new LeaveAppAction
            {
                StepOrder = dt.StepOrder,
                Role = dt.Role,
                Action = dt.Action,
                Comment = dt.Comment,
                LeaveRequestId = dt.LeaveRequestId,
                ApprovedById = dt.ApprovedById,
                LeaveAppStepId = dt.LeaveAppStepId,
                DateApp = now
            };
            await _uow.Add(appAction, ct);
        }
        else
        {
            addded.Role = dt.Role;
            addded.Comment = dt.Comment;
            addded.ApprovedById = dt.ApprovedById;
            addded.DateApp = now;
            _uow.Update(addded);
        }
    }

    private async Task ApproveLeaveRequest(LeaveRequest lReq, CancellationToken ct)
    {
        var actApp = BoolToStr.EnumToString(Status.Approved);
        lReq.Status = actApp;
        lReq.DateApp = DateTime.UtcNow;
        lReq.PerApp = 100;
        _uow.Update(lReq);

        var bal = lReq.DaysRequested;
        var lBal = await _uow.Set<LeaveBalance>().FirstOrDefaultAsync(b => b.EmployeeId == lReq.EmployeeId && b.LeaveTypeId == lReq.LeaveTypeId, ct);
        if (lBal == null)
        {
            lBal = new LeaveBalance
            {
                EmployeeId = lReq.EmployeeId,
                LeaveTypeId = lReq.LeaveTypeId,
                Balance = 0
            };
            await _uow.Add(lBal, ct);
        }

        lBal.Balance -= lReq.DaysRequested;
        lBal.AsOf = DateTime.UtcNow;

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

        await _lgrSer.Credit(dto, ct);
    }

    private static async Task<bool> VerifyAppPer(Guid approvedById, LeaveAppStep step)
    {
        if (step.EmployeeId == approvedById) { return true; }
        return false;
    }
}