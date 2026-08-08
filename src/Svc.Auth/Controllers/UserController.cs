using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Interfaces;
using Dapper;
using System.Security.Claims;
using Svc.Auth.Queries;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
namespace Svc.Auth.Controllers;

/// <summary>
/// USER management end points
/// </summary>
[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/User")]
[ApiVersion("1.0")]
public class UserController : ControllerBase
{
    private readonly IMediator _med;
    private readonly IDapperHelper _dapper;
    private readonly IConfiguration _configuration;
 private readonly ILogger<UserController> _logger;
    public UserController(IMediator med, IDapperHelper dapper, IConfiguration configuration, ILogger<UserController> logger)
    {
        _med = med;
        _dapper = dapper;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("AllAppUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAppUsers()
    {
        var res = await _med.Send(new GetAllAppUsersQry());
        return Ok(ApiResponse<object>.Ok(res, "App users retrieved successfully."));
    }

    [HttpDelete("DelUserAcct/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var res = await _med.Send(new UserDelCmd { Id = id });
       if (res == null || !res.IsSuccess) { throw new DomainException("UNABLE to DELETE selected User's Account."); }
        return Ok(ApiResponse<object>.Ok(res, "Selected USER'S ACCOUNT successfully DELETED."));
    }

    [HttpPost("ReactivateAccount/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReactivateAccount(string userId)
    {
        var res = await _med.Send(new ReactivateAccountCmd { UserId = userId });
        if (res == null || !res.IsSuccess) throw new DomainException("Failed to reactivate account");
        return Ok(ApiResponse<object>.Ok(res, "Account reactivated successfully."));
    }

    [HttpPost("ResetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var res = await _med.Send(new ResetPasswordCmd { UserId = dto.UserId, NewPassword = dto.NewPassword });
       if (res == null || !res.IsSuccess) throw new DomainException("Failed to reset password");
        return Ok(ApiResponse<object>.Ok(res, "Password reset successfully."));
    }

    [HttpPut("ChangePwd")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePwd([FromBody] PwdChgDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var userId = User.FindFirstValue(AuthCons.UserId);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access.");

        dto.Id = userId;
        var res = await _med.Send(new PwdChangeCmd { Dto = dto });
       if (res == null || !res.IsSuccess) { throw new DomainException("UNABLE to CHANGE User's Password."); }
        return Ok(ApiResponse<string>.Ok(null!, "User's Password successfully CHANGED."));
    }

    // ? Updated to use LOCAL COPY from Auth database
    [HttpGet("account/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccountByEmployeeId(string employeeId)
    {
        try
        {
            if (!Guid.TryParse(employeeId, out var employeeGuid))
            {
                return Ok(ApiResponse<object>.Ok(new
                {
                    hasAccount = false,
                    isActive = false,
                    userId = (string?)null,
                    error = "Invalid employee ID format"
                }));
            }

            var connectionString = _configuration.GetConnectionString("authMgrCon");
            using var connection = new NpgsqlConnection(connectionString);

            // ? Check if employee exists in LOCAL COPY
            const string empSql = @"
                SELECT ""Id"", ""FirstName"", ""LastName"", ""Email""
                FROM ""Employees""
                WHERE ""Id"" = @EmployeeId
                AND ""IsDeleted"" = false";

            var employee = await connection.QueryFirstOrDefaultAsync<dynamic>(
                empSql,
                new { EmployeeId = employeeGuid });

            if (employee == null)
            {
                return Ok(ApiResponse<object>.Ok(new
                {
                    hasAccount = false,
                    isActive = false,
                    userId = (string?)null,
                    employeeExists = false
                }));
            }

            // ? Check if user exists for this employee
            const string userSql = @"
                SELECT ""Id"", ""IsActive""
                FROM ""AppUser""
                WHERE ""EmployeeId"" = @EmployeeId";

            var appUser = await connection.QueryFirstOrDefaultAsync<dynamic>(
                userSql,
                new { EmployeeId = employeeGuid });

            return Ok(ApiResponse<object>.Ok(new
            {
                hasAccount = appUser != null,
                userId = appUser?.Id?.ToString(),
                employeeId = employeeGuid.ToString(),
                employeeName = $"{employee.FirstName} {employee.LastName}",
                employeeEmail = employee.Email,
                isActive = appUser?.IsActive == true
            }));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                hasAccount = false,
                isActive = false,
                error = ex.Message
            }));
        }
    }

    // ? Updated to use LOCAL COPY
    [HttpGet("GetAppUserByEmployeeId/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppUserByEmployeeId(string employeeId)
    {
        try
        {
            if (!Guid.TryParse(employeeId, out var employeeGuid))
            {
                return Ok(ApiResponse<object>.Ok(null));
            }

            var connectionString = _configuration.GetConnectionString("authMgrCon");
            using var connection = new NpgsqlConnection(connectionString);

            const string sql = @"
                SELECT
                    u.""Id"",
                    u.""EmployeeId"",
                    u.""IsActive"",
                    u.""UserName"",
                    u.""Email"",
                    e.""FirstName"",
                    e.""LastName""
                FROM ""AppUser"" u
                LEFT JOIN ""Employees"" e ON u.""EmployeeId"" = e.""Id""
                WHERE u.""EmployeeId"" = @EmployeeId";

            var appUser = await connection.QueryFirstOrDefaultAsync<dynamic>(
                sql,
                new { EmployeeId = employeeGuid });

            if (appUser == null)
            {
                return Ok(ApiResponse<object>.Ok(null));
            }

            return Ok(ApiResponse<object>.Ok(new
            {
                Id = appUser.Id?.ToString(),
                EmployeeId = appUser.EmployeeId?.ToString(),
                UserName = appUser.UserName,
                Email = appUser.Email,
                IsActive = appUser.IsActive,
                EmployeeName = $"{appUser.FirstName} {appUser.LastName}"
            }));
        }
        catch (Exception )
        {
            return Ok(ApiResponse<object>.Ok(null));
        }
    }

    [HttpGet("ByBranch/{branchId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersByBranch(Guid branchId)
    {
        var res = await _med.Send(new GetUsersByBranchQry { BranchId = branchId });
        return Ok(ApiResponse<object>.Ok(res, "Users retrieved successfully."));
    }

    [HttpGet("ByDepartment/{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersByDepartment(Guid departmentId)
    {
        var res = await _med.Send(new GetUsersByDepartmentQry { DepartmentId = departmentId });
        return Ok(ApiResponse<object>.Ok(res, "Users retrieved successfully."));
    }

    [HttpGet("WithOrg/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserWithOrg(string userId)
    {
        var user = await _med.Send(new GetUserWithOrgQry { UserId = userId });
        if (user == null)
            throw new DomainException($"User with ID '{userId}' not found");
        return Ok(ApiResponse<object>.Ok(user, "User retrieved successfully."));
    }

    [HttpPut("UpdateBranch/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserBranch(string userId, [FromBody] UpdateBranchDto dto)
    {
        var result = await _med.Send(new UpdateUserBranchCmd { UserId = userId, BranchId = dto.BranchId });
        if (!result.IsSuccess)
            throw new DomainException("Failed to update Branches");
        return Ok(ApiResponse<object>.Ok(null, "User Branches updated successfully."));
    }

    [HttpPut("UpdateDepartment/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserDepartment(string userId, [FromBody] UpdateDepartmentDto dto)
    {
        var result = await _med.Send(new UpdateUserDepartmentCmd { UserId = userId, DepartmentId = dto.DepartmentId });
        if (!result.IsSuccess)
            throw new DomainException("Failed to update department");
        return Ok(ApiResponse<object>.Ok(null, "User department updated successfully."));
    }

    [HttpPut("UpdatePosition/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserPosition(string userId, [FromBody] UpdatePositionDto dto)
    {
        var result = await _med.Send(new UpdateUserPositionCmd { UserId = userId, PositionId = dto.PositionId });
        if (!result.IsSuccess)
            throw new DomainException("Failed to update position");
        return Ok(ApiResponse<object>.Ok(null, "User position updated successfully."));
    }

    // UserController.cs - Add this new endpoint

    /// <summary>
    /// Get user by ID
    /// </summary>
 [HttpGet("{id}")]
 [ProducesResponseType(StatusCodes.Status200OK)]
 [ProducesResponseType(StatusCodes.Status400BadRequest)]
 [ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetUserById(string id)
{
    try
    {
        if (!Guid.TryParse(id, out var userId))
            return BadRequest(ApiResponse<object>.Fail($"Invalid user ID format: {id}"));

        using var connection = new NpgsqlConnection(_configuration.GetConnectionString("authMgrCon"));

        const string sql = @"
            SELECT
                u.""Id""::text as Id,
                u.""UserName"",
                 u.""IsActive"",
                 u.""EmployeeId"",
                 e.""FirstName"",
                 e.""MiddleName"",
                 e.""LastName"",
                 e.""FirstNameAm"",
                 e.""MiddleNameAm"",
                 e.""LastNameAm"",
                 e.""Phone"",
                 e.""Gender"",
                 e.""PositionId"",
                 e.""DepartmentId"",
                 p.""Name"" as PositionName,
                 d.""Name"" as DepartmentName,
                 b.""Name"" as BranchName,
                 b.""Id"" as BranchId
             FROM ""AppUser"" u
             LEFT JOIN ""Employees"" e ON u.""EmployeeId"" = e.""Id"" AND e.""IsDeleted"" = false
             LEFT JOIN ""Positions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
             LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
             LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            WHERE u.""Id"" = @UserId::text";           // ? Fixed here

                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", userId.ToString(), DbType.String);   // ? Better: send as string

                    var user = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, parameters);

                    if (user == null)
                        return NotFound(ApiResponse<object>.Fail($"User with ID '{id}' not found"));

                    return Ok(ApiResponse<object>.Ok(user, "User retrieved successfully."));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                    return StatusCode(500, ApiResponse<object>.Fail($"An error occurred: {ex.Message}"));
                }
            }
   }