using Asp.Versioning;
using Common;
using Dapper;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Svc.Auth.Queries;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Services;

namespace Svc.Auth.Controllers;

/// <summary>
/// Employees Management by ADMIN end points
/// </summary>
[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/AdminEmp")]
[ApiVersion("1.0")]
public class EmpController : ControllerBase
{
    private readonly IMediator _med;
    private readonly IConfiguration _configuration;
    private readonly IHrmProApiService _hrmPro;

    public EmpController(IMediator med, IConfiguration configuration, IHrmProApiService hrmPro)
    {
        _med = med;
        _configuration = configuration;
        _hrmPro = hrmPro;
    }

    // When an employee is not present in the Auth local-copy table (e.g. it hasn't
    // been synced yet), fall back to the HRM Profile service as the source of truth.
    private async Task<IActionResult> EmployeeFromProfileOrNotFound(Guid id)
    {
        try
        {
            var fromProfile = await _hrmPro.GetEmployeeAsync(id);
            if (fromProfile != null)
                return Ok(ApiResponse<object>.Ok(fromProfile, "Employee retrieved from HRM Profile."));
        }
        catch
        {
            // fall through to 404 below
        }
        return NotFound(ApiResponse<object>.Error($"Employee not found for ID: {id}"));
    }

    [PerAuth("hr.emp.view|hr.emp.list.view|core.users.view|hr.db.view")]
    [HttpGet("AllEmployee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEmployee()
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                e.""Id""::text as Id,
                e.""Code"",
                e.""FirstName"",
                e.""MiddleName"",
                e.""LastName"",
                e.""FirstNameAm"",
                e.""MiddleNameAm"",
                e.""LastNameAm"",
                e.""Email"",
                e.""Phone"",
                e.""Gender"",
                e.""EmpState"",
                e.""EmploymentType"",
                e.""EmploymentNature"",
                e.""WorkArrangement"",
                e.""PositionId"",
                e.""DepartmentId"",
                e.""JobGradeId"",
                e.""AppUserId"",
                p.""Name"" as PositionName,
                d.""Name"" as DepartmentName,
                b.""Name"" as BranchName,
                j.""Name"" as JobGradeName,
                CASE WHEN u.""Id"" IS NOT NULL THEN true ELSE false END as HasAccount,
                CASE WHEN u.""Id"" IS NOT NULL AND u.""IsActive"" = true THEN true ELSE false END as IsAccountActive
            FROM ""Employees"" e
            LEFT JOIN ""Positions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""JobGrades"" j ON e.""JobGradeId"" = j.""Id"" AND j.""IsDeleted"" = false
            LEFT JOIN ""AppUser"" u ON e.""Id"" = u.""EmployeeId""
            WHERE e.""IsDeleted"" = false
            ORDER BY e.""FirstName"", e.""LastName""";

        var employees = await connection.QueryAsync<AdminEmpListDto>(sql);
        return Ok(ApiResponse<object>.Ok(employees, "Employees retrieved successfully."));
    }

    [PerAuth("hr.emp.view|hr.emp.list.view|core.users.view|hr.db.view")]
    [HttpGet("ByDepartment/{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeesByDepartment(Guid departmentId)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                e.""Id""::text as Id,
                e.""Code"",
                e.""FirstName"",
                e.""MiddleName"",
                e.""LastName"",
                e.""FirstNameAm"",
                e.""MiddleNameAm"",
                e.""LastNameAm"",
                e.""Email"",
                e.""Phone"",
                e.""Gender"",
                e.""EmpState"",
                e.""EmploymentType"",
                e.""EmploymentNature"",
                e.""WorkArrangement"",
                e.""PositionId"",
                e.""DepartmentId"",
                e.""JobGradeId"",
                e.""AppUserId"",
                p.""Name"" as PositionName,
                d.""Name"" as DepartmentName,
                b.""Name"" as BranchName,
                j.""Name"" as JobGradeName,
                CASE WHEN u.""Id"" IS NOT NULL THEN true ELSE false END as HasAccount,
                CASE WHEN u.""Id"" IS NOT NULL AND u.""IsActive"" = true THEN true ELSE false END as IsAccountActive
            FROM ""Employees"" e
            LEFT JOIN ""Positions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""JobGrades"" j ON e.""JobGradeId"" = j.""Id"" AND j.""IsDeleted"" = false
            LEFT JOIN ""AppUser"" u ON e.""Id"" = u.""EmployeeId""
            WHERE e.""DepartmentId"" = @DepartmentId
            AND e.""IsDeleted"" = false
            ORDER BY e.""FirstName"", e.""LastName""";

        var employees = await connection.QueryAsync<AdminEmpListDto>(sql, new { DepartmentId = departmentId });
        return Ok(ApiResponse<object>.Ok(employees, "Employees retrieved successfully."));
    }

    [PerAuth("hr.emp.view|hr.emp.list.view|core.users.view|hr.db.view")]
    [HttpGet("ByPosition/{positionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeesByPosition(Guid positionId)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                e.""Id""::text as Id,
                e.""Code"",
                e.""FirstName"",
                e.""MiddleName"",
                e.""LastName"",
                e.""FirstNameAm"",
                e.""MiddleNameAm"",
                e.""LastNameAm"",
                e.""Email"",
                e.""Phone"",
                e.""Gender"",
                e.""EmpState"",
                e.""EmploymentType"",
                e.""EmploymentNature"",
                e.""WorkArrangement"",
                e.""PositionId"",
                e.""DepartmentId"",
                e.""JobGradeId"",
                e.""AppUserId"",
                p.""Name"" as PositionName,
                d.""Name"" as DepartmentName,
                b.""Name"" as BranchName,
                j.""Name"" as JobGradeName,
                CASE WHEN u.""Id"" IS NOT NULL THEN true ELSE false END as HasAccount,
                CASE WHEN u.""Id"" IS NOT NULL AND u.""IsActive"" = true THEN true ELSE false END as IsAccountActive
            FROM ""Employees"" e
            LEFT JOIN ""Positions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""JobGrades"" j ON e.""JobGradeId"" = j.""Id"" AND j.""IsDeleted"" = false
            LEFT JOIN ""AppUser"" u ON e.""Id"" = u.""EmployeeId""
            WHERE e.""PositionId"" = @PositionId
            AND e.""IsDeleted"" = false
            ORDER BY e.""FirstName"", e.""LastName""";

        var employees = await connection.QueryAsync<AdminEmpListDto>(sql, new { PositionId = positionId });
        return Ok(ApiResponse<object>.Ok(employees, "Employees retrieved successfully."));
    }

    /// <summary>
    /// Get employee by ID - supports both Employee ID and AppUser ID
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployee(Guid id)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ? FIX: Cast AppUserId to text for comparison with UUID
        const string sql = @"
            SELECT
                e.""Id""::text as Id,
                e.""Code"",
                e.""FirstName"",
                e.""FirstNameAm"",
                e.""MiddleName"",
                e.""MiddleNameAm"",
                e.""LastName"",
                e.""LastNameAm"",
                e.""Email"",
                e.""Phone"",
                e.""Gender"",
                e.""Nationality"",
                e.""EmploymentType"",
                e.""EmploymentNature"",
                e.""WorkArrangement"",
                e.""EmpState"",
                e.""EmploymentDate"",
                e.""PositionId"",
                e.""DepartmentId"",
                e.""JobGradeId"",
                e.""AppUserId"",
                p.""Name"" as PositionName,
                d.""Name"" as DepartmentName,
                b.""Name"" as BranchName,
                b.""Id"" as BranchId,
                j.""Name"" as JobGradeName,
                CASE WHEN u.""Id"" IS NOT NULL THEN true ELSE false END as HasAccount,
                CASE WHEN u.""Id"" IS NOT NULL AND u.""IsActive"" = true THEN true ELSE false END as IsAccountActive
            FROM ""Employees"" e
            LEFT JOIN ""Positions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""JobGrades"" j ON e.""JobGradeId"" = j.""Id"" AND j.""IsDeleted"" = false
            LEFT JOIN ""AppUser"" u ON e.""Id"" = u.""EmployeeId""
            WHERE (e.""Id"" = @Id OR e.""AppUserId""::text = @Id::text)
            AND e.""IsDeleted"" = false";

        var employee = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });

        if (employee == null)
        {
            return await EmployeeFromProfileOrNotFound(id);
        }

        return Ok(ApiResponse<object>.Ok(employee, "Employee retrieved successfully."));
    }

    /// <summary>
    /// Get employee by ID (for any authenticated user)
    /// </summary>
    [Authorize]
    [HttpGet("~/api/auth/v{version:apiVersion}/Employee/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeById(Guid id)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ? FIX: Use a single query with proper casting
        const string sql = @"
            SELECT
                e.""Id""::text as Id,
                e.""Code"",
                e.""FirstName"",
                e.""FirstNameAm"",
                e.""MiddleName"",
                e.""MiddleNameAm"",
                e.""LastName"",
                e.""LastNameAm"",
                e.""Email"",
                e.""Phone"",
                e.""Gender"",
                e.""Nationality"",
                e.""EmploymentType"",
                e.""EmploymentNature"",
                e.""WorkArrangement"",
                e.""EmpState"",
                e.""EmploymentDate"",
                e.""PositionId"",
                e.""DepartmentId"",
                e.""JobGradeId"",
                e.""AppUserId"",
                p.""Name"" as PositionName,
                d.""Name"" as DepartmentName,
                b.""Name"" as BranchName,
                b.""Id"" as BranchId,
                j.""Name"" as JobGradeName
            FROM ""Employees"" e
            LEFT JOIN ""Positions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""JobGrades"" j ON e.""JobGradeId"" = j.""Id"" AND j.""IsDeleted"" = false
            WHERE (e.""Id"" = @Id OR e.""AppUserId""::text = @Id::text)
            AND e.""IsDeleted"" = false";

        var employee = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });

        if (employee == null)
        {
            return await EmployeeFromProfileOrNotFound(id);
        }

        return Ok(ApiResponse<object>.Ok(employee, "Employee retrieved successfully."));
    }
}