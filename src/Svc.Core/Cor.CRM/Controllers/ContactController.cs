// Cor.CRM/Controllers/ContactController.cs

using Asp.Versioning;
using Common;
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
[Route("api/core/crm/v{version:apiVersion}/Contact")]
[ApiVersion("1.0")]
public class ContactController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public ContactController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all contacts with optional filters
    /// </summary>
    [HttpGet("AllContacts")]
    [PerAuth("crm.contacts.list.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllContacts([FromQuery] string? customerId, [FromQuery] string? search)
    {
        try
        {
            var response = await _mediator.Send(new ContactAllQry { CustomerId = customerId, Search = search });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all contacts");
            throw;
        }
    }

    /// <summary>
    /// Get a single contact by ID
    /// </summary>
    [HttpGet("GetContact/{id:guid}")]
    [PerAuth("crm.contacts.list.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContact(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ContactByIdQry { Id = id });
            if (response == null)
            {
                throw new DomainException($"Contact with id [{id}] NOT FOUND.");
            }
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact: {ContactId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new contact
    /// </summary>
    [HttpPost("AddContact")]
    [PerAuth("crm.contacts.list.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContact([FromBody] CreateContactDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new ContactAddCmd { Dto = addDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Contact created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact");
            throw;
        }
    }

    /// <summary>
    /// Update an existing contact
    /// </summary>
    [HttpPut("ModContact/{id:guid}")]
    [PerAuth("crm.contacts.list.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateContact(Guid id, [FromBody] UpdateContactDto modDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new ContactModCmd { Id = id, Dto = modDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Contact updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact: {ContactId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a contact
    /// </summary>
    [HttpDelete("DelContact/{id:guid}")]
    [PerAuth("crm.contacts.list.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContact(Guid id)
    {
        try
        {
            var command = new ContactDelCmd { Id = id };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, $"Contact with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact: {ContactId}", id);
            throw;
        }
    }
}