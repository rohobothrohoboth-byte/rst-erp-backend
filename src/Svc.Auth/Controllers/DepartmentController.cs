// Controllers/DepartmentController.cs

using Asp.Versioning;
using Dapper;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/auth/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DepartmentController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get department by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDepartment(Guid id)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ✅ Fixed: Removed "Code" column (doesn't exist in table)
        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""DeptStat"",
                ""BranchId"",
                ""DateAdd"",
                ""DateMod"",
                ""IsDeleted"",
                ""SyncedAt""
            FROM ""Departments""
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var department = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });

        if (department == null)
        {
            return NotFound(ApiResponse<object>.Error($"Department not found for ID: {id}"));
        }

        return Ok(ApiResponse<object>.Ok(department, "Department retrieved successfully."));
    }

    /// <summary>
    /// Get all departments
    /// </summary>
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDepartments()
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ✅ Fixed: Removed "Code" column
        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""DeptStat"",
                ""BranchId"",
                ""DateAdd"",
                ""DateMod"",
                ""IsDeleted"",
                ""SyncedAt""
            FROM ""Departments""
            WHERE ""IsDeleted"" = false
            ORDER BY ""Name""";

        var departments = await connection.QueryAsync<dynamic>(sql);
        return Ok(ApiResponse<object>.Ok(departments, "Departments retrieved successfully."));
    }

    /// <summary>
    /// Get departments by branch ID
    /// </summary>
    [HttpGet("ByBranch/{branchId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartmentsByBranch(Guid branchId)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ✅ Fixed: Removed "Code" column
        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""DeptStat"",
                ""BranchId"",
                ""DateAdd"",
                ""DateMod"",
                ""IsDeleted"",
                ""SyncedAt""
            FROM ""Departments""
            WHERE ""BranchId"" = @BranchId AND ""IsDeleted"" = false
            ORDER BY ""Name""";

        var departments = await connection.QueryAsync<dynamic>(sql, new { BranchId = branchId });
        return Ok(ApiResponse<object>.Ok(departments, "Departments retrieved successfully."));
    }
}