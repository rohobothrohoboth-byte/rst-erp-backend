using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using Dapper;

namespace Profile.App.Queries;

public class EmpDbRepQry : IRequest<EmpDbReport> { }
public class EmpDbPendQry : IRequest<List<EmpDbPendList>> { }
public class EmpDbPendEduExpQry : IRequest<List<EmpDbPendEduExpList>> { }
public class HrDashboardQry : IRequest<HrDashboardDto> { }

// ============================================================
// LOCAL REFERENCE DATA (Cached from Database)
// ============================================================
public class LocalReferenceData
{
    public Dictionary<Guid, string> Departments { get; set; } = new();
    public Dictionary<Guid, string> DepartmentAmharic { get; set; } = new();
    public Dictionary<Guid, string> Positions { get; set; } = new();
    public Dictionary<Guid, string> JobGrades { get; set; } = new();
}

// ============================================================
// LOCAL DTOs for Database Queries (Internal use only)
// ============================================================
public class DepartmentLocalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
}

public class PositionLocalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class JobGradeLocalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// ============================================================
// UNIFIED HR DASHBOARD HANDLER
// ============================================================
public class HrDashboardHandler(
    IDapperHelper _dapper,
    IMemoryCache _cache) : IRequestHandler<HrDashboardQry, HrDashboardDto>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private const string CacheKey = "hr_dashboard_data";
    private const string RefCacheKey = "hr_reference_data_local";

    public async Task<HrDashboardDto> Handle(HrDashboardQry request, CancellationToken ct)
    {
        // Try get from cache
        if (_cache.TryGetValue(CacheKey, out HrDashboardDto? cached) && cached is not null)
        {
            return cached;
        }

        // Get reference data from local database
        var refData = await GetLocalReferenceDataAsync(ct);

        // Get employee data
        var employeeData = await GetEmployeeDataAsync(ct);

        // Build dashboard
        var dashboard = BuildDashboard(employeeData, refData);

        _cache.Set(CacheKey, dashboard, CacheDuration);

        return dashboard;
    }

    private async Task<LocalReferenceData> GetLocalReferenceDataAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(RefCacheKey, out LocalReferenceData? cached) && cached is not null)
        {
            return cached;
        }

        var refData = new LocalReferenceData();

        // Get Departments from local table
        const string deptSql = @"
            SELECT ""Id"", ""Name"", ""NameAm""
            FROM ""Departments""
            WHERE ""IsDeleted"" = false";

        var departments = await _dapper.QueryAsync<DepartmentLocalDto>(deptSql, null, ct);
        foreach (var dept in departments)
        {
            refData.Departments[dept.Id] = dept.Name;
            refData.DepartmentAmharic[dept.Id] = dept.NameAm;
        }

        // Get Positions from local table
        const string posSql = @"
            SELECT ""Id"", ""Name""
            FROM ""Positions""
            WHERE ""IsDeleted"" = false";

        var positions = await _dapper.QueryAsync<PositionLocalDto>(posSql, null, ct);
        foreach (var pos in positions)
        {
            refData.Positions[pos.Id] = pos.Name;
        }

        // Get Job Grades from local table
        const string jgSql = @"
            SELECT ""Id"", ""Name""
            FROM ""JobGrades""
            WHERE ""IsDeleted"" = false";

        var jobGrades = await _dapper.QueryAsync<JobGradeLocalDto>(jgSql, null, ct);
        foreach (var jg in jobGrades)
        {
            refData.JobGrades[jg.Id] = jg.Name;
        }

        _cache.Set(RefCacheKey, refData, CacheDuration);

        return refData;
    }

    private async Task<List<EmployeeDataDto>> GetEmployeeDataAsync(CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";

        var qb = new QueryBuilder()
            .Select<Employee>(e,
                x => x.Id,
                x => x.Code,
                x => x.EmpState,
                x => x.JobGradeId,
                x => x.DepartmentId,
                x => x.PositionId,
                x => x.DateAdd)
            .Select<Person>(p,
                x => x.FirstName,
                x => x.MiddleName,
                x => x.LastName,
                x => x.FirstNameAm,
                x => x.MiddleNameAm,
                x => x.LastNameAm,
                x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .OrderBy<Employee>(e, x => x.DateAdd, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);

        return await reader.ToListAsync<EmployeeDataDto>(ct);
    }

    private HrDashboardDto BuildDashboard(
        List<EmployeeDataDto> employees,
        LocalReferenceData refData)
    {
        var dashboard = new HrDashboardDto
        {
            TotalEmployees = employees.Count,
            EmployeesByStatus = new Dictionary<string, int>(),
            EmployeesByDepartment = new Dictionary<string, int>(),
            EmployeesByPosition = new Dictionary<string, int>(),
            PendingEmployeesList = new List<EmpDbPendList>(),
            PendingEducationExperienceList = new List<EmpDbPendEduExpList>()
        };

        var statusCounts = new Dictionary<string, int>();

        // Get enum names
        var activeEnumName = Enum.GetName(typeof(EmpState), EmpState.Active) ?? "Active";
        var penEnumName = Enum.GetName(typeof(EmpState), EmpState.Pen) ?? "Pen";
        var susEnumName = Enum.GetName(typeof(EmpState), EmpState.Sus) ?? "Sus";
        var retireEnumName = Enum.GetName(typeof(EmpState), EmpState.Retire) ?? "Retire";
        var standByEnumName = Enum.GetName(typeof(EmpState), EmpState.StandBy) ?? "StandBy";
        var termEnumName = Enum.GetName(typeof(EmpState), EmpState.Term) ?? "Term";
        var leaveEnumName = Enum.GetName(typeof(EmpState), EmpState.Leave) ?? "Leave";
        var rejEnumName = Enum.GetName(typeof(EmpState), EmpState.Rej) ?? "Rej";

        foreach (var emp in employees)
        {
            var status = emp.EmpState ?? "Unknown";

            // Count by status
            statusCounts.TryGetValue(status, out var count);
            statusCounts[status] = count + 1;

            // Get reference data from local cache
            refData.Departments.TryGetValue(emp.DepartmentId, out var deptName);
            refData.DepartmentAmharic.TryGetValue(emp.DepartmentId, out var deptNameAm);
            refData.Positions.TryGetValue(emp.PositionId, out var posName);
            refData.JobGrades.TryGetValue(emp.JobGradeId, out var jgName);

            // Count by department
            var deptKey = deptName ?? "Unassigned";
            dashboard.EmployeesByDepartment.TryGetValue(deptKey, out var deptCount);
            dashboard.EmployeesByDepartment[deptKey] = deptCount + 1;

            // Count by position
            var posKey = posName ?? "Unassigned";
            dashboard.EmployeesByPosition.TryGetValue(posKey, out var posCount);
            dashboard.EmployeesByPosition[posKey] = posCount + 1;

            // Build full name
            var fullName = $"{emp.FirstName} {emp.MiddleName} {emp.LastName}".Trim();
            var fullNameAm = $"{emp.FirstNameAm} {emp.MiddleNameAm} {emp.LastNameAm}".Trim();

            // Add to pending lists if status is "Pen" (the enum name stored in database)
            if (status == penEnumName || status == "0")
            {
                dashboard.PendingEmployeesList.Add(new EmpDbPendList
                {
                    Id = emp.Id,
                    Code = emp.Code ?? "",
                    EmpFullName = fullName,
                    EmpFullNameAm = fullNameAm,
                    Gender = MyEnumHelper.FormatEnum<Gender>(emp.Gender),
                    Branch = deptNameAm ?? "",
                    Department = deptName ?? "",
                    DepartmentAm = deptNameAm ?? "",
                    Position = posName ?? "",
                    JobGrade = jgName ?? "",
                });

                dashboard.PendingEducationExperienceList.Add(new EmpDbPendEduExpList
                {
                    Id = emp.Id,
                    Code = emp.Code ?? "",
                    EmpFullName = fullName,
                    EmpFullNameAm = fullNameAm,
                    Gender = MyEnumHelper.FormatEnum<Gender>(emp.Gender),
                    Branch = deptNameAm ?? "",
                    Department = deptName ?? "",
                    DepartmentAm = deptNameAm ?? "",
                    Position = posName ?? "",
                    JobGrade = jgName ?? "",
                });
            }
        }

        // Set counts using enum names
        dashboard.ActiveEmployees = statusCounts.GetValueOrDefault(activeEnumName, 0);
        dashboard.PendingEmployeesCount = statusCounts.GetValueOrDefault(penEnumName, 0)
                                        + statusCounts.GetValueOrDefault("0", 0);
        dashboard.SuspendedEmployees = statusCounts.GetValueOrDefault(susEnumName, 0);
        dashboard.RetiredEmployees = statusCounts.GetValueOrDefault(retireEnumName, 0);
        dashboard.StandByEmployees = statusCounts.GetValueOrDefault(standByEnumName, 0);
        dashboard.TerminatedEmployees = statusCounts.GetValueOrDefault(termEnumName, 0);
        dashboard.LeaveEmployees = statusCounts.GetValueOrDefault(leaveEnumName, 0);
        dashboard.RejectedEmployees = statusCounts.GetValueOrDefault(rejEnumName, 0);

        // Additional metrics
        dashboard.TotalDepartments = refData.Departments.Count;
        dashboard.TotalPositions = refData.Positions.Count;
        dashboard.TotalJobGrades = refData.JobGrades.Count;
        dashboard.EmployeesByStatus = statusCounts;
        dashboard.GeneratedAt = DateTime.UtcNow;

        return dashboard;
    }
}

