// Controllers/ConsolidationReportController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/consolidation-reports")]
[ApiVersion("1.0")]
[Authorize]
public class ConsolidationReportController : BaseApiController
{
    private readonly IMediator _mediator;

    public ConsolidationReportController(IMediator mediator, ILogger<ConsolidationReportController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ConsolidationReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? groupId,
        [FromQuery] string? period,
        [FromQuery] string? status)
    {
        try
        {
            var result = await _mediator.Send(new GetAllConsolidationReportsQry
            {
                GroupId = groupId,
                Period = period,
                Status = status
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllConsolidationReports");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ConsolidationReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetConsolidationReportByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetConsolidationReportById", id);
        }
    }

    [HttpPost("Generate")]
    [ProducesResponseType(typeof(ConsolidationReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] GenerateConsolidationReportDto dto)
    {
        try
        {
            var result = await _mediator.Send(new GenerateConsolidationReportCmd { GenerateDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Consolidation report generated successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GenerateConsolidationReport");
        }
    }

    // ✅ FIXED: Download endpoint with proper handling
 // Backend - Add proper content-disposition header

 // ✅ FIXED: Download endpoint with proper handling
 [HttpGet("{id}/download")]
 [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
 [ProducesResponseType(StatusCodes.Status404NotFound)]
 [ProducesResponseType(StatusCodes.Status400BadRequest)]
 public async Task<IActionResult> Download(Guid id, [FromQuery] string format = "pdf")
 {
     try
     {
         var result = await _mediator.Send(new DownloadConsolidationReportQry
         {
             Id = id,
             Format = format
         });

         if (result == null || result.FileContent.Length == 0)
         {
             return NotFound(new { success = false, message = $"Consolidation report with ID '{id}' not found or empty" });
         }

         // ✅ Determine content type and filename
         var (contentType, fileExtension) = format.ToLower() switch
         {
             "pdf" => ("application/pdf", "pdf"),
             "excel" or "xlsx" => ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"),
             "csv" => ("text/csv", "csv"),
             "html" => ("text/html", "html"),
             "json" => ("application/json", "json"),
             _ => ("application/octet-stream", "bin")
         };

         var fileName = $"{result.FileName}.{fileExtension}";

         Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
         Response.Headers.Append("Content-Type", contentType);
         Response.Headers.Append("Content-Length", result.FileContent.Length.ToString());

         return File(result.FileContent, contentType, fileName);
     }
     catch (Exception ex)
     {
         return HandleException(ex, "DownloadConsolidationReport", id);
     }
 }

    // ✅ ADD THIS: Download Compliance Report
    [HttpGet("{id}/download-compliance")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DownloadCompliance(Guid id, [FromQuery] string format = "pdf")
    {
        try
        {
            var result = await _mediator.Send(new DownloadComplianceReportQry
            {
                Id = id,
                Format = format
            });

            if (result == null || result.FileContent.Length == 0)
            {
                return NotFound(new { success = false, message = $"Compliance report with ID '{id}' not found or empty" });
            }

            return format.ToLower() switch
            {
                "pdf" => File(result.FileContent, "application/pdf", $"{result.FileName}.pdf"),
                "excel" or "xlsx" => File(result.FileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{result.FileName}.xlsx"),
                "csv" => File(result.FileContent, "text/csv", $"{result.FileName}.csv"),
                "html" => File(result.FileContent, "text/html", $"{result.FileName}.html"),
                "json" => File(result.FileContent, "application/json", $"{result.FileName}.json"),
                _ => File(result.FileContent, "application/octet-stream", $"{result.FileName}.bin")
            };
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DownloadComplianceReport", id);
        }
    }

    // ✅ ADD THIS: Bulk Export
    [HttpPost("export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Export([FromBody] ExportReportsRequestDto request)
    {
        try
        {
            if (request == null || request.ReportIds == null || !request.ReportIds.Any())
                return BadRequest(new { message = "No report IDs provided" });

            var exportData = new List<ConsolidationReportDto>();
            foreach (var id in request.ReportIds)
            {
                try
                {
                    var report = await _mediator.Send(new GetConsolidationReportByIdQry { Id = id });
                    if (report != null)
                        exportData.Add(report);
                }
                catch { /* Skip individual errors */ }
            }

            if (!exportData.Any())
                return NotFound(new { message = "No valid reports found" });

            var jsonData = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var content = System.Text.Encoding.UTF8.GetBytes(jsonData);
            var format = request.Format?.ToLower() ?? "json";
            var contentType = format switch
            {
                "pdf" => "application/pdf",
                "excel" or "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                "html" => "text/html",
                "json" => "application/json",
                _ => "application/json"
            };

            var fileName = $"Consolidation_Reports_Export_{DateTime.Now:yyyyMMddHHmmss}.{format}";
            return File(content, contentType, fileName);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ExportReports");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteConsolidationReportCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Consolidation report with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteConsolidationReport", id);
        }
    }
}

// ✅ ADD THIS DTO
public class ExportReportsRequestDto
{
    public List<Guid> ReportIds { get; set; } = new();
    public string? Format { get; set; } = "json";
}