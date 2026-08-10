// Cor.CRM/Controllers/PropertyController.cs

using Asp.Versioning;
using Common;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Cor.CRM.Commands;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;
namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class PropertyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public PropertyController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("crm.realestate.properties.view")]
    public async Task<IActionResult> GetAll([FromQuery] PropertyFilterDto filter)
    {
        try
        {
            var query = new PropertyAllQry { Filter = filter };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting properties");
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    [PerAuth("crm.realestate.properties.view")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new PropertyByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Property with id [{id}] NOT FOUND.");
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting property: {PropertyId}", id);
            throw;
        }
    }

    [HttpPost]
    [PerAuth("crm.realestate.properties.add")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
    {
        try
        {
            var command = new PropertyAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Property created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property");
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    [PerAuth("crm.realestate.properties.mod")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto dto)
    {
        try
        {
            var command = new PropertyUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Property updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property: {PropertyId}", id);
            throw;
        }
    }

    [HttpDelete("{id:guid}")]
    [PerAuth("crm.realestate.properties.del")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new PropertyDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Property with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property: {PropertyId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/publish")]
    [PerAuth("crm.realestate.properties.view")]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new PropertyPublishCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Property published successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing property: {PropertyId}", id);
            throw;
        }
    }
    [HttpGet("stats")]
        [PerAuth("crm.realestate.properties.view")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var response = await _mediator.Send(new PropertyStatsQry());
                return Ok(ApiResponse<object>.Ok(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting property stats");
                throw;
            }
        }
}