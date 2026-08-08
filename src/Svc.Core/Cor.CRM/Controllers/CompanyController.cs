// Cor.CRM/Controllers/CompanyController.cs

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
[Route("api/core/crm/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CompanyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public CompanyController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all companies with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? industry,
        [FromQuery] string? status,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder)
    {
        try
        {
            var query = new CompanyAllQry
            {
                Search = search,
                Industry = industry,
                Status = status,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all companies");
            throw;
        }
    }

    /// <summary>
    /// Get a single company by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CompanyByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Company with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company: {CompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
        try
        {
            var command = new CompanyAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Company created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            throw;
        }
    }

    /// <summary>
    /// Update an existing company
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyDto dto)
    {
        try
        {
            var command = new CompanyModCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Company updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company: {CompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a company
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new CompanyDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Company with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company: {CompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get company statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new CompanyStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company stats");
            throw;
        }
    }
}