// ============================================================
// EMPLOYEE DATABASE REPORT HANDLER
// ============================================================
public class EmpDbRepHandler(IDapperHelper _dapper) : IRequestHandler<EmpDbRepQry, EmpDbReport>
{
    public async Task<EmpDbReport> Handle(EmpDbRepQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<Employee>(v, x => x.EmpState)
            .From<Employee>(v)
            .OrderBy<Employee>(v, x => x.DateAdd, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpStateList>(ct);
        if (list.Count <= 0) { return new EmpDbReport(); }

        // Get enum names
        var activeName = Enum.GetName(typeof(EmpState), EmpState.Active) ?? "Active";
        var penName = Enum.GetName(typeof(EmpState), EmpState.Pen) ?? "Pen";
        var susName = Enum.GetName(typeof(EmpState), EmpState.Sus) ?? "Sus";
        var retireName = Enum.GetName(typeof(EmpState), EmpState.Retire) ?? "Retire";
        var standByName = Enum.GetName(typeof(EmpState), EmpState.StandBy) ?? "StandBy";
        var termName = Enum.GetName(typeof(EmpState), EmpState.Term) ?? "Term";
        var leaveName = Enum.GetName(typeof(EmpState), EmpState.Leave) ?? "Leave";
        var rejName = Enum.GetName(typeof(EmpState), EmpState.Rej) ?? "Rej";

        return new EmpDbReport
        {
            EmpTot = list.Count,
            EmpAct = list.Count(e => e.EmpState == activeName),
            EmpPen = list.Count(e => e.EmpState == penName || e.EmpState == "0"),
            EmpSus = list.Count(e => e.EmpState == susName),
            EmpRet = list.Count(e => e.EmpState == retireName),
            EmpStd = list.Count(e => e.EmpState == standByName),
            EmpTer = list.Count(e => e.EmpState == termName),
            EmpLeave = list.Count(e => e.EmpState == leaveName),
            EmpRej = list.Count(e => e.EmpState == rejName),
        };
    }
}

// ============================================================
// PENDING EMPLOYEE LIST HANDLER
// ============================================================
public class EmpDbPendHandler(
    IDapperHelper _dapper,
    IMemoryCache _cache) : IRequestHandler<EmpDbPendQry, List<EmpDbPendList>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private const string RefCacheKey = "hr_reference_data_local";

