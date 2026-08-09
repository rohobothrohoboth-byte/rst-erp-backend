using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Data;

namespace Leave.App.Commands;

// ==================== LEAVE REQUEST COMMANDS ====================

public class LeaveRequestAddCmd : IRequest<LeaveRequestListDto>
{
    public LeaveRequestAddDto AddDto { get; set; } = default!;
    public Guid EmpId { get; set; }
}

public class LeaveRequestModCmd : IRequest<LeaveRequestListDto>
{
    public LeaveRequestModDto ModDto { get; set; } = default!;
}

public class LeaveRequestDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class LeaveRequestCancelCmd : IRequest<LeaveRequestListDto>
{
    public Guid Id { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}

public class LeaveRequestApproveCmd : IRequest<LeaveRequestListDto>
{
    public Guid Id { get; set; }
    public string Comments { get; set; } = string.Empty;
    public string RowVersion { get; set; } = string.Empty;
    public Guid ApprovedById { get; set; }
}

public class LeaveRequestRejectCmd : IRequest<LeaveRequestListDto>
{
    public Guid Id { get; set; }
    public string Comments { get; set; } = string.Empty;
    public string RowVersion { get; set; } = string.Empty;
}

// ==================== LEAVE TYPE COMMANDS ====================

public class LeaveTypeAddCmd : IRequest<LeaveTypeListDto>
{
    public LeaveTypeAddDto AddDto { get; set; } = default!;
}

public class LeaveTypeModCmd : IRequest<LeaveTypeListDto>
{
    public LeaveTypeModDto ModDto { get; set; } = default!;
}

public class LeaveTypeStatCmd : IRequest<LeaveTypeListDto>
{
    public StatChangeDto StatDto { get; set; } = default!;
}

public class LeaveTypeDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ==================== POLICY ASSIGNMENT COMMANDS ====================

public class PolicyAssignCmd : IRequest<string> { }

// ==================== LEAVE REQUEST HANDLERS ====================

// ✅ FULLY CORRECTED LeaveRequestAddHandler
public class LeaveRequestAddHandler : IRequestHandler<LeaveRequestAddCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IHolidayService _hdService;
    private readonly ILeaveValService _lvService;
    private readonly IApprovalEngine _approvalEngine;

