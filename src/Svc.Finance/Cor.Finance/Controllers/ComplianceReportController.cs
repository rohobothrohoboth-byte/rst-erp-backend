// Controllers/ComplianceReportController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/compliance/reports")]
[ApiVersion("1.0")]
[Authorize]
public class ComplianceReportController : BaseApiController
{
    private readonly IMediator _mediator;

    public ComplianceReportController(IMediator mediator, ILogger<ComplianceReportController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ComplianceReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? type,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var result = await _mediator.Send(new GetAllComplianceReportsQry
            {
                Status = status,
                Type = type,
                FromDate = fromDate,
                ToDate = toDate
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllComplianceReports");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ComplianceReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetComplianceReportByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetComplianceReportById", id);
        }
    }

    [HttpPost("Generate")]
    [ProducesResponseType(typeof(ComplianceReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] GenerateComplianceReportDto dto)
    {
        try
        {
            var result = await _mediator.Send(new GenerateComplianceReportCmd { GenerateDto = dto });

            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to generate report" });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Compliance report generated successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GenerateComplianceReport");
        }
    }

    [HttpGet("{id}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Download(Guid id, [FromQuery] string format = "pdf")
    {
        try
        {
            var result = await _mediator.Send(new DownloadComplianceReportQry
            {
                Id = id,
                Format = format
            });

            if (result == null)
            {
                return NotFound(new { success = false, message = $"Compliance report with ID '{id}' not found" });
            }

            // Return file based on format
            return format.ToLower() switch
            {
                "pdf" => File(result.FileContent, "application/pdf", $"{result.FileName}.pdf"),
                "excel" or "xlsx" => File(result.FileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{result.FileName}.xlsx"),
                "csv" => File(result.FileContent, "text/csv", $"{result.FileName}.csv"),
                _ => File(result.FileContent, "application/pdf", $"{result.FileName}.pdf")
            };
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DownloadComplianceReport", id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteComplianceReportCmd { Id = id });

            if (result is bool success && !success)
                return NotFound(new { message = $"Compliance report with ID '{id}' not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteComplianceReport", id);
        }
    }
}