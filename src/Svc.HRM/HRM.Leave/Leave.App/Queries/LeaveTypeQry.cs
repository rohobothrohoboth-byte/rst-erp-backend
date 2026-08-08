using Common;
using Dapper;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Entities.Local;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Leave.App.Queries;

// ==================== LEAVE TYPE QUERIES ====================
public class LeaveTypeAllQry : IRequest<List<LeaveTypeListDto>> { }
public class LeaveTypeByIdQry : IRequest<LeaveTypeListDto?> { public Guid Id { get; set; } }
public class LeaveTypeNameAllQry : IRequest<List<NameList>> { }
public class LeaveTypeNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class LeavePolicyNameAllQry : IRequest<List<NameList>> { }
public class LeavePolicyNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class EmpLeavePolicyAllQry : IRequest<List<EmpLeavePolicyListDto>> { }
public class EmpLeavePolicyByEmployeeQry : IRequest<List<EmpLeavePolicyListDto>> { public Guid EmployeeId { get; set; } }
public class EmpLeavePolicyByIdQry : IRequest<EmpLeavePolicyListDto?> { public Guid Id { get; set; } }

// ==================== LEAVE REQUEST QUERIES ====================
public class LeaveRequestAllQry : IRequest<List<LeaveRequestListDto>> { }
public class LeaveRequestByIdQry : IRequest<LeaveRequestListDto?> { public Guid Id { get; set; } }
public class LeaveRequestMyQry : IRequest<List<LeaveRequestListDto>> { public Guid EmployeeId { get; set; } }

public class LeaveRequestByIdHandler : IRequestHandler<LeaveRequestByIdQry, LeaveRequestListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;

    public LeaveRequestByIdHandler(IDapperHelper dapper, IUnitOfWork uow)
    {
        _dapper = dapper;
        _uow = uow;
    }

    public async Task<LeaveRequestListDto?> Handle(LeaveRequestByIdQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                lr.""Id"",
                lr.""StartDate"",
                lr.""EndDate"",
                lr.""DaysRequested"",
                lr.""IsHalfDay"",
                lr.""Status"",
                lr.""EmployeeId"",
                lr.""DateAdd"",
                lr.""DateMod"",
                lr.""xmin"",
                lr.""Comments"",
                lt.""Name"" as LeaveType,
                lr.""ApprovalChainId"",
                lr.""CurrentAppStep""
            FROM ""LeaveRequest"" lr
            LEFT JOIN ""LeaveType"" lt ON lr.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            WHERE lr.""Id"" = @Id AND lr.""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<LeaveRequestListDto>(sql, new { request.Id }, ct);

        if (result != null)
        {
            // ✅ Get employee from local database
            var employee = await _uow.Set<LocalEmployee>()
                .FirstOrDefaultAsync(e => e.Id == result.EmployeeId && !e.IsDeleted, ct);
            result.Employee = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown";
            result.StatusStr = GetStatusString(result.Status);
            result.IsHalfDayStr = result.IsHalfDay ? "Yes" : "No";
            result.DaysRequestedStr = $"{result.DaysRequested:F2} days";
        }

        return result;
    }

    private string GetStatusString(string status)
    {
        return status switch
        {
            "0" => "Pending",
            "1" => "Approved",
            "2" => "Rejected",
            "3" => "Cancelled",
            _ => status ?? "Unknown"
        };
    }
}

public class LeaveRequestMyHandler : IRequestHandler<LeaveRequestMyQry, List<LeaveRequestListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;

    public LeaveRequestMyHandler(IDapperHelper dapper, IUnitOfWork uow)
    {
        _dapper = dapper;
        _uow = uow;
    }

    public async Task<List<LeaveRequestListDto>> Handle(LeaveRequestMyQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                lr.""Id"",
                lr.""StartDate"",
                lr.""EndDate"",
                lr.""DaysRequested"",
                lr.""IsHalfDay"",
                lr.""Status"",
                lr.""EmployeeId"",
                lr.""DateAdd"",
                lr.""DateMod"",
                lr.""xmin"",
                lr.""Comments"",
                lt.""Name"" as LeaveType,
                lr.""ApprovalChainId"",
                lr.""CurrentAppStep""
            FROM ""LeaveRequest"" lr
            LEFT JOIN ""LeaveType"" lt ON lr.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            WHERE lr.""EmployeeId"" = @EmployeeId AND lr.""IsDeleted"" = false
            ORDER BY lr.""DateAdd"" DESC";

        var results = await _dapper.QueryAsync<LeaveRequestListDto>(sql, new { request.EmployeeId }, ct);
        var list = results.ToList();

        // ✅ Get employee from local database
        var employee = await _uow.Set<LocalEmployee>()
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && !e.IsDeleted, ct);
        var employeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown";

        foreach (var item in list)
        {
            item.Employee = employeeName;
            item.StatusStr = GetStatusString(item.Status);
            item.IsHalfDayStr = item.IsHalfDay ? "Yes" : "No";
            item.DaysRequestedStr = $"{item.DaysRequested:F2} days";
        }

        return list;
    }

    private string GetStatusString(string status)
    {
        return status switch
        {
            "0" => "Pending",
            "1" => "Approved",
            "2" => "Rejected",
            "3" => "Cancelled",
            _ => status ?? "Unknown"
        };
    }
}

