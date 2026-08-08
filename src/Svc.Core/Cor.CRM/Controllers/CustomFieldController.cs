// Cor.CRM/Controllers/CustomFieldController.cs

using Asp.Versioning;
using Cor.CRM.Commands;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;
namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/CustomField")]
[ApiVersion("1.0")]
public class CustomFieldController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public CustomFieldController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all custom field definitions
    /// </summary>
    [HttpGet("Definitions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDefinitions([FromQuery] string? entityType)
    {
        try
        {
            var response = await _mediator.Send(new CustomFieldDefinitionsQry { EntityType = entityType });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting custom field definitions");
            throw;
        }
    }

    /// <summary>
    /// Get custom field definition by ID
    /// </summary>
    [HttpGet("Definition/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDefinition(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CustomFieldDefinitionByIdQry { Id = id });
            if (response == null)
            {
                throw new DomainException($"Custom field definition with id [{id}] NOT FOUND.");
            }
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting custom field definition: {DefinitionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a custom field definition
    /// </summary>
    [HttpPost("Definition")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDefinition([FromBody] CreateCustomFieldDefinitionDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new CustomFieldDefinitionAddCmd { Dto = addDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Custom field definition created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating custom field definition");
            throw;
        }
    }

    /// <summary>
    /// Update a custom field definition
    /// </summary>
    [HttpPut("Definition/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDefinition(Guid id, [FromBody] UpdateCustomFieldDefinitionDto modDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new CustomFieldDefinitionModCmd { Id = id, Dto = modDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Custom field definition updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating custom field definition: {DefinitionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a custom field definition
    /// </summary>
    [HttpDelete("Definition/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDefinition(Guid id)
    {
        try
        {
            var command = new CustomFieldDefinitionDelCmd { Id = id };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, $"Custom field definition with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting custom field definition: {DefinitionId}", id);
            throw;
        }
    }
}