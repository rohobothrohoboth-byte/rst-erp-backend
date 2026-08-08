// Controllers/IFRSReportController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class IFRSReportController : BaseApiController
{
    private readonly IMediator _mediator;

    public IFRSReportController(IMediator mediator, ILogger<IFRSReportController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    // ============================================================
    // IFRS REPORTS
    // ============================================================

    [HttpGet]
    [ProducesResponseType(typeof(List<IFRSReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? standard,
        [FromQuery] string? status,
        [FromQuery] string? period)
    {
        try
        {
            var result = await _mediator.Send(new GetAllIFRSReportsQry
            {
                Standard = standard,
                Status = status,
                Period = period
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllIFRSReports");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IFRSReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetIFRSReportByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetIFRSReportById", id);
        }
    }

    [HttpPost("Generate")]
    [ProducesResponseType(typeof(IFRSReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateIFRSReportDto dto)
    {
        try
        {
            // Validate standard
            var validStandards = new[] { "IFRS 9", "IFRS 15", "IFRS 16", "IFRS 7", "IFRS 8" };
            if (!validStandards.Contains(dto.Standard))
                return BadRequest(new { success = false, message = $"Invalid standard. Must be one of: {string.Join(", ", validStandards)}" });

            var result = await _mediator.Send(new GenerateIFRSReportCmd { GenerateDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = $"{dto.Standard} report generated successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GenerateIFRSReport");
        }
    }

    [HttpPost("Schedule")]
    [ProducesResponseType(typeof(IFRSReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScheduleReport([FromBody] ScheduleIFRSReportDto dto)
    {
        try
        {
            var validStandards = new[] { "IFRS 9", "IFRS 15", "IFRS 16", "IFRS 7", "IFRS 8" };
            if (!validStandards.Contains(dto.Standard))
                return BadRequest(new { success = false, message = $"Invalid standard. Must be one of: {string.Join(", ", validStandards)}" });

            var result = await _mediator.Send(new ScheduleIFRSReportCmd { ScheduleDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = $"{dto.Standard} report scheduled successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ScheduleIFRSReport");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteIFRSReportCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"IFRS Report with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteIFRSReport", id);
        }
    }

    // ============================================================
    // UTILITY ENDPOINTS
    // ============================================================

    [HttpGet("Standards")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableStandards()
    {
        try
        {
            var standards = new List<string> { "IFRS 9", "IFRS 15", "IFRS 16", "IFRS 7", "IFRS 8" };
            return Ok(new { success = true, data = standards });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAvailableStandards");
        }
    }

    [HttpGet("Stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var reports = await _mediator.Send(new GetAllIFRSReportsQry());

            var stats = new
            {
                TotalReports = reports.Count,
                ByStandard = reports.GroupBy(r => r.Standard)
                    .Select(g => new { Standard = g.Key, Count = g.Count() }),
                ByStatus = reports.GroupBy(r => r.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() }),
                GeneratedCount = reports.Count(r => r.Status == "Generated"),
                ScheduledCount = reports.Count(r => r.Status == "Scheduled"),
                InProgressCount = reports.Count(r => r.Status == "InProgress")
            };

            return Ok(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetIFRSStats");
        }
    }
}