    public LeaveRequestAddHandler(IUnitOfWork uow, IMediator med, IHolidayService hdService, ILeaveValService lvService, IApprovalEngine approvalEngine)
    {
        _uow = uow;
        _med = med;
        _hdService = hdService;
        _lvService = lvService;
        _approvalEngine = approvalEngine;
    }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestAddCmd request, CancellationToken ct)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("LeaveRequestAddHandler STARTED");
        Console.WriteLine($"Employee ID: {request.EmpId}");
        Console.WriteLine($"Leave Type ID: {request.AddDto.LeaveTypeId}");
        Console.WriteLine($"Start Date: {request.AddDto.StartDate}");
        Console.WriteLine($"End Date: {request.AddDto.EndDate}");
        Console.WriteLine("========================================");

        return await _uow.ExecuteAsync(async token =>
        {
            try
            {
                var startDateUtc = DateTime.SpecifyKind(request.AddDto.StartDate.Date, DateTimeKind.Utc);
                var endDateUtc = DateTime.SpecifyKind(request.AddDto.EndDate.Date, DateTimeKind.Utc);

                var workingDays = await _hdService.CalEmpLeaveWorkingDays(request.EmpId, startDateUtc, endDateUtc, request.AddDto.IsHalfDay);

                var valReq = await _lvService.ValLeaveRequest(request.EmpId, request.AddDto.LeaveTypeId, startDateUtc, endDateUtc, request.AddDto.IsHalfDay, token);

                if (!valReq.IsValid)
                {
                    throw new DomainException($"Leave request VALIDATION FAILED: {string.Join(", ", valReq.Errors)}");
                }

                Guid? approvalChainId = null;
                int? currentStepOrder = null;

                var empLeavePolicy = await _uow.Set<EmpLeavePolicy>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.EmployeeId == request.EmpId
                        && x.LeaveTypeId == request.AddDto.LeaveTypeId
                        && x.EffectiveFrom.Date <= startDateUtc.Date
                        && (x.EffectiveTo == null || x.EffectiveTo!.Value.Date >= startDateUtc.Date)
                        && !x.IsDeleted, token);

                if (empLeavePolicy?.LeavePolicyId != null)
                {
                    var approvalChain = await _uow.Set<LeaveAppChain>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(lac => lac.LeavePolicyId == empLeavePolicy.LeavePolicyId.Value
                            && lac.IsActive
                            && lac.EffectiveFrom.Date <= startDateUtc.Date
                            && (lac.EffectiveTo == null || lac.EffectiveTo!.Value.Date >= startDateUtc.Date)
                            && !lac.IsDeleted, token);

                    if (approvalChain != null)
                    {
                        approvalChainId = approvalChain.Id;
                        currentStepOrder = 1;
                    }
                }

                var data = new LeaveRequest
                {
                    EmployeeId = request.EmpId,
                    LeaveTypeId = request.AddDto.LeaveTypeId,
                    StartDate = startDateUtc,
                    EndDate = endDateUtc,
                    DaysRequested = workingDays,
                    IsHalfDay = request.AddDto.IsHalfDay,
                    Status = "0",
                    Comments = request.AddDto.Comments ?? string.Empty,
                    CurrentAppStep = currentStepOrder ?? 0,
                    ApprovalChainId = approvalChainId,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _uow.Add(data, token);
                await _uow.SaveChangesAsync(token);

                await _approvalEngine.InitApproval(data.Id, token);

                var leaveType = await _uow.Set<LeaveType>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == data.LeaveTypeId, token);

                return new LeaveRequestListDto
                {
                    Id = data.Id,
                    EmployeeId = data.EmployeeId,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    DaysRequested = data.DaysRequested,
                    IsHalfDay = data.IsHalfDay,
                    Status = data.Status,
                    LeaveType = leaveType?.Name ?? "",
                    DaysRequestedStr = $"{data.DaysRequested:F2} days",
                    IsHalfDayStr = data.IsHalfDay ? "Yes" : "No",
                    StatusStr = "Pending",
                    ApprovalChainId = data.ApprovalChainId,
                    CurrentStepOrder = data.CurrentAppStep
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EXCEPTION: {ex.Message}");
                throw;
            }
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveRequestModHandler
public class LeaveRequestModHandler : IRequestHandler<LeaveRequestModCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IHolidayService _hdService;
    private readonly ILeaveValService _lvService;

    public LeaveRequestModHandler(IUnitOfWork uow, IMediator med, IHolidayService hdService, ILeaveValService lvService)
    {
        _uow = uow;
        _med = med;
        _hdService = hdService;
        _lvService = lvService;
    }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestModCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            var oldData = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);
            if (oldData == null) throw new DomainException($"LEAVE REQUEST with Id {request.ModDto.Id} NOT FOUND.");

            var startDateUtc = DateTime.SpecifyKind(request.ModDto.StartDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(request.ModDto.EndDate, DateTimeKind.Utc);

            var workingDays = await _hdService.CalEmpLeaveWorkingDays(oldData.EmployeeId, startDateUtc, endDateUtc, request.ModDto.IsHalfDay);
            var valReq = await _lvService.ValLeaveRequest(oldData.EmployeeId, request.ModDto.LeaveTypeId, startDateUtc, endDateUtc, request.ModDto.IsHalfDay, token);

            if (!valReq.IsValid)
                throw new DomainException($"Leave request VALIDATION FAILED: {string.Join(", ", valReq.Errors)}");

            oldData.LeaveTypeId = request.ModDto.LeaveTypeId;
            oldData.StartDate = startDateUtc;
            oldData.EndDate = endDateUtc;
            oldData.DaysRequested = workingDays;
            oldData.IsHalfDay = request.ModDto.IsHalfDay;
            oldData.Comments = request.ModDto.Comments;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.SaveChangesAsync(token);

            return await _med.Send(new LeaveRequestByIdQry { Id = request.ModDto.Id }, token) ?? new LeaveRequestListDto();
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveRequestDelHandler
public class LeaveRequestDelHandler : IRequestHandler<LeaveRequestDelCmd>
{
    private readonly IUnitOfWork _uow;

    public LeaveRequestDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(LeaveRequestDelCmd request, CancellationToken ct)
    {
        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == request.Id, token);
            if (data == null) throw new DomainException($"LEAVE REQUEST with id [{request.Id}] NOT FOUND.");
            await _uow.Delete(data);
            await _uow.SaveChangesAsync(token);
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveRequestCancelHandler
public class LeaveRequestCancelHandler : IRequestHandler<LeaveRequestCancelCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveRequestCancelHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestCancelCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == request.Id, token);
            if (data == null) throw new DomainException($"LEAVE REQUEST with Id {request.Id} NOT FOUND.");
            if (data.Status != "0")
                throw new DomainException("Only pending requests can be cancelled.");

            data.Status = "3";
            data.SetRowVersion(uint.Parse(request.RowVersion));
            await _uow.Update(data);
            await _uow.SaveChangesAsync(token);

            return await _med.Send(new LeaveRequestByIdQry { Id = request.Id }, token) ?? new LeaveRequestListDto();
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveRequestApproveHandler
public class LeaveRequestApproveHandler : IRequestHandler<LeaveRequestApproveCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IApprovalEngine _approvalEngine;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LeaveRequestApproveHandler(IUnitOfWork uow, IMediator med, IApprovalEngine approvalEngine, IHttpContextAccessor httpContextAccessor)
    {
        _uow = uow;
        _med = med;
        _approvalEngine = approvalEngine;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestApproveCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            var approvedByIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("employeeId");
            if (string.IsNullOrEmpty(approvedByIdClaim))
                throw new UnauthorizedException("User not authenticated");

            var approvedById = Guid.Parse(approvedByIdClaim);

            var data = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == request.Id, token);
            if (data == null)
                throw new DomainException($"LEAVE REQUEST with Id {request.Id} NOT FOUND.");

            if (!string.IsNullOrEmpty(request.Comments))
            {
                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var existingComments = data.Comments ?? "";
                var approveComment = $"[{timestamp}] APPROVED: {request.Comments}";
                data.Comments = string.IsNullOrEmpty(existingComments) ? approveComment : existingComments + "\n" + approveComment;
            }

            var approvalResult = await _approvalEngine.ProcessApproval(data.Id, approvedById, "1", token, request.Comments);

            await _uow.SaveChangesAsync(token);

            var result = await _med.Send(new LeaveRequestByIdQry { Id = request.Id }, token);

            if (result != null)
            {
                result.EmployeeId = data.EmployeeId;
            }

            return result ?? new LeaveRequestListDto();
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveRequestRejectHandler
public class LeaveRequestRejectHandler : IRequestHandler<LeaveRequestRejectCmd, LeaveRequestListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IApprovalEngine _approvalEngine;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LeaveRequestRejectHandler(IUnitOfWork uow, IMediator med, IApprovalEngine approvalEngine, IHttpContextAccessor httpContextAccessor)
    {
        _uow = uow;
        _med = med;
        _approvalEngine = approvalEngine;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LeaveRequestListDto> Handle(LeaveRequestRejectCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            var approvedByIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("employeeId");
            if (string.IsNullOrEmpty(approvedByIdClaim))
                throw new UnauthorizedException("User not authenticated");

            var approvedById = Guid.Parse(approvedByIdClaim);

            var data = await _uow.Set<LeaveRequest>().FirstOrDefaultAsync(x => x.Id == request.Id, token);
            if (data == null)
                throw new DomainException($"LEAVE REQUEST with Id {request.Id} NOT FOUND.");

            if (!string.IsNullOrEmpty(request.Comments))
            {
                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var existingComments = data.Comments ?? "";
                var rejectComment = $"[{timestamp}] REJECTED: {request.Comments}";
                data.Comments = string.IsNullOrEmpty(existingComments) ? rejectComment : existingComments + "\n" + rejectComment;
            }

            var approvalResult = await _approvalEngine.ProcessApproval(data.Id, approvedById, "2", token, request.Comments);

            await _uow.SaveChangesAsync(token);

            var result = await _med.Send(new LeaveRequestByIdQry { Id = request.Id }, token);
            return result ?? new LeaveRequestListDto();
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ==================== LEAVE TYPE HANDLERS ====================

// ✅ FULLY CORRECTED LeaveTypeAddHandler
public class LeaveTypeAddHandler : IRequestHandler<LeaveTypeAddCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveTypeAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeAddCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            var existing = await _uow.Set<LeaveType>()
                .FirstOrDefaultAsync(x => x.Code == request.AddDto.Code, token);

            if (existing != null)
                throw new DomainException($"Leave type with code '{request.AddDto.Code}' already exists.");

            var data = new LeaveType
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                Code = request.AddDto.Code,
                LeaveCategory = request.AddDto.LeaveCategory,
                Description = request.AddDto.Description,
                AccrualFrequency = request.AddDto.AccrualFrequency,
                AccrualRate = request.AddDto.AccrualRate,
                MaxAccrual = request.AddDto.MaxAccrual,
                AllowCarryover = request.AddDto.AllowCarryover,
                MaxCarryoverDays = request.AddDto.MaxCarryoverDays,
                CarryoverExpiryMonths = request.AddDto.CarryoverExpiryMonths,
                MaxDaysPerRequest = request.AddDto.MaxDaysPerRequest,
                MaxDaysPerYear = request.AddDto.MaxDaysPerYear,
                MinDaysPerRequest = request.AddDto.MinDaysPerRequest,
                RequiresAttachment = request.AddDto.RequiresAttachment,
                RequiresDoctorNote = request.AddDto.RequiresDoctorNote,
                RequiresApproval = request.AddDto.RequiresApproval,
                AllowHalfDay = request.AddDto.AllowHalfDay,
                AllowNegativeBalance = request.AddDto.AllowNegativeBalance,
                HolidaysAsLeave = request.AddDto.HolidaysAsLeave,
                MinServiceMonths = request.AddDto.MinServiceMonths,
                ProbationPeriodOnly = request.AddDto.ProbationPeriodOnly,
                EligibleEmploymentTypes = request.AddDto.EligibleEmploymentTypes,
                SendReminderDays = request.AddDto.SendReminderDays,
                NotifyManagerOnRequest = request.AddDto.NotifyManagerOnRequest,
                Icon = request.AddDto.Icon,
                Color = request.AddDto.Color,
                Priority = request.AddDto.Priority,
                IsActive = true
            };

            await _uow.Add(data, token);
            await _uow.SaveChangesAsync(token);

            return await _med.Send(new LeaveTypeByIdQry { Id = data.Id }, token) ?? new LeaveTypeListDto();
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveTypeModHandler
public class LeaveTypeModHandler : IRequestHandler<LeaveTypeModCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveTypeModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeModCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            var oldData = await _uow.Set<LeaveType>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);
            if (oldData == null)
                throw new DomainException($"LEAVE TYPE with Id {request.ModDto.Id} NOT FOUND.");

            var existing = await _uow.Set<LeaveType>()
                .FirstOrDefaultAsync(x => x.Code == request.ModDto.Code && x.Id != request.ModDto.Id, token);

            if (existing != null)
                throw new DomainException($"Leave type with code '{request.ModDto.Code}' already exists.");

            oldData.Name = request.ModDto.Name;
            oldData.NameAm = request.ModDto.NameAm;
            oldData.Code = request.ModDto.Code;
            oldData.LeaveCategory = request.ModDto.LeaveCategory;
            oldData.Description = request.ModDto.Description;
            oldData.AccrualFrequency = request.ModDto.AccrualFrequency;
            oldData.AccrualRate = request.ModDto.AccrualRate;
            oldData.MaxAccrual = request.ModDto.MaxAccrual;
            oldData.AllowCarryover = request.ModDto.AllowCarryover;
            oldData.MaxCarryoverDays = request.ModDto.MaxCarryoverDays;
            oldData.CarryoverExpiryMonths = request.ModDto.CarryoverExpiryMonths;
            oldData.MaxDaysPerRequest = request.ModDto.MaxDaysPerRequest;
            oldData.MaxDaysPerYear = request.ModDto.MaxDaysPerYear;
            oldData.MinDaysPerRequest = request.ModDto.MinDaysPerRequest;
            oldData.RequiresAttachment = request.ModDto.RequiresAttachment;
            oldData.RequiresDoctorNote = request.ModDto.RequiresDoctorNote;
            oldData.RequiresApproval = request.ModDto.RequiresApproval;
            oldData.AllowHalfDay = request.ModDto.AllowHalfDay;
            oldData.AllowNegativeBalance = request.ModDto.AllowNegativeBalance;
            oldData.HolidaysAsLeave = request.ModDto.HolidaysAsLeave;
            oldData.MinServiceMonths = request.ModDto.MinServiceMonths;
            oldData.ProbationPeriodOnly = request.ModDto.ProbationPeriodOnly;
            oldData.EligibleEmploymentTypes = request.ModDto.EligibleEmploymentTypes;
            oldData.SendReminderDays = request.ModDto.SendReminderDays;
            oldData.NotifyManagerOnRequest = request.ModDto.NotifyManagerOnRequest;
            oldData.Icon = request.ModDto.Icon;
            oldData.Color = request.ModDto.Color;
            oldData.Priority = request.ModDto.Priority;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

            await _uow.Update(oldData);
            await _uow.SaveChangesAsync(token);

            return await _med.Send(new LeaveTypeByIdQry { Id = request.ModDto.Id }, token) ?? new LeaveTypeListDto();
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveTypeStatHandler
public class LeaveTypeStatHandler : IRequestHandler<LeaveTypeStatCmd, LeaveTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveTypeStatHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveTypeListDto> Handle(LeaveTypeStatCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            var oldData = await _uow.Set<LeaveType>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id, token);
            if (oldData == null)
                throw new DomainException($"LEAVE TYPE with Id {request.StatDto.Id} NOT FOUND.");

            oldData.IsActive = request.StatDto.Stat;
            oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));

            await _uow.Update(oldData);
            await _uow.SaveChangesAsync(token);

            return await _med.Send(new LeaveTypeByIdQry { Id = request.StatDto.Id }, token) ?? new LeaveTypeListDto();
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ✅ FULLY CORRECTED LeaveTypeDelHandler
public class LeaveTypeDelHandler : IRequestHandler<LeaveTypeDelCmd>
{
    private readonly IUnitOfWork _uow;

    public LeaveTypeDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(LeaveTypeDelCmd request, CancellationToken ct)
    {
        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<LeaveType>().FirstOrDefaultAsync(x => x.Id == request.Id, token);
            if (data == null)
                throw new DomainException($"LEAVE TYPE with id [{request.Id}] NOT FOUND.");

            await _uow.Delete(data);
            await _uow.SaveChangesAsync(token);
        }, IsolationLevel.ReadCommitted, ct);
    }
}

// ==================== POLICY ASSIGNMENT HANDLERS ====================

// ✅ FULLY CORRECTED PolicyAssignHandler
public class PolicyAssignHandler : IRequestHandler<PolicyAssignCmd, string>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;
    private readonly IHrmProfileClient _hrmPro;

    public PolicyAssignHandler(IDapperHelper dapper, IUnitOfWork uow, IHrmProfileClient hrmPro)
    {
        _dapper = dapper;
        _uow = uow;
        _hrmPro = hrmPro;
    }

    private async Task<List<PolicyCondCtx>> GetCondList(CancellationToken ct)
    {
        const string p = "p";
        const string r = "r";
        const string c = "c";
        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        var qb = new QueryBuilder()
            .SelectAs<LeavePolicy, PolicyCondCtx>(p, x => x.Id, d => d.PolicyId)
            .SelectAs<LeavePolicy, PolicyCondCtx>(p, x => x.LeaveTypeId, d => d.LeaveTypeId)
            .SelectAs<PolicyAssignmentRule, PolicyCondCtx>(r, x => x.Id, d => d.PolAssignRuleId)
            .SelectAs<PolicyAssignmentRule, PolicyCondCtx>(r, x => x.EffectiveFrom, d => d.EffectiveFrom)
            .SelectAs<PolicyAssignmentRule, PolicyCondCtx>(r, x => x.Priority, d => d.Priority)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Id, d => d.PolRuleCondId)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Field, d => d.Field)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Operator, d => d.Operator)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Value, d => d.Value)
            .From<LeavePolicy>(p)
            .Join<LeavePolicy, PolicyAssignmentRule>(p, r, x => x.Id, x => x.LeavePolicyId)
            .Join<PolicyAssignmentRule, PolicyRuleCondition>(r, c, x => x.Id, x => x.PolicyAssignmentRuleId)
            .Where<LeavePolicy>(p, x => x.Status == stat)
            .Where<PolicyAssignmentRule>(r, x => x.IsActive)
            .OrderBy<PolicyAssignmentRule>(r, x => x.EffectiveFrom);

        var (sql, parameters) = qb.Build();
        var result = await _dapper.QueryAsync<PolicyCondCtx>(sql, parameters, ct);
        return result.ToList();
    }

    private async Task<List<EmpPolicyCtx>> GetEmpList(CancellationToken ct)
    {
        var empList = new List<EmpPolicyCtx>();
        var empL = await _hrmPro.GetListEmpPolicy(ct);

        if (empL?.Res != null && empL.Res.Count > 0)
        {
            foreach (var emp in empL.Res)
            {
                try
                {
                    var vm = new EmpPolicyCtx
                    {
                        EmployeeId = Guid.Parse(emp.Id),
                        Name = emp.Name ?? "Unknown",
                        SerYear = string.IsNullOrEmpty(emp.SerYear) ? 0 : double.Parse(emp.SerYear),
                        EmpType = emp.EmpType ?? "",
                        WorkAr = emp.WorkAr ?? "",
                        Gender = emp.Gender ?? "",
                        Jg = emp.Jg ?? "",
                    };
                    empList.Add(vm);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing employee {emp.Id}: {ex.Message}");
                }
            }
        }
        return empList;
    }

    private bool IsSamePolicy(EmpLeavePolicy active, ResolvePolicy resolved)
    {
        double activeEntitlement = (double)active.AssignedEntitlement;
        return active.LeavePolicyId == resolved.LeavePolicyId &&
               Math.Abs(activeEntitlement - resolved.AssignedEntitlement) < 0.0001 &&
               active.EffectiveFrom == resolved.EffectiveFrom;
    }

    public async Task<string> Handle(PolicyAssignCmd request, CancellationToken ct)
    {
        return await _uow.ExecuteAsync(async token =>
        {
            try
            {
                Console.WriteLine("=== POLICY ASSIGNMENT STARTED ===");

                // STEP 1: Get all employees
                var empList = await GetEmpList(token);
                Console.WriteLine($"Employees found: {empList?.Count ?? 0}");
                if (empList == null || !empList.Any())
                    throw new DomainException("NO EMPLOYEES FOUND for leave policy assignment.");

                // STEP 2: Get all conditions
                var allCondList = await GetCondList(token);
                Console.WriteLine($"Conditions found: {allCondList?.Count ?? 0}");
                if (allCondList == null || !allCondList.Any())
                    throw new DomainException("NO ACTIVE LEAVE POLICIES OR CONDITIONS. Please create leave policies and assignment rules first.");

                // STEP 3: Build policy configs query
                const string pc = "pc";
                const string lp = "lp";
                var qbPolicy = new QueryBuilder()
                    .SelectAs<LeavePolicyConfig, ResolvePolicy>(pc, x => x.LeavePolicyId, d => d.LeavePolicyId)
                    .SelectAs<LeavePolicyConfig, ResolvePolicy>(pc, x => x.AnnualEntitlement, d => d.AssignedEntitlement)
                    .SelectAs<LeavePolicy, ResolvePolicy>(lp, x => x.LeaveTypeId, d => d.LeaveTypeId)
                    .From<LeavePolicyConfig>(pc)
                    .Join<LeavePolicyConfig, LeavePolicy>(pc, lp, x => x.LeavePolicyId, x => x.Id)
                    .Where<LeavePolicyConfig>(pc, x => x.IsActive);

                var (sqlPol, paramPol) = qbPolicy.Build();
                Console.WriteLine($"Policy SQL: {sqlPol}");

                var policyConfigs = await _dapper.QueryAsync<LeavePolicyConfig>(sqlPol, paramPol, token);
                Console.WriteLine($"Policy configs found: {policyConfigs?.Count() ?? 0}");

                if (policyConfigs == null)
                    throw new DomainException("Policy configs is NULL");

                var configLookup = policyConfigs.GroupBy(x => x.LeavePolicyId).ToDictionary(g => g.Key, g => g.ToList());
                Console.WriteLine($"Config lookup keys: {configLookup.Count}");

                // STEP 4: Build active policies query
                const string ep = "ep";
                var qbActive = new QueryBuilder()
                    .Select<EmpLeavePolicy>(ep, x => x.Id, x => x.EmployeeId, x => x.LeaveTypeId, x => x.LeavePolicyId!, x => x.EffectiveFrom, x => x.EffectiveTo!, x => x.AssignedEntitlement)
                    .From<EmpLeavePolicy>(ep)
                    .WhereRaw<EmpLeavePolicy>(ep, x => x.EffectiveTo!, "IS NULL");

                var (sqlAct, paramAct) = qbActive.Build();
                Console.WriteLine($"Active policies SQL: {sqlAct}");

                var activePolicies = await _dapper.QueryAsync<EmpLeavePolicy>(sqlAct, paramAct, token);
                Console.WriteLine($"Active policies found: {activePolicies?.Count() ?? 0}");

                var activeDict = activePolicies?.GroupBy(x => (x.EmployeeId, x.LeaveTypeId))
                    .ToDictionary(g => g.Key, g => g.First())
                    ?? new Dictionary<(Guid, Guid), EmpLeavePolicy>();

                // STEP 5: Resolve policies for each employee
                Console.WriteLine("Resolving policies for employees...");
                var resolvedPolicies = new List<ResolvePolicy>();

                foreach (var emp in empList)
                {
                    Console.WriteLine($"Processing employee: {emp.Name} (ID: {emp.EmployeeId})");

                    var res = await LeavePolicyRuleEngine.Resolve(emp, allCondList,
                        policyId => configLookup.TryGetValue(policyId, out var list) ? list : []);

                    foreach (var r in res)
                    {
                        r.EmployeeId = emp.EmployeeId;
                        resolvedPolicies.Add(r);
                    }
                }

                Console.WriteLine($"Resolved policies: {resolvedPolicies.Count}");

                if (!resolvedPolicies.Any())
                    throw new DomainException("NO RESOLVED LEAVE POLICIES FOR EMPLOYEES. Check your assignment rules and conditions.");

                // STEP 6: Update and insert policies
                var updates = new List<EmpLeavePolicy>();
                var inserts = new List<EmpLeavePolicy>();
                var reason = BoolToStr.EnumToString(EmpLeavePolReason.PolChange);
                var today = DateTime.UtcNow;
                var assignedCount = 0;

                foreach (var res in resolvedPolicies)
                {
                    var key = (res.EmployeeId, res.LeaveTypeId);
                    if (activeDict.TryGetValue(key, out var activePolicy))
                    {
                        if (IsSamePolicy(activePolicy, res)) continue;
                        activePolicy.EffectiveTo = today;
                        updates.Add(activePolicy);
                    }

                    inserts.Add(new EmpLeavePolicy
                    {
                        EmployeeId = res.EmployeeId,
                        LeaveTypeId = res.LeaveTypeId,
                        LeavePolicyId = res.LeavePolicyId,
                        EffectiveFrom = res.EffectiveFrom,
                        AssignedEntitlement = (decimal)res.AssignedEntitlement,
                        Reason = reason
                    });
                }

                // STEP 7: Save to database - Use UnitOfWork methods
                foreach (var upd in updates)
                    await _uow.Update(upd);

                foreach (var ins in inserts)
                {
                    await _uow.Add(ins, token);
                    assignedCount++;
                }

                await _uow.SaveChangesAsync(token);
                Console.WriteLine($"Successfully assigned {assignedCount} policies");

                return $"{assignedCount} EMPLOYEES LEAVE POLICY ASSIGNMENTS processed successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new DomainException($"Failed to assign policies: {ex.Message}");
            }
        }, IsolationLevel.ReadCommitted, ct);
    }
}