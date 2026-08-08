// Controllers/InternalControlController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class InternalControlController : BaseApiController
{
    private readonly IMediator _mediator;

    public InternalControlController(IMediator mediator, ILogger<InternalControlController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<InternalControlDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] string? category)
    {
        try
        {
            var result = await _mediator.Send(new GetAllInternalControlsQry { Status = status, Category = category });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllInternalControls");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InternalControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetInternalControlByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetInternalControlById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(InternalControlDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddInternalControlDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddInternalControlCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateInternalControl");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(InternalControlDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditInternalControlDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditInternalControlCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateInternalControl", dto.Id);
        }
    }

    [HttpPost("{id}/test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestControl(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new TestInternalControlCmd { Id = id });
            return Ok(new { success = true, message = "Control tested successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "TestInternalControl", id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteInternalControlCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Internal control with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteInternalControl", id);
        }
    }
}
