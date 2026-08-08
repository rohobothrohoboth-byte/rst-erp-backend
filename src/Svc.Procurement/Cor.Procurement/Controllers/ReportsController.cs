using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Procurement.Commands;
using Cor.Procurement.Queries;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Controllers;

[ApiController]
[Route("api/procurement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ReportsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ReportsDashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] string? period = null,
        [FromQuery] int topVendorsCount = 5,
        [FromQuery] int recentReportsCount = 10)
    {
        try
        {
            var query = new GetReportsDashboardQuery
            {
                Period = period,
                TopVendorsCount = topVendorsCount,
                RecentReportsCount = recentReportsCount
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetDashboard));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetReportByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Report with ID '{id}' not found" });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    [HttpGet("download/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(Guid id)
    {
        try
        {
            var query = new DownloadReportQuery { Id = id };
            var (fileData, fileName, contentType) = await _mediator.Send(query);

            return File(fileData, contentType, fileName);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Report with ID '{id}' not found" });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Download));
        }
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(GenerateReportResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] CreateReportDto createDto)
    {
        try
        {
            var command = new GenerateReportCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Generate));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteReportCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Report with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    [HttpGet("spend-analysis")]
    [ProducesResponseType(typeof(SpendAnalysisDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpendAnalysis([FromQuery] string? period = null)
    {
        try
        {
            var query = new GetReportsDashboardQuery { Period = period };
            var result = await _mediator.Send(query);
            return Ok(result.SpendAnalysis);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSpendAnalysis));
        }
    }

    [HttpGet("vendor-performance")]
    [ProducesResponseType(typeof(List<VendorPerformanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVendorPerformance([FromQuery] int count = 5)
    {
        try
        {
            var query = new GetReportsDashboardQuery { TopVendorsCount = count };
            var result = await _mediator.Send(query);
            return Ok(result.VendorPerformance);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetVendorPerformance));
        }
    }
}