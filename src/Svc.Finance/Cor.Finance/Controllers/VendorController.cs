// Controllers/VendorController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using Cor.Finance.Helpers; // For DateTimeHelper if needed

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize] // ✅ Add authorization
public class VendorController : BaseApiController // ✅ Inherit from BaseApiController
{
    private readonly IMediator _mediator;

    public VendorController(IMediator mediator, ILogger<VendorController> logger)
        : base(mediator, logger) // ✅ Pass to base
    {
        _mediator = mediator;
    }

    [HttpGet("All")]
    [ProducesResponseType(typeof(List<VendorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _mediator.Send(new VendorAllQry());
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllVendors");
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new VendorByIdQry { Id = id });
            if (response == null)
                return NotFound(new { message = $"Vendor with ID '{id}' not found" });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetVendorById", id);
        }
    }

    [HttpGet("ByCode/{code}")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var response = await _mediator.Send(new VendorByCodeQry { Code = code });
            if (response == null)
                return NotFound(new { message = $"Vendor with code '{code}' not found" });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetVendorByCode", code);
        }
    }

    [HttpGet("ByType/{vendorType}")]
    [ProducesResponseType(typeof(List<VendorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByType(string vendorType)
    {
        try
        {
            var response = await _mediator.Send(new VendorByTypeQry { VendorType = vendorType });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetVendorsByType", vendorType);
        }
    }

    [HttpGet("Active")]
    [ProducesResponseType(typeof(List<VendorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var response = await _mediator.Send(new VendorActiveQry { IsActive = true });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetActiveVendors");
        }
    }

    [HttpGet("Summary")]
    [ProducesResponseType(typeof(VendorSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary([FromQuery] Guid? vendorId)
    {
        try
        {
            var response = await _mediator.Send(new VendorSummaryQry { VendorId = vendorId });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetVendorSummary");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] VendorCreateDto dto)
    {
        try
        {
            var response = await _mediator.Send(new VendorAddCmd { CreateDto = dto });
            return CreatedAtAction(nameof(Get), new { id = response.Id }, new
            {
                success = true,
                message = "Vendor created successfully",
                data = response
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateVendor");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] VendorUpdateDto dto)
    {
        try
        {
            var response = await _mediator.Send(new VendorModCmd { UpdateDto = dto });
            return Ok(new
            {
                success = true,
                message = "Vendor updated successfully",
                data = response
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateVendor", dto.Id);
        }
    }

    [HttpPatch("{id:guid}/toggle-status")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new VendorToggleStatusCmd { Id = id });
            return Ok(new
            {
                success = true,
                message = $"Vendor status toggled to {(response.IsActive ? "Active" : "Inactive")}",
                data = response
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleVendorStatus", id);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new VendorDelCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Vendor with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteVendor", id);
        }
    }
}