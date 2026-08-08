// Controllers/AdminController.cs
using Cor.Finance.Seeding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize(Roles = "admin")]
public class AdminController : BaseApiController
{
    private readonly DbPerformanceTestSeeder _seeder;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        DbPerformanceTestSeeder seeder,
        ILogger<AdminController> logger)
    {
        _seeder = seeder;
        _logger = logger;
    }

    /// <summary>
    /// Seed test data for performance testing
    /// </summary>
    [HttpPost("SeedTestData")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SeedTestData([FromQuery] int count = 1000)
    {
        try
        {
            if (count > 10000)
                return BadRequest("Count cannot exceed 10,000");

            await _seeder.SeedDataAsync(count);
            return Ok(new { message = $"Successfully seeded {count} records per table" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding test data");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get database statistics
    /// </summary>
    [HttpGet("DbStats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDatabaseStats()
    {
        var stats = new
        {
            Companies = await _seeder.GetTableCountAsync("LocalCompanies"),
            Branches = await _seeder.GetTableCountAsync("LocalBranches"),
            Departments = await _seeder.GetTableCountAsync("LocalDepartments"),
            Employees = await _seeder.GetTableCountAsync("LocalEmployees"),
            Invoices = await _seeder.GetTableCountAsync("Invoices"),
            Payments = await _seeder.GetTableCountAsync("Payments"),
            Expenses = await _seeder.GetTableCountAsync("Expenses"),
            JournalEntries = await _seeder.GetTableCountAsync("JournalEntries"),
            JournalLines = await _seeder.GetTableCountAsync("JournalLines"),
            Budgets = await _seeder.GetTableCountAsync("Budgets"),
            BudgetLines = await _seeder.GetTableCountAsync("BudgetLines"),
            ChartOfAccounts = await _seeder.GetTableCountAsync("ChartOfAccounts"),
            FinancialPeriods = await _seeder.GetTableCountAsync("FinancialPeriods")
        };

        return Ok(stats);
    }
}