    public async Task<List<EmpDbPendList>> Handle(EmpDbPendQry request, CancellationToken ct)
    {
        var refData = await GetLocalReferenceDataAsync(ct);

        var penEmp = Enum.GetName(typeof(EmpState), EmpState.Pen) ?? "Pen";
        const string e = "e";
        const string p = "p";

        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.JobGradeId, x => x.DepartmentId, x => x.PositionId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.EmpState == penEmp || x.EmpState == "0")
            .OrderBy<Employee>(e, x => x.DateAdd, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpDbPendJoin>(ct);

        if (list.Count <= 0) { return []; }

        var result = new List<EmpDbPendList>(list.Count);
        foreach (var data in list)
        {
            refData.Departments.TryGetValue(data.DepartmentId, out var deptName);
            refData.DepartmentAmharic.TryGetValue(data.DepartmentId, out var deptNameAm);
            refData.Positions.TryGetValue(data.PositionId, out var posName);
            refData.JobGrades.TryGetValue(data.JobGradeId, out var jgName);

            result.Add(new EmpDbPendList
            {
                Id = data.Id,
                Code = data.Code ?? "",
                EmpFullName = $"{data.FirstName} {data.MiddleName} {data.LastName}".Trim(),
                EmpFullNameAm = $"{data.FirstNameAm} {data.MiddleNameAm} {data.LastNameAm}".Trim(),
                Gender = MyEnumHelper.FormatEnum<Gender>(data.Gender),
                Branch = deptNameAm ?? "",
                Department = deptName ?? "",
                DepartmentAm = deptNameAm ?? "",
                Position = posName ?? "",
                JobGrade = jgName ?? "",
            });
        }

        return result;
    }

