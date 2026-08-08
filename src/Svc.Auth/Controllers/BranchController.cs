// Controllers/BranchController.cs

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
public class BranchController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public BranchController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get branch by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch(Guid id)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""Code"",
                ""Location"",
                ""CompId"",
                ""IsActive"",
                ""DateAdd"",
                ""DateMod""
            FROM ""Branches""
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var branch = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });

        if (branch == null)
        {
            return NotFound(ApiResponse<object>.Error($"Branch not found for ID: {id}"));
        }

        return Ok(ApiResponse<object>.Ok(branch, "Branch retrieved successfully."));
    }

    /// <summary>
    /// Get all branches
    /// </summary>
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBranches()
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                ""Id""::text as Id,
                ""Name"",
                ""NameAm"",
                ""Code"",
                ""Location"",
                ""CompId"",
                ""IsActive"",
                ""DateAdd"",
                ""DateMod""
            FROM ""Branches""
            WHERE ""IsDeleted"" = false
            ORDER BY ""Name""";

        var branches = await connection.QueryAsync<dynamic>(sql);
        return Ok(ApiResponse<object>.Ok(branches, "Branches retrieved successfully."));
    }
}