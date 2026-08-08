// Controllers/ComplianceRequirementController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

// Controllers/ComplianceRequirementController.cs
[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ComplianceRequirementController : BaseApiController
{
    private readonly IMediator _mediator;

    public ComplianceRequirementController(IMediator mediator, ILogger<ComplianceRequirementController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ComplianceRequirementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? regulation,
        [FromQuery] string? riskLevel)
    {
        try
        {
            var result = await _mediator.Send(new GetAllComplianceRequirementsQry
            {
                Status = status,
                Regulation = regulation,
                RiskLevel = riskLevel
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllComplianceRequirements");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ComplianceRequirementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetComplianceRequirementByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetComplianceRequirementById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ComplianceRequirementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddComplianceRequirementDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddComplianceRequirementCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateComplianceRequirement");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(ComplianceRequirementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditComplianceRequirementDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditComplianceRequirementCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateComplianceRequirement", dto.Id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteComplianceRequirementCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Compliance requirement with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteComplianceRequirement", id);
        }
    }
}