    private async Task<LocalReferenceData> GetLocalReferenceDataAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(RefCacheKey, out LocalReferenceData? cached) && cached is not null)
        {
            return cached;
        }

        var refData = new LocalReferenceData();

        const string deptSql = @"
            SELECT ""Id"", ""Name"", ""NameAm""
            FROM ""Departments""
            WHERE ""IsDeleted"" = false";

        var departments = await _dapper.QueryAsync<DepartmentLocalDto>(deptSql, null, ct);
        foreach (var dept in departments)
        {
            refData.Departments[dept.Id] = dept.Name;
            refData.DepartmentAmharic[dept.Id] = dept.NameAm;
        }

        const string posSql = @"
            SELECT ""Id"", ""Name""
            FROM ""Positions""
            WHERE ""IsDeleted"" = false";

        var positions = await _dapper.QueryAsync<PositionLocalDto>(posSql, null, ct);
        foreach (var pos in positions)
        {
            refData.Positions[pos.Id] = pos.Name;
        }

        const string jgSql = @"
            SELECT ""Id"", ""Name""
            FROM ""JobGrades""
            WHERE ""IsDeleted"" = false";

        var jobGrades = await _dapper.QueryAsync<JobGradeLocalDto>(jgSql, null, ct);
        foreach (var jg in jobGrades)
        {
            refData.JobGrades[jg.Id] = jg.Name;
        }

        _cache.Set(RefCacheKey, refData, CacheDuration);

        return refData;
    }
}

// ============================================================
// PENDING EDUCATION/EXPERIENCE HANDLER
// ============================================================
public class EmpDbPendEduExpHandler(
    IDapperHelper _dapper,
    IMemoryCache _cache) : IRequestHandler<EmpDbPendEduExpQry, List<EmpDbPendEduExpList>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private const string RefCacheKey = "hr_reference_data_local";

    public async Task<List<EmpDbPendEduExpList>> Handle(EmpDbPendEduExpQry request, CancellationToken ct)
    {
        var refData = await GetLocalReferenceDataAsync(ct);

        var penEmp = Enum.GetName(typeof(EmpState), EmpState.Pen) ?? "Pen";
        const string e = "e";
        const string p = "p";

        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.JobGradeId, x => x.DepartmentId, x => x.PositionId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.EmpState == penEmp || x.EmpState == "0")
            .OrderBy<Employee>(e, x => x.DateAdd, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpDbPendJoin>(ct);

        if (list.Count <= 0) { return []; }

        var result = new List<EmpDbPendEduExpList>(list.Count);
        foreach (var data in list)
        {
            refData.Departments.TryGetValue(data.DepartmentId, out var deptName);
            refData.DepartmentAmharic.TryGetValue(data.DepartmentId, out var deptNameAm);
            refData.Positions.TryGetValue(data.PositionId, out var posName);
            refData.JobGrades.TryGetValue(data.JobGradeId, out var jgName);

            result.Add(new EmpDbPendEduExpList
            {
                Id = data.Id,
                Code = data.Code ?? "",
                EmpFullName = $"{data.FirstName} {data.MiddleName} {data.LastName}".Trim(),
                EmpFullNameAm = $"{data.FirstNameAm} {data.MiddleNameAm} {data.LastNameAm}".Trim(),
                Gender = MyEnumHelper.FormatEnum<Gender>(data.Gender),
                Branch = deptNameAm ?? "",
                Department = deptName ?? "",
                DepartmentAm = deptNameAm ?? "",
                Position = posName ?? "",
                JobGrade = jgName ?? "",
            });
        }

        return result;
    }

    private async Task<LocalReferenceData> GetLocalReferenceDataAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(RefCacheKey, out LocalReferenceData? cached) && cached is not null)
        {
            return cached;
        }

        var refData = new LocalReferenceData();

        const string deptSql = @"
            SELECT ""Id"", ""Name"", ""NameAm""
            FROM ""Departments""
            WHERE ""IsDeleted"" = false";

        var departments = await _dapper.QueryAsync<DepartmentLocalDto>(deptSql, null, ct);
        foreach (var dept in departments)
        {
            refData.Departments[dept.Id] = dept.Name;
            refData.DepartmentAmharic[dept.Id] = dept.NameAm;
        }

        const string posSql = @"
            SELECT ""Id"", ""Name""
            FROM ""Positions""
            WHERE ""IsDeleted"" = false";

        var positions = await _dapper.QueryAsync<PositionLocalDto>(posSql, null, ct);
        foreach (var pos in positions)
        {
            refData.Positions[pos.Id] = pos.Name;
        }

        const string jgSql = @"
            SELECT ""Id"", ""Name""
            FROM ""JobGrades""
            WHERE ""IsDeleted"" = false";

        var jobGrades = await _dapper.QueryAsync<JobGradeLocalDto>(jgSql, null, ct);
        foreach (var jg in jobGrades)
        {
            refData.JobGrades[jg.Id] = jg.Name;
        }

        _cache.Set(RefCacheKey, refData, CacheDuration);

        return refData;
    }
}