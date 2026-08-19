using Asp.Versioning;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.Finance.Controllers;

/// <summary>
/// Internal budget availability / encumbrance endpoints used by other services
/// (e.g. Recruitment reserves budget when a workforce plan is approved).
/// </summary>
[ApiController]
[Route("api/finance/v{version:apiVersion}/BudgetCheck")]
[ApiVersion("1.0")]
[AllowAnonymous]
public class BudgetCheckController : ControllerBase
{
    private readonly IBudgetReservationService _service;
    public BudgetCheckController(IBudgetReservationService service) { _service = service; }

    [HttpGet("Availability/{budgetId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Availability(Guid budgetId, [FromQuery] decimal amount = 0)
    {
        var res = await _service.CheckAvailabilityAsync(budgetId, amount);
        return Ok(new { data = res });
    }

    [HttpPost("Reserve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reserve([FromBody] BudgetReserveRequest req)
    {
        var res = await _service.ReserveAsync(req);
        return Ok(new { data = res });
    }

    [HttpPost("Release")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Release([FromBody] BudgetReleaseRequest req)
    {
        await _service.ReleaseAsync(req);
        return Ok(new { message = "Reservation released." });
    }

    [HttpPost("Consume")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Consume([FromBody] BudgetConsumeRequest req)
    {
        await _service.ConsumeAsync(req);
        return Ok(new { message = "Reservation consumed." });
    }
}
