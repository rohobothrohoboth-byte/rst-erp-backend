// Cor.CRM/Controllers/CustomerController.cs

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
[Route("api/core/crm/v{version:apiVersion}/Customer")]
[ApiVersion("1.0")]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public CustomerController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers with optional filters
    /// </summary>
    [HttpGet("AllCustomers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCustomers([FromQuery] CustomerFilterDto? filter)
    {
        try
        {
            var response = await _mediator.Send(new CustomerAllQry { Filter = filter });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all customers");
            throw;
        }
    }

    /// <summary>
    /// Get a single customer by ID
    /// </summary>
    [HttpGet("GetCustomer/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CustomerByIdQry { Id = id });
            if (response == null)
            {
                throw new DomainException($"Customer with id [{id}] NOT FOUND.");
            }
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer: {CustomerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost("AddCustomer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new CustomerAddCmd { Dto = addDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Customer created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            throw;
        }
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    [HttpPut("ModCustomer/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto modDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new CustomerModCmd { Id = id, Dto = modDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Customer updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer: {CustomerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("DelCustomer/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        try
        {
            var command = new CustomerDelCmd { Id = id };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, $"Customer with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer: {CustomerId}", id);
            throw;
        }
    }
}