public class LeaveRequestAllHandler : IRequestHandler<LeaveRequestAllQry, List<LeaveRequestListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;

    public LeaveRequestAllHandler(IDapperHelper dapper, IUnitOfWork uow)
    {
        _dapper = dapper;
        _uow = uow;
    }

    public async Task<List<LeaveRequestListDto>> Handle(LeaveRequestAllQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                lr.""Id"",
                lr.""StartDate"",
                lr.""EndDate"",
                lr.""DaysRequested"",
                lr.""IsHalfDay"",
                lr.""Status"",
                lr.""EmployeeId"",
                lr.""DateAdd"",
                lr.""DateMod"",
                lr.""xmin"",
                lr.""Comments"",
                lt.""Name"" as LeaveType,
                lr.""ApprovalChainId"",
                lr.""CurrentAppStep""
            FROM ""LeaveRequest"" lr
            LEFT JOIN ""LeaveType"" lt ON lr.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            WHERE lr.""IsDeleted"" = false
            ORDER BY lr.""DateAdd"" DESC";

        var results = await _dapper.QueryAsync<LeaveRequestListDto>(sql, null, ct);
        var list = results.ToList();

        // ✅ Get all employees from local database
        var employees = await _uow.Set<LocalEmployee>()
            .Where(e => !e.IsDeleted)
            .Select(e => new { e.Id, e.FirstName, e.LastName })
            .ToDictionaryAsync(e => e.Id, e => $"{e.FirstName} {e.LastName}", ct);

        foreach (var item in list)
        {
            employees.TryGetValue(item.EmployeeId, out var empName);
            item.Employee = empName ?? "Unknown";
            item.StatusStr = GetStatusString(item.Status);
            item.IsHalfDayStr = item.IsHalfDay ? "Yes" : "No";
            item.DaysRequestedStr = $"{item.DaysRequested:F2} days";

            // If CurrentStepOrder is null or 0 for pending requests, set it to 1
            if (item.StatusStr == "Pending" && (item.CurrentStepOrder == null || item.CurrentStepOrder == 0))
            {
                item.CurrentStepOrder = 1;
            }
        }

        return list;
    }

    private string GetStatusString(string status)
    {
        return status switch
        {
            "0" => "Pending",
            "1" => "Approved",
            "2" => "Rejected",
            "3" => "Cancelled",
            _ => status ?? "Unknown"
        };
    }
}

// ==================== LEAVE TYPE HANDLERS ====================

public class LeaveTypeAllHandler : IRequestHandler<LeaveTypeAllQry, List<LeaveTypeListDto>>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeaveTypeListDto>> Handle(LeaveTypeAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<LeaveType>(v, x => x.Id, x => x.Name, x => x.LeaveCategory, x => x.RequiresApproval, x => x.AllowHalfDay, x => x.HolidaysAsLeave, x => x.IsActive, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<LeaveType>(v)
            .OrderBy<LeaveType>(v, x => x.Name, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeaveTypeListDto>(ct);

        foreach (var data in list)
        {
            data.LeaveCategoryStr = MyEnumHelper.FormatEnum<LeaveCategory>(data.LeaveCategory);
            data.RequiresApprovalStr = BoolToStr.FormatBool(data.RequiresApproval);
            data.AllowHalfDayStr = BoolToStr.FormatBool(data.AllowHalfDay);
            data.HolidaysAsLeaveStr = BoolToStr.FormatBool(data.HolidaysAsLeave);
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }
        return list;
    }
}

public class LeaveTypeByIdHandler : IRequestHandler<LeaveTypeByIdQry, LeaveTypeListDto?>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeaveTypeListDto?> Handle(LeaveTypeByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<LeaveType>(v, x => x.Id, x => x.Name, x => x.LeaveCategory, x => x.RequiresApproval, x => x.AllowHalfDay, x => x.HolidaysAsLeave, x => x.IsActive, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<LeaveType>(v)
            .Where<LeaveType>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveTypeListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.LeaveCategoryStr = MyEnumHelper.FormatEnum<LeaveCategory>(data.LeaveCategory);
        data.RequiresApprovalStr = BoolToStr.FormatBool(data.RequiresApproval);
        data.AllowHalfDayStr = BoolToStr.FormatBool(data.AllowHalfDay);
        data.HolidaysAsLeaveStr = BoolToStr.FormatBool(data.HolidaysAsLeave);
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class LeaveTypeNameAllHandler : IRequestHandler<LeaveTypeNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(LeaveTypeNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<LeaveType>(v, x => x.Id, x => x.Name).From<LeaveType>(v);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        return await reader.ToListAsync<NameList>(ct);
    }
}

public class LeaveTypeNameByIdHandler : IRequestHandler<LeaveTypeNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public LeaveTypeNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(LeaveTypeNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().SelectDto<LeaveType, NameList>(v).From<LeaveType>(v).Where<LeaveType>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        return await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
    }
}

// ==================== LEAVE POLICY QUERIES ====================

public class LeavePolicyAllQry : IRequest<List<LeavePolicyListDto>> { }
public class LeavePolicyByIdQry : IRequest<LeavePolicyListDto?> { public Guid Id { get; set; } }
public class ActiveLeavePolicyQry : IRequest<List<LeavePolicyListDto>> { }

public class LeavePolicyAllHandler : IRequestHandler<LeavePolicyAllQry, List<LeavePolicyListDto>>
{
    private readonly IDapperHelper _dapper;
    public LeavePolicyAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeavePolicyListDto>> Handle(LeavePolicyAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicy>(v, x => x.Id, x => x.Name, x => x.Code, x => x.AllowEncashment, x => x.RequiresAttachment, x => x.Status, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeaveType, LeavePolicyListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeavePolicy>(v)
            .Join<LeavePolicy, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .OrderBy<LeavePolicy>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeavePolicyListDto>(ct);

        foreach (var data in list)
        {
            data.StatusStr = MyEnumHelper.FormatEnum<PolicyStatus>(data.Status);
            data.AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment);
            data.RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment);
            data.RowVersion = data.xmin.ToString();
        }
        return list;
    }
}

public class LeavePolicyByIdHandler : IRequestHandler<LeavePolicyByIdQry, LeavePolicyListDto?>
{
    private readonly IDapperHelper _dapper;
    public LeavePolicyByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeavePolicyListDto?> Handle(LeavePolicyByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicy>(v, x => x.Id, x => x.Name, x => x.Code, x => x.AllowEncashment, x => x.RequiresAttachment, x => x.Status, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeaveType, LeavePolicyListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeavePolicy>(v)
            .Join<LeavePolicy, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeavePolicy>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeavePolicyListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.StatusStr = MyEnumHelper.FormatEnum<PolicyStatus>(data.Status);
        data.AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment);
        data.RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActiveLeavePolicyHandler : IRequestHandler<ActiveLeavePolicyQry, List<LeavePolicyListDto>>
{
    private readonly IDapperHelper _dapper;
    public ActiveLeavePolicyHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeavePolicyListDto>> Handle(ActiveLeavePolicyQry request, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicy>(v, x => x.Id, x => x.Name, x => x.Code, x => x.AllowEncashment, x => x.RequiresAttachment, x => x.Status, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeaveType, LeavePolicyListDto>(c, x => x.Name, d => d.LeaveType)
            .From<LeavePolicy>(v)
            .Join<LeavePolicy, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeavePolicy>(v, x => x.Status == stat)
            .OrderBy<LeavePolicy>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeavePolicyListDto>(ct);

        foreach (var data in list)
        {
            data.StatusStr = MyEnumHelper.FormatEnum<PolicyStatus>(data.Status);
            data.AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment);
            data.RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment);
            data.RowVersion = data.xmin.ToString();
        }
        return list;
    }
}

public class LeavePolicyNameAllHandler : IRequestHandler<LeavePolicyNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public LeavePolicyNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(LeavePolicyNameAllQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT ""Id"", ""Name""
            FROM ""LeavePolicy""
            WHERE ""IsDeleted"" = false
            ORDER BY ""Name""";

        var result = await _dapper.QueryAsync<NameList>(sql, null, ct);
        return result.ToList();
    }
}

public class LeavePolicyNameByIdHandler : IRequestHandler<LeavePolicyNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public LeavePolicyNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(LeavePolicyNameByIdQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT ""Id"", ""Name""
            FROM ""LeavePolicy""
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        return await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, new { Id = request.Id }, ct);
    }
}

// ==================== POLICY CONFIGURATION QUERIES ====================

public class PolicyConfigByPolicyIdQry : IRequest<List<LeavePolicyConfigListDto>> { public Guid Id { get; set; } }
public class LeavePolicyConfigByIdQry : IRequest<LeavePolicyConfigListDto?> { public Guid Id { get; set; } }
public class ActivePolicyConfigQry : IRequest<LeavePolicyConfigListDto?> { public Guid Id { get; set; } }

public class PolicyConfigByPolicyIdHandler : IRequestHandler<PolicyConfigByPolicyIdQry, List<LeavePolicyConfigListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    public PolicyConfigByPolicyIdHandler(IDapperHelper dapper, ICorModClient corMod) { _dapper = dapper; _corMod = corMod; }

    public async Task<List<LeavePolicyConfigListDto>> Handle(PolicyConfigByPolicyIdQry request, CancellationToken ct)
    {
        var fyTask = await _corMod.GetListFiscalYear(ct);
        var fyDict = fyTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicyConfig>(v, x => x.Id, x => x.AnnualEntitlement, x => x.AccrualFrequency, x => x.AccrualRate, x => x.MaxDaysPerReq, x => x.MaxCarryOverDays, x => x.MinServiceMonths, x => x.IsActive, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeavePolicy, LeavePolicyConfigListDto>(c, x => x.Name, d => d.LeavePolicy)
            .From<LeavePolicyConfig>(v)
            .Join<LeavePolicyConfig, LeavePolicy>(v, c, x => x.LeavePolicyId, x => x.Id)
            .Where<LeavePolicyConfig>(v, x => x.LeavePolicyId == request.Id);

        var (sql, parameters) = qb.Build();
        var result = new List<LeavePolicyConfigListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<LeavePolicyConfigListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            fyDict.TryGetValue(data.FiscalYearId, out var fy);

            result.Add(new LeavePolicyConfigListDto
            {
                Id = data.Id,
                AnnualEntitlement = data.AnnualEntitlement,
                AccrualFrequency = data.AccrualFrequency,
                AccrualRate = data.AccrualRate,
                MaxDaysPerReq = data.MaxDaysPerReq,
                MaxCarryOverDays = data.MaxCarryOverDays,
                MinServiceMonths = data.MinServiceMonths,
                IsActive = data.IsActive,
                AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s",
                AccrualFrequencyStr = MyEnumHelper.FormatEnum<AccrualFrequency>(data.AccrualFrequency),
                AccrualRateStr = $"{data.AccrualRate} day/s",
                MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s",
                MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s",
                MinServiceMonthsStr = $"{data.MinServiceMonths} month/s",
                IsActiveStr = BoolToStr.FormatStat(data.IsActive),
                FiscalYear = fy?.Name ?? "",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return result;
    }
}

public class LeavePolicyConfigByIdHandler : IRequestHandler<LeavePolicyConfigByIdQry, LeavePolicyConfigListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    public LeavePolicyConfigByIdHandler(IDapperHelper dapper, ICorModClient corMod) { _dapper = dapper; _corMod = corMod; }

    public async Task<LeavePolicyConfigListDto?> Handle(LeavePolicyConfigByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicyConfig>(v, x => x.Id, x => x.AnnualEntitlement, x => x.AccrualFrequency, x => x.AccrualRate, x => x.MaxDaysPerReq, x => x.MaxCarryOverDays, x => x.MinServiceMonths, x => x.IsActive, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeavePolicy, LeavePolicyConfigListDto>(c, x => x.Name, d => d.LeavePolicy)
            .From<LeavePolicyConfig>(v)
            .Join<LeavePolicyConfig, LeavePolicy>(v, c, x => x.LeavePolicyId, x => x.Id)
            .Where<LeavePolicyConfig>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeavePolicyConfigListDto>(sql, parameters, ct);
        if (data == null) return null;

        var fy = await _corMod.GetFiscalYear(data.FiscalYearId.ToString(), ct);
        data.AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s";
        data.AccrualFrequencyStr = MyEnumHelper.FormatEnum<AccrualFrequency>(data.AccrualFrequency);
        data.AccrualRateStr = $"{data.AccrualRate} day/s";
        data.MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s";
        data.MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s";
        data.MinServiceMonthsStr = $"{data.MinServiceMonths} month/s";
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.FiscalYear = fy.Res.Name ?? "NOT AVAILABLE";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActivePolicyConfigHandler : IRequestHandler<ActivePolicyConfigQry, LeavePolicyConfigListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    public ActivePolicyConfigHandler(IDapperHelper dapper, ICorModClient corMod) { _dapper = dapper; _corMod = corMod; }

    public async Task<LeavePolicyConfigListDto?> Handle(ActivePolicyConfigQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeavePolicyConfig>(v, x => x.Id, x => x.AnnualEntitlement, x => x.AccrualFrequency, x => x.AccrualRate, x => x.MaxDaysPerReq, x => x.MaxCarryOverDays, x => x.MinServiceMonths, x => x.IsActive, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeavePolicy, LeavePolicyConfigListDto>(c, x => x.Name, d => d.LeavePolicy)
            .From<LeavePolicyConfig>(v)
            .Join<LeavePolicyConfig, LeavePolicy>(v, c, x => x.LeavePolicyId, x => x.Id)
            .Where<LeavePolicyConfig>(v, x => x.LeavePolicyId == request.Id && x.IsActive == true)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeavePolicyConfigListDto>(sql, parameters, ct);
        if (data == null) return null;

        var fy = await _corMod.GetFiscalYear(data.FiscalYearId.ToString(), ct);
        data.AnnualEntitlementStr = $"{data.AnnualEntitlement} day/s";
        data.AccrualFrequencyStr = MyEnumHelper.FormatEnum<AccrualFrequency>(data.AccrualFrequency);
        data.AccrualRateStr = $"{data.AccrualRate} day/s";
        data.MaxDaysPerReqStr = $"{data.MaxDaysPerReq} day/s";
        data.MaxCarryOverDaysStr = $"{data.MaxCarryOverDays} day/s";
        data.MinServiceMonthsStr = $"{data.MinServiceMonths} month/s";
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.FiscalYear = fy.Res.Name ?? "NOT AVAILABLE";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

// ==================== APPROVAL CHAIN QUERIES ====================

public class LeaveAppChainByPolicyIdQry : IRequest<List<LeaveAppChainListDto>> { public Guid Id { get; set; } }
public class LeaveAppChainByIdQry : IRequest<LeaveAppChainListDto?> { public Guid Id { get; set; } }
public class ActiveLeaveAppChainQry : IRequest<LeaveAppChainListDto?> { public Guid Id { get; set; } }

public class LeaveAppChainByPolicyIdHandler : IRequestHandler<LeaveAppChainByPolicyIdQry, List<LeaveAppChainListDto>>
{
    private readonly IDapperHelper _dapper;
    public LeaveAppChainByPolicyIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<LeaveAppChainListDto>> Handle(LeaveAppChainByPolicyIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<LeaveAppChain>(v, x => x.Id, x => x.EffectiveFrom, x => x.EffectiveTo!, x => x.IsActive, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeavePolicy, LeaveAppChainListDto>(p, x => x.Name, d => d.LeavePolicy)
            .From<LeaveAppChain>(v)
            .Join<LeaveAppChain, LeavePolicy>(v, p, x => x.LeavePolicyId!, x => x.Id)
            .OrderBy<LeaveAppChain>(v, x => x.DateAdd, desc: true)
            .Where<LeaveAppChain>(v, x => x.LeavePolicyId == request.Id);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<LeaveAppChainListDto>(ct);

        foreach (var item in list)
        {
            item.IsActiveStr = BoolToStr.FormatStat(item.IsActive);
            item.RowVersion = item.xmin.ToString();
        }
        return list;
    }
}

public class LeaveAppChainByIdHandler : IRequestHandler<LeaveAppChainByIdQry, LeaveAppChainListDto?>
{
    private readonly IDapperHelper _dapper;
    public LeaveAppChainByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeaveAppChainListDto?> Handle(LeaveAppChainByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<LeaveAppChain>(v, x => x.Id, x => x.EffectiveFrom, x => x.EffectiveTo!, x => x.IsActive, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeavePolicy, LeaveAppChainListDto>(p, x => x.Name, d => d.LeavePolicy)
            .From<LeaveAppChain>(v)
            .Join<LeaveAppChain, LeavePolicy>(v, p, x => x.LeavePolicyId!, x => x.Id)
            .Where<LeaveAppChain>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveAppChainListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActiveLeaveAppChainHandler : IRequestHandler<ActiveLeaveAppChainQry, LeaveAppChainListDto?>
{
    private readonly IDapperHelper _dapper;
    public ActiveLeaveAppChainHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<LeaveAppChainListDto?> Handle(ActiveLeaveAppChainQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<LeaveAppChain>(v, x => x.Id, x => x.EffectiveFrom, x => x.EffectiveTo!, x => x.IsActive, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<LeavePolicy, LeaveAppChainListDto>(p, x => x.Name, d => d.LeavePolicy)
            .From<LeaveAppChain>(v)
            .Join<LeaveAppChain, LeavePolicy>(v, p, x => x.LeavePolicyId!, x => x.Id)
            .OrderBy<LeaveAppChain>(v, x => x.DateAdd, desc: true)
            .Where<LeaveAppChain>(v, x => x.LeavePolicyId == request.Id && x.IsActive == true)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveAppChainListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

// ==================== APPROVAL STEP QUERIES ====================

public class AppStepByChainIdQry : IRequest<List<LeaveAppStepListDto>> { public Guid Id { get; set; } }
public class LeaveAppStepByIdQry : IRequest<LeaveAppStepListDto?> { public Guid Id { get; set; } }

public class AppStepByChainIdHandler : IRequestHandler<AppStepByChainIdQry, List<LeaveAppStepListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;

    public AppStepByChainIdHandler(IDapperHelper dapper, IUnitOfWork uow)
    {
        _dapper = dapper;
        _uow = uow;
    }

    public async Task<List<LeaveAppStepListDto>> Handle(AppStepByChainIdQry request, CancellationToken ct)
    {
        // ✅ Get employees from local database
        var employees = await _uow.Set<LocalEmployee>()
            .Where(e => !e.IsDeleted)
            .Select(e => new { e.Id, e.FirstName, e.LastName })
            .ToDictionaryAsync(e => e.Id, e => $"{e.FirstName} {e.LastName}", ct);

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveAppStep>(v, x => x.Id, x => x.StepName, x => x.StepOrder, x => x.Role, x => x.IsFinal, x => x.EmployeeId!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<LeaveAppChain>(c, x => x.EffectiveFrom)
            .From<LeaveAppStep>(v)
            .Join<LeaveAppStep, LeaveAppChain>(v, c, x => x.LeaveAppChainId, x => x.Id)
            .Where<LeaveAppStep>(v, x => x.LeaveAppChainId == request.Id);

        var (sql, parameters) = qb.Build();
        var result = new List<LeaveAppStepListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<LeaveAppStepListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            var emp = "Not Assigned";
            if (data.EmployeeId != null)
            {
                employees.TryGetValue((Guid)data.EmployeeId, out var empN);
                emp = empN ?? "Not Assigned";
            }

            result.Add(new LeaveAppStepListDto
            {
                Id = data.Id,
                StepName = data.StepName,
                StepOrder = data.StepOrder,
                Role = data.Role,
                IsFinal = data.IsFinal,
                RoleStr = MyEnumHelper.FormatEnum<ApprovalRole>(data.Role),
                IsFinalStr = BoolToStr.FormatBool(data.IsFinal),
                Employee = emp,
                LeaveAppChain = $"From : {data.EffectiveFrom:MMMM dd, yyyy}",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return result;
    }
}

public class LeaveAppStepByIdHandler : IRequestHandler<LeaveAppStepByIdQry, LeaveAppStepListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;

    public LeaveAppStepByIdHandler(IDapperHelper dapper, IUnitOfWork uow)
    {
        _dapper = dapper;
        _uow = uow;
    }

    public async Task<LeaveAppStepListDto?> Handle(LeaveAppStepByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveAppStep>(v, x => x.Id, x => x.StepName, x => x.StepOrder, x => x.Role, x => x.IsFinal, x => x.EmployeeId!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<LeaveAppChain>(c, x => x.EffectiveFrom)
            .From<LeaveAppStep>(v)
            .Join<LeaveAppStep, LeaveAppChain>(v, c, x => x.LeaveAppChainId, x => x.Id)
            .Where<LeaveAppStep>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveAppStepListDto>(sql, parameters, ct);
        if (data == null) return null;

        // ✅ Get employee from local database
        var emp = "NOT ASSIGNED";
        if (data.EmployeeId != null)
        {
            var employee = await _uow.Set<LocalEmployee>()
                .FirstOrDefaultAsync(e => e.Id == data.EmployeeId && !e.IsDeleted, ct);
            if (employee != null)
            {
                emp = $"{employee.FirstName} {employee.LastName}";
            }
        }

        data.RoleStr = MyEnumHelper.FormatEnum<ApprovalRole>(data.Role);
        data.IsFinalStr = BoolToStr.FormatBool(data.IsFinal);
        data.Employee = emp;
        data.LeaveAppChain = $"From : {data.EffectiveFrom:MMMM dd, yyyy}";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

// ==================== ASSIGNMENT RULE QUERIES ====================

public class AssignmentRuleByPolicyIdQry : IRequest<List<PolicyAssignmentRuleListDto>> { public Guid Id { get; set; } }
public class PolicyAssignmentRuleByIdQry : IRequest<PolicyAssignmentRuleListDto?> { public Guid Id { get; set; } }
public class ActiveAssignmentRulesQry : IRequest<List<PolicyAssignmentRuleListDto>> { public Guid Id { get; set; } }

public class AssignmentRuleByPolicyIdHandler : IRequestHandler<AssignmentRuleByPolicyIdQry, List<PolicyAssignmentRuleListDto>>
{
    private readonly IDapperHelper _dapper;
    public AssignmentRuleByPolicyIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PolicyAssignmentRuleListDto>> Handle(AssignmentRuleByPolicyIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PolicyAssignmentRule>(v, x => x.Id, x => x.Name, x => x.Code, x => x.Priority, x => x.IsActive, x => x.EffectiveFrom, x => x.EffectiveTo!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<PolicyAssignmentRule>(v)
            .OrderBy<PolicyAssignmentRule>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PolicyAssignmentRuleListDto>(ct);

        foreach (var data in list)
        {
            data.PriorityStr = MyEnumHelper.FormatEnum<Priority>(data.Priority);
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }
        return list;
    }
}

public class PolicyAssignmentRuleByIdHandler : IRequestHandler<PolicyAssignmentRuleByIdQry, PolicyAssignmentRuleListDto?>
{
    private readonly IDapperHelper _dapper;
    public PolicyAssignmentRuleByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PolicyAssignmentRuleListDto?> Handle(PolicyAssignmentRuleByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PolicyAssignmentRule>(v, x => x.Id, x => x.Name, x => x.Code, x => x.Priority, x => x.IsActive, x => x.EffectiveFrom, x => x.EffectiveTo!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<PolicyAssignmentRule>(v)
            .Where<PolicyAssignmentRule>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PolicyAssignmentRuleListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.PriorityStr = MyEnumHelper.FormatEnum<Priority>(data.Priority);
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActiveAssignmentRulesHandler : IRequestHandler<ActiveAssignmentRulesQry, List<PolicyAssignmentRuleListDto>>
{
    private readonly IDapperHelper _dapper;
    public ActiveAssignmentRulesHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PolicyAssignmentRuleListDto>> Handle(ActiveAssignmentRulesQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PolicyAssignmentRule>(v, x => x.Id, x => x.Name, x => x.Code, x => x.Priority, x => x.IsActive, x => x.EffectiveFrom, x => x.EffectiveTo!, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<PolicyAssignmentRule>(v)
            .Where<PolicyAssignmentRule>(v, x => x.LeavePolicyId == request.Id && x.IsActive == true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PolicyAssignmentRuleListDto>(ct);

        foreach (var data in list)
        {
            data.PriorityStr = MyEnumHelper.FormatEnum<Priority>(data.Priority);
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }
        return list;
    }
}

// ==================== RULE CONDITION QUERIES ====================

public class PolicyRuleCondByRuleIdQry : IRequest<List<PolicyRuleCondListDto>> { public Guid Id { get; set; } }
public class PolicyRuleCondByIdQry : IRequest<PolicyRuleCondListDto?> { public Guid Id { get; set; } }

public class PolicyRuleCondByRuleIdHandler : IRequestHandler<PolicyRuleCondByRuleIdQry, List<PolicyRuleCondListDto>>
{
    private readonly IDapperHelper _dapper;
    public PolicyRuleCondByRuleIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PolicyRuleCondListDto>> Handle(PolicyRuleCondByRuleIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PolicyRuleCondition>(v, x => x.Id, x => x.Field, x => x.Operator, x => x.Value, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<PolicyAssignmentRule, PolicyRuleCondListDto>(c, x => x.Name, d => d.RuleName)
            .From<PolicyRuleCondition>(v)
            .Join<PolicyRuleCondition, PolicyAssignmentRule>(v, c, x => x.PolicyAssignmentRuleId, x => x.Id)
            .Where<PolicyRuleCondition>(v, x => x.PolicyAssignmentRuleId == request.Id)
            .OrderBy<PolicyRuleCondition>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PolicyRuleCondListDto>(ct);

        foreach (var data in list)
        {
            data.FieldStr = MyEnumHelper.FormatEnum<ConditionField>(data.Field);
            data.OperatorStr = MyEnumHelper.FormatEnum<ConditionOperator>(data.Operator);
            data.RowVersion = data.xmin.ToString();
        }
        return list;
    }
}

public class PolicyRuleCondByIdHandler : IRequestHandler<PolicyRuleCondByIdQry, PolicyRuleCondListDto?>
{
    private readonly IDapperHelper _dapper;
    public PolicyRuleCondByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PolicyRuleCondListDto?> Handle(PolicyRuleCondByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PolicyRuleCondition>(v, x => x.Id, x => x.Field, x => x.Operator, x => x.Value, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<PolicyAssignmentRule, PolicyRuleCondListDto>(c, x => x.Name, d => d.RuleName)
            .From<PolicyRuleCondition>(v)
            .Join<PolicyRuleCondition, PolicyAssignmentRule>(v, c, x => x.PolicyAssignmentRuleId, x => x.Id)
            .Where<PolicyRuleCondition>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PolicyRuleCondListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.FieldStr = MyEnumHelper.FormatEnum<ConditionField>(data.Field);
        data.OperatorStr = MyEnumHelper.FormatEnum<ConditionOperator>(data.Operator);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

// ==================== LEAVE BALANCE QUERIES ====================

public class EmpLeaveBalQry : IRequest<List<EmpLeaveBal>>
{
    public Guid Id { get; set; }  // Employee ID
}

// Raw DTO for database query
internal class EmpLeaveBalanceRaw
{
    public Guid LeaveTypeId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeavePolicyId { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public double AssignedEntitlement { get; set; }
    public double Balance { get; set; }
    public double RemainingBalance { get; set; }
    public double CarryForward { get; set; }
}

public class EmpLeaveBalHandler : IRequestHandler<EmpLeaveBalQry, List<EmpLeaveBal>>
{
    private readonly IDapperHelper _dapper;

    public EmpLeaveBalHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<EmpLeaveBal>> Handle(EmpLeaveBalQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                ep.""LeaveTypeId"",
                ep.""EmployeeId"",
                ep.""LeavePolicyId"",
                lt.""Name"" as LeaveType,
                ep.""AssignedEntitlement"",
                COALESCE(ep.""UsedEntitlement"", 0) as Balance,
                (ep.""AssignedEntitlement"" - COALESCE(ep.""UsedEntitlement"", 0)) as RemainingBalance,
                COALESCE(ep.""CarryForward"", 0) as CarryForward
            FROM public.""EmpLeavePolicy"" ep
            INNER JOIN public.""LeaveType"" lt ON ep.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            WHERE ep.""EmployeeId"" = @EmployeeId::uuid
            AND ep.""IsDeleted"" = false
            AND (ep.""EffectiveTo"" IS NULL OR ep.""EffectiveTo"" >= CURRENT_DATE)
            ORDER BY lt.""Name""";

        var results = await _dapper.QueryAsync<EmpLeaveBalanceRaw>(sql, new { EmployeeId = request.Id }, ct);

        return results.Select(r => new EmpLeaveBal
        {
            LeaveType = r.LeaveType,
            Percent = 100,
            AssignedEntitlement = r.AssignedEntitlement,
            Balance = r.Balance,
            EmployeeId = r.EmployeeId,
            LeaveTypeId = r.LeaveTypeId,
            LeavePolicyId = r.LeavePolicyId,
            TotalDays = $"{r.AssignedEntitlement:F2} days",
            UsedDays = $"{r.Balance:F2} days",
            RemainDays = $"{r.RemainingBalance:F2} days",
            CarryForward = $"{r.CarryForward:F2} days"
        }).ToList();
    }
}

// ==================== EMPLOYEE LEAVE POLICY QUERIES ====================

public class EmpLeavePolicyAllHandler : IRequestHandler<EmpLeavePolicyAllQry, List<EmpLeavePolicyListDto>>
{
    private readonly IDapperHelper _dapper;

    public EmpLeavePolicyAllHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<EmpLeavePolicyListDto>> Handle(EmpLeavePolicyAllQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                ep.""Id"", ep.""EmployeeId"", ep.""LeaveTypeId"", ep.""LeavePolicyId"",
                ep.""AssignedEntitlement"", ep.""UsedEntitlement"", ep.""EffectiveFrom"",
                ep.""EffectiveTo"", ep.""AssignmentReason"", ep.""IsDeleted"", ep.""DateAdd"",
                ep.""DateMod"", ep.""xmin"",
                lt.""Name"" as LeaveTypeName,
                lp.""Name"" as LeavePolicyName
            FROM public.""EmpLeavePolicy"" ep
            LEFT JOIN public.""LeaveType"" lt ON ep.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            LEFT JOIN public.""LeavePolicy"" lp ON ep.""LeavePolicyId"" = lp.""Id"" AND lp.""IsDeleted"" = false
            WHERE ep.""IsDeleted"" = false
            ORDER BY ep.""DateAdd"" DESC";

        var results = await _dapper.QueryAsync<EmpLeavePolicyListDto>(sql, null, ct);
        return results.ToList();
    }
}

public class EmpLeavePolicyByEmployeeHandler : IRequestHandler<EmpLeavePolicyByEmployeeQry, List<EmpLeavePolicyListDto>>
{
    private readonly IDapperHelper _dapper;

    public EmpLeavePolicyByEmployeeHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<EmpLeavePolicyListDto>> Handle(EmpLeavePolicyByEmployeeQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                ep.""Id"", ep.""EmployeeId"", ep.""LeaveTypeId"", ep.""LeavePolicyId"",
                ep.""AssignedEntitlement"", ep.""UsedEntitlement"", ep.""EffectiveFrom"",
                ep.""EffectiveTo"", ep.""AssignmentReason"", ep.""IsDeleted"", ep.""DateAdd"",
                ep.""DateMod"", ep.""xmin"",
                lt.""Name"" as LeaveTypeName,
                lp.""Name"" as LeavePolicyName
            FROM public.""EmpLeavePolicy"" ep
            LEFT JOIN public.""LeaveType"" lt ON ep.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            LEFT JOIN public.""LeavePolicy"" lp ON ep.""LeavePolicyId"" = lp.""Id"" AND lp.""IsDeleted"" = false
            WHERE ep.""EmployeeId"" = @EmployeeId AND ep.""IsDeleted"" = false
            ORDER BY ep.""DateAdd"" DESC";

        var results = await _dapper.QueryAsync<EmpLeavePolicyListDto>(sql, new { request.EmployeeId }, ct);
        return results.ToList();
    }
}

public class EmpLeavePolicyByIdHandler : IRequestHandler<EmpLeavePolicyByIdQry, EmpLeavePolicyListDto?>
{
    private readonly IDapperHelper _dapper;

    public EmpLeavePolicyByIdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<EmpLeavePolicyListDto?> Handle(EmpLeavePolicyByIdQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                ep.""Id"", ep.""EmployeeId"", ep.""LeaveTypeId"", ep.""LeavePolicyId"",
                ep.""AssignedEntitlement"", ep.""UsedEntitlement"", ep.""EffectiveFrom"",
                ep.""EffectiveTo"", ep.""AssignmentReason"", ep.""IsDeleted"", ep.""DateAdd"",
                ep.""DateMod"", ep.""xmin"",
                lt.""Name"" as LeaveTypeName,
                lp.""Name"" as LeavePolicyName
            FROM public.""EmpLeavePolicy"" ep
            LEFT JOIN public.""LeaveType"" lt ON ep.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            LEFT JOIN public.""LeavePolicy"" lp ON ep.""LeavePolicyId"" = lp.""Id"" AND lp.""IsDeleted"" = false
            WHERE ep.""Id"" = @Id AND ep.""IsDeleted"" = false";

        return await _dapper.QueryFirstOrDefaultAsync<EmpLeavePolicyListDto>(sql, new { request.Id }, ct);
    }
}