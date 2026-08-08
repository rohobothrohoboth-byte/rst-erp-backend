using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Services;

public interface IApprovalEngine
{
   Task<LeaveAppRes> InitApproval(Guid id, CancellationToken ct);
       Task ProcessApproval(Guid leaveRequestId, bool isApproved, string comments, CancellationToken ct);
       Task<LeaveAppRes> ProcessApproval(Guid id, Guid approvedById, string action, CancellationToken ct, string? comment = null);
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

    // ==================== INIT APPROVAL ====================

    public async Task<LeaveAppRes> InitApproval(Guid id, CancellationToken ct)
    {
        var result = new LeaveAppRes();
        var req = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (req == null)
        {
            result.AddError("Leave request NOT FOUND.");
            return result;
        }

        // If approval chain is already set, use it
        if (req.ApprovalChainId.HasValue)
        {
            var steps = await _uow.Set<LeaveAppStep>()
                .Where(s => s.LeaveAppChainId == req.ApprovalChainId.Value && !s.IsDeleted)
                .OrderBy(s => s.StepOrder)
                .ToListAsync(ct);

            if (steps.Any())
            {
                result.Status = "Pending";
                result.CurrentStep = req.CurrentAppStep > 0 ? req.CurrentAppStep : 1;
                result.TotalSteps = steps.Count;
                result.RequiresApproval = true;
                result.Message = $"Leave request is in approval workflow (Step {result.CurrentStep} of {steps.Count})";
                return result;
            }
        }

        // Get the approval chain from the policy
        var empLeavePolicy = await _uow.Set<EmpLeavePolicy>()
            .FirstOrDefaultAsync(x => x.EmployeeId == req.EmployeeId
                && x.LeaveTypeId == req.LeaveTypeId
                && x.EffectiveFrom <= req.StartDate
                && (x.EffectiveTo == null || x.EffectiveTo >= req.StartDate)
                && !x.IsDeleted, ct);

        if (empLeavePolicy == null || !empLeavePolicy.LeavePolicyId.HasValue)
        {
            await AutoAppLeaveReq(req, ct);
            result.Status = "Approved";
            result.Message = "Leave request auto-approved (no leave policy found)";
            result.RequiresApproval = false;
            return result;
        }

        // Find the active approval chain
        var chain = await _uow.Set<LeaveAppChain>()
            .FirstOrDefaultAsync(lac => lac.LeavePolicyId == empLeavePolicy.LeavePolicyId.Value
                && lac.IsActive
                && lac.EffectiveFrom <= req.StartDate
                && (lac.EffectiveTo == null || lac.EffectiveTo >= req.StartDate)
                && !lac.IsDeleted, ct);

        if (chain == null)
        {
            await AutoAppLeaveReq(req, ct);
            result.Status = "Approved";
            result.Message = "Leave request auto-approved (no approval chain configured)";
            result.RequiresApproval = false;
            return result;
        }

        // Get all steps for this chain
        var approvalSteps = await _uow.Set<LeaveAppStep>()
            .Where(s => s.LeaveAppChainId == chain.Id && !s.IsDeleted)
            .OrderBy(s => s.StepOrder)
            .ToListAsync(ct);

        if (!approvalSteps.Any())
        {
            await AutoAppLeaveReq(req, ct);
            result.Status = "Approved";
            result.Message = "Leave request auto-approved (no approval steps configured)";
            result.RequiresApproval = false;
            return result;
        }

        // Update the leave request with chain info
        req.ApprovalChainId = chain.Id;
        req.CurrentAppStep = 1;
        await _uow.SaveChangesAsync(ct);

        result.Status = "Pending";
        result.CurrentStep = 1;
        result.TotalSteps = approvalSteps.Count;
        result.RequiresApproval = true;
        result.Message = $"Leave request submitted for approval (Step 1 of {approvalSteps.Count})";

        return result;
    }

    // ==================== SIMPLE PROCESS APPROVAL (for backward compatibility) ====================
public async Task ProcessApproval(Guid leaveRequestId, bool isApproved, string comments, CancellationToken ct)
{
    var leaveRequest = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == leaveRequestId, ct);
    if (leaveRequest == null)
        throw new DomainException($"Leave request with id {leaveRequestId} not found");

    // Prevent duplicate processing
    if (leaveRequest.Status != "0")
    {
        Console.WriteLine($"Request already processed. Current status: {leaveRequest.Status}");
        return;
    }

    if (isApproved)
    {
        leaveRequest.Status = "1";
        leaveRequest.DateApproved = DateTime.UtcNow;
        await _uow.Update(leaveRequest);
        await DeductLeaveBalance(leaveRequest, ct, saveChanges: false);
    }
    else
    {
        leaveRequest.Status = "2";
        await _uow.Update(leaveRequest);
    }

    await _uow.SaveChangesAsync(ct);
    Console.WriteLine($"? Status updated to: {leaveRequest.Status}");
}
 public async Task<LeaveAppRes> ProcessApproval(Guid id, Guid approvedById, string action, CancellationToken ct, string? comment = null)
 {
     var result = new LeaveAppRes { LeaveRequestId = id };
     var actRej = "2";
     var actApp = "1";
     var actPen = "0";

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

     // If already approved or rejected, return
     if (lReq.Status == actApp || lReq.Status == actRej)
     {
         result.Status = lReq.Status == actApp ? "Approved" : "Rejected";
         result.Message = $"Request is already {GetStatusText(lReq.Status)}";
         return result;
     }

     // Get approval steps for this request
     var approvalSteps = await GetApprovalSteps(id, ct);

     // If no steps configured, simple approval
     if (approvalSteps == null || !approvalSteps.Any())
     {
         if (action == actApp)
         {
             lReq.Status = actApp;
             lReq.DateApproved = DateTime.UtcNow;
             lReq.ApprovedById = approvedById;
             await DeductLeaveBalance(lReq, ct);
             await _uow.Update(lReq);
             await _uow.SaveChangesAsync(ct);

             result.Status = "Approved";
             result.Message = "Leave request approved successfully";
             result.IsFinalApproval = true;
         }
         else
         {
             lReq.Status = actRej;
             await _uow.Update(lReq);
             await _uow.SaveChangesAsync(ct);
             result.Status = "Rejected";
             result.Message = "Leave request rejected";
         }
         return result;
     }

     // Get current step
     int currentStepOrder = lReq.CurrentAppStep;
     if (currentStepOrder == 0)
     {
         currentStepOrder = 1;
         lReq.CurrentAppStep = 1;
         await _uow.Update(lReq);
         await _uow.SaveChangesAsync(ct);
     }

     var currentStep = approvalSteps.FirstOrDefault(s => s.StepOrder == currentStepOrder);
     if (currentStep == null)
     {
         result.AddError($"Invalid approval step {currentStepOrder}");
         return result;
     }

     Console.WriteLine($"Processing step {currentStep.StepOrder}: {currentStep.StepName}");

     // Check if action already exists for this step
     var existingAction = await _uow.Set<LeaveAppAction>()
         .FirstOrDefaultAsync(a => a.LeaveRequestId == id && a.StepOrder == currentStep.StepOrder, ct);

     if (existingAction == null)
     {
         var hasPermission = await VerifyApproverPermission(approvedById, currentStep);
         if (!hasPermission)
         {
             result.AddError("You do not have permission to approve at this step");
             return result;
         }

         var appAction = new LeaveAppAction
         {
             Id = Guid.NewGuid(),
             LeaveRequestId = id,
             StepOrder = currentStep.StepOrder,
             Role = currentStep.Role,
             Action = action,
             Comment = comment,
             ActionAt = DateTime.UtcNow,
             ApprovedById = approvedById,
             DateAdd = DateTime.UtcNow,
             IsDeleted = false
         };
         await _uow.Add(appAction, ct);
         await _uow.SaveChangesAsync(ct);
         Console.WriteLine($"Created action for step {currentStep.StepOrder}");
     }
     else
     {
         Console.WriteLine($"Action already exists for step {currentStep.StepOrder}");
     }

     if (action == actRej)
     {
         lReq.Status = actRej;
         result.Status = "Rejected";
         result.Message = "Leave request has been rejected";
         result.CurrentStep = currentStep.StepOrder;
         result.TotalSteps = approvalSteps.Count;
         await _uow.Update(lReq);
         await _uow.SaveChangesAsync(ct);
         return result;
     }

     bool isFinalStep = currentStep.IsFinal || currentStep.StepOrder == approvalSteps.Count;

     if (isFinalStep)
     {
         Console.WriteLine($"Final step {currentStep.StepOrder} - approving and deducting balance");
         lReq.Status = actApp;
         lReq.DateApproved = DateTime.UtcNow;
         lReq.ApprovedById = approvedById;

         await DeductLeaveBalance(lReq, ct);
         await _uow.Update(lReq);
         await _uow.SaveChangesAsync(ct);

         result.Status = "Approved";
         result.Message = "Leave request has been fully approved";
         result.CurrentStep = currentStep.StepOrder;
         result.TotalSteps = approvalSteps.Count;
         result.IsFinalApproval = true;
     }
     else
     {
         int nextStepOrder = currentStep.StepOrder + 1;
         Console.WriteLine($"Moving from step {currentStep.StepOrder} to step {nextStepOrder}");

         lReq.CurrentAppStep = nextStepOrder;
         lReq.Status = actPen;
         await _uow.Update(lReq);
         await _uow.SaveChangesAsync(ct);

         result.Status = "Pending";
         result.CurrentStep = nextStepOrder;
         result.TotalSteps = approvalSteps.Count;

         var nextStep = approvalSteps.FirstOrDefault(s => s.StepOrder == nextStepOrder);
         result.NextApprover = nextStep != null ? new ApproverInfo
         {
             StepOrder = nextStep.StepOrder,
             StepName = nextStep.StepName,
             Role = nextStep.Role
         } : null;
         result.Message = $"Approved at step {currentStep.StepOrder}. Moved to step {nextStepOrder}";
     }

     Console.WriteLine($"? ProcessApproval completed. Status: {result.Status}, Step: {result.CurrentStep}");

     return result;
 }


    // ==================== PRIVATE HELPER METHODS ====================

    private async Task<decimal> GetMaxCarryOverDays(Guid leaveTypeId, CancellationToken ct)
    {
        var policy = await _uow.Set<LeavePolicy>()
            .FirstOrDefaultAsync(x => x.LeaveTypeId == leaveTypeId && x.Status == "Active" && !x.IsDeleted, ct);

        if (policy == null)
            return 0;

        var policyConfig = await _uow.Set<LeavePolicyConfig>()
            .FirstOrDefaultAsync(x => x.LeavePolicyId == policy.Id && x.IsActive && !x.IsDeleted, ct);

        if (policyConfig == null)
            return 0;

        return (decimal)policyConfig.MaxCarryOverDays;
    }

    public async Task ProcessYearEndCarryOver(Guid employeeId, int year, CancellationToken ct)
    {
        var policies = await _uow.Set<EmpLeavePolicy>()
            .Where(x => x.EmployeeId == employeeId
                && x.EffectiveFrom.Year == year
                && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var policy in policies)
        {
            var maxCarryOver = await GetMaxCarryOverDays(policy.LeaveTypeId, ct);
            var remaining = policy.AssignedEntitlement - policy.UsedEntitlement;
            var carryForward = remaining > maxCarryOver ? maxCarryOver : remaining;

            if (carryForward <= 0) continue;

            var newPolicy = new EmpLeavePolicy
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                LeaveTypeId = policy.LeaveTypeId,
                LeavePolicyId = policy.LeavePolicyId,
                AssignedEntitlement = carryForward,
                UsedEntitlement = 0,
                CarryForward = carryForward,
                EffectiveFrom = new DateTime(year + 1, 1, 1),
                EffectiveTo = null,
                IsActive = true,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(newPolicy, ct);
            policy.EffectiveTo = new DateTime(year, 12, 31);
            await _uow.Update(policy);
        }

        await _uow.SaveChangesAsync(ct);
    }

    private async Task<bool> VerifyApproverPermission(Guid approvedById, LeaveAppStep step)
    {
        if (step.EmployeeId.HasValue)
        {
            return step.EmployeeId.Value == approvedById;
        }
        return true;
    }

    private string GetRoleName(string roleCode)
    {
        return roleCode switch
        {
            "0" => "Manager",
            "1" => "HR",
            "2" => "HOD",
            "3" => "CEO",
            "4" => "Team Lead",
            "5" => "Department Head",
            _ => roleCode
        };
    }

    private string GetStatusText(string status)
    {
        return status switch
        {
            "0" => "Pending",
            "1" => "Approved",
            "2" => "Rejected",
            "3" => "Cancelled",
            _ => status
        };
    }

    private async Task DeductLeaveBalance(LeaveRequest leaveRequest, CancellationToken ct, bool saveChanges = false)
    {
        try
        {
            Console.WriteLine($"=== DeductLeaveBalance called ===");
            Console.WriteLine($"EmployeeId: {leaveRequest.EmployeeId}");
            Console.WriteLine($"LeaveTypeId: {leaveRequest.LeaveTypeId}");
            Console.WriteLine($"DaysRequested: {leaveRequest.DaysRequested}");

            var empPolicy = await _uow.Set<EmpLeavePolicy>()
                .FirstOrDefaultAsync(x => x.EmployeeId == leaveRequest.EmployeeId
                    && x.LeaveTypeId == leaveRequest.LeaveTypeId
                    && (x.EffectiveTo == null || x.EffectiveTo >= DateTime.UtcNow)
                    && x.IsDeleted == false, ct);

            if (empPolicy == null)
            {
                Console.WriteLine($"?? No active policy found for employee {leaveRequest.EmployeeId}");
                return;
            }

            var oldBalance = empPolicy.AssignedEntitlement;
            var oldUsed = empPolicy.UsedEntitlement;

            empPolicy.AssignedEntitlement -= (decimal)leaveRequest.DaysRequested;
            empPolicy.UsedEntitlement = oldUsed + (decimal)leaveRequest.DaysRequested;
            empPolicy.DateMod = DateTime.UtcNow;

            Console.WriteLine($"? Before update - Assigned: {oldBalance}, Used: {oldUsed}");
            Console.WriteLine($"? After update - Assigned: {empPolicy.AssignedEntitlement}, Used: {empPolicy.UsedEntitlement}");

            await _uow.Update(empPolicy);

            var currentUtcTime = DateTime.UtcNow;
            var ledgerDto = new LedgerEntryDto
            {
                EmployeeId = leaveRequest.EmployeeId,
                LeaveTypeId = leaveRequest.LeaveTypeId,
                LeavePolicyId = empPolicy.LeavePolicyId,
                Amount = -leaveRequest.DaysRequested,
                EntryType = BoolToStr.EnumToString(LedgerEntryType.Debit),
                SourceType = BoolToStr.EnumToString(LedgerSource.LeaveReq),
                ReferenceId = leaveRequest.Id,
                Date = currentUtcTime,
                DateAdd = currentUtcTime,
                DateMod = null
            };

            await _leaveLedgerService.Credit(ledgerDto, ct);

            if (saveChanges)
            {
                await _uow.SaveChangesAsync(ct);
                Console.WriteLine($"? Policy changes saved to database");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error in DeductLeaveBalance: {ex.Message}");
            throw;
        }
    }

    private EmpLeavePolicy? GetActiveEmpLeavePolicy(Guid empId, Guid leaveTypeId, DateTime requestDate)
    {
        var utcRequestDate = DateTime.SpecifyKind(requestDate, DateTimeKind.Utc);

        var ePolicyL = _uow.Set<EmpLeavePolicy>()
            .Where(elp => elp.EmployeeId == empId
                && elp.LeaveTypeId == leaveTypeId
                && elp.EffectiveFrom <= utcRequestDate
                && elp.IsDeleted == false)
            .ToList();

        if (ePolicyL.Count <= 0)
            return null;

        var ePolicy = ePolicyL.FirstOrDefault(e =>
            e.EffectiveTo == null || e.EffectiveTo >= utcRequestDate);

        return ePolicy;
    }

    private LeaveAppChain? GetActiveApprovalChain(Guid leavePolicyId, DateTime effectiveDate)
    {
        var aChainL = _uow.Set<LeaveAppChain>()
            .Where(lac => lac.LeavePolicyId == leavePolicyId && lac.IsActive && lac.EffectiveFrom <= effectiveDate)
            .ToList();

        var aChain = aChainL.FirstOrDefault(lac => lac.EffectiveTo == null || lac.EffectiveTo >= effectiveDate);
        return aChain;
    }

   public async Task<List<LeaveAppStep>> GetApprovalSteps(Guid id, CancellationToken ct)
   {
       var req = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == id, ct)
           ?? throw new DomainException("Leave request NOT FOUND.");

       // If the request has an ApprovalChainId, use it directly
       if (req.ApprovalChainId.HasValue)
       {
           return await _uow.Set<LeaveAppStep>()
               .Where(s => s.LeaveAppChainId == req.ApprovalChainId.Value && !s.IsDeleted)
               .OrderBy(s => s.StepOrder)
               .ToListAsync(ct);
       }

       // Fallback to policy-based lookup
       var eLp = GetActiveEmpLeavePolicy(req.EmployeeId, req.LeaveTypeId, req.StartDate)
           ?? throw new DomainException("NO ACTIVE leave policy found.");

       var aChain = eLp.LeavePolicyId.HasValue
           ? GetActiveApprovalChain(eLp.LeavePolicyId.Value, req.StartDate)
           : throw new DomainException("Leave policy ID is required for approval chain");

       if (aChain == null)
       {
           return new List<LeaveAppStep>();
       }

       return await _uow.Set<LeaveAppStep>()
           .Where(s => s.LeaveAppChainId == aChain.Id && !s.IsDeleted)
           .OrderBy(s => s.StepOrder)
           .ToListAsync(ct);
   }
    private async Task AutoAppLeaveReq(LeaveRequest leaveRequest, CancellationToken ct)
    {
        leaveRequest.Status = BoolToStr.EnumToString(Status.Approved);
        leaveRequest.DateApproved = DateTime.UtcNow;

        await DeductLeaveBalance(leaveRequest, ct);
        await _uow.SaveChangesAsync(ct);

        var currentUtcTime = DateTime.UtcNow;
        var dto = new LedgerEntryDto
        {
            EmployeeId = leaveRequest.EmployeeId,
            LeaveTypeId = leaveRequest.LeaveTypeId,
            LeavePolicyId = null,
            Amount = -leaveRequest.DaysRequested,
            EntryType = BoolToStr.EnumToString(LedgerEntryType.Debit),
            SourceType = BoolToStr.EnumToString(LedgerSource.LeaveReq),
            ReferenceId = leaveRequest.Id,
            Date = currentUtcTime,
            DateAdd = currentUtcTime,
            DateMod = null
        };

        await _leaveLedgerService.Credit(dto, ct);
    }
}