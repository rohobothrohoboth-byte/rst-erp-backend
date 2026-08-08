// Controllers/PositionController.cs

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
public class PositionController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PositionController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get position by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPosition(Guid id)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ✅ Fixed: Removed "IsActive" column (doesn't exist in table)
        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""NoOfPosition"",
                ""IsVacant"",
                ""DepartmentId"",
                ""JobGradeId"",
                ""DateAdd"",
                ""DateMod"",
                ""IsDeleted"",
                ""SyncedAt""
            FROM ""Positions""
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var position = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });

        if (position == null)
        {
            return NotFound(ApiResponse<object>.Error($"Position not found for ID: {id}"));
        }

        return Ok(ApiResponse<object>.Ok(position, "Position retrieved successfully."));
    }

    /// <summary>
    /// Get all positions
    /// </summary>
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPositions()
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ✅ Fixed: Removed "IsActive" column
        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""NoOfPosition"",
                ""IsVacant"",
                ""DepartmentId"",
                ""JobGradeId"",
                ""DateAdd"",
                ""DateMod"",
                ""IsDeleted"",
                ""SyncedAt""
            FROM ""Positions""
            WHERE ""IsDeleted"" = false
            ORDER BY ""Name""";

        var positions = await connection.QueryAsync<dynamic>(sql);
        return Ok(ApiResponse<object>.Ok(positions, "Positions retrieved successfully."));
    }

    /// <summary>
    /// Get positions by department ID
    /// </summary>
    [HttpGet("ByDepartment/{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPositionsByDepartment(Guid departmentId)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // ✅ Fixed: Removed "IsActive" column
        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""NoOfPosition"",
                ""IsVacant"",
                ""DepartmentId"",
                ""JobGradeId"",
                ""DateAdd"",
                ""DateMod"",
                ""IsDeleted"",
                ""SyncedAt""
            FROM ""Positions""
            WHERE ""DepartmentId"" = @DepartmentId AND ""IsDeleted"" = false
            ORDER BY ""Name""";

        var positions = await connection.QueryAsync<dynamic>(sql, new { DepartmentId = departmentId });
        return Ok(ApiResponse<object>.Ok(positions, "Positions retrieved successfully."));
    }
}