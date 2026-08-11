// Controllers/EliminationEntryController.cs
using Common;
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;
// Controllers/EliminationEntryController.cs
[ApiController]
[Route("api/finance/v{version:apiVersion}/elimination-entries")]
[ApiVersion("1.0")]
[Authorize]
public class EliminationEntryController : BaseApiController
{
    private readonly IMediator _mediator;

    public EliminationEntryController(IMediator mediator, ILogger<EliminationEntryController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [PerAuth("fnm.cons.eliminations.view")]
    [ProducesResponseType(typeof(List<EliminationEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? consolidationGroupId,
        [FromQuery] string? status,
        [FromQuery] string? type,
        [FromQuery] Guid? fromEntityId,
        [FromQuery] Guid? toEntityId)
    {
        try
        {
            var result = await _mediator.Send(new GetAllEliminationEntriesQry
            {
                ConsolidationGroupId = consolidationGroupId,
                Status = status,
                Type = type,
                FromEntityId = fromEntityId,
                ToEntityId = toEntityId
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllEliminationEntries");
        }
    }

    [HttpGet("{id}")]
    [PerAuth("fnm.cons.eliminations.view")]
    [ProducesResponseType(typeof(EliminationEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetEliminationEntryByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetEliminationEntryById", id);
        }
    }

    [HttpPost]
    [PerAuth("fnm.cons.eliminations.add")]
    [ProducesResponseType(typeof(EliminationEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddEliminationEntryDto dto)
    {
        try
        {
            var result = await _mediator.Send(new AddEliminationEntryCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Elimination entry created successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateEliminationEntry");
        }
    }

    [HttpPut]
    [PerAuth("fnm.cons.eliminations.mod")]
    [ProducesResponseType(typeof(EliminationEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditEliminationEntryDto dto)
    {
        try
        {
            var result = await _mediator.Send(new EditEliminationEntryCmd { EditDto = dto });
            return Ok(new
            {
                success = true,
                message = "Elimination entry updated successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateEliminationEntry", dto.Id);
        }
    }

    [HttpPost("{id}/post")]
    [PerAuth("fnm.cons.eliminations.view")]
    [ProducesResponseType(typeof(EliminationEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post(Guid id, [FromBody] PostEliminationEntryDto? dto = null)
    {
        try
        {
            var postDto = dto ?? new PostEliminationEntryDto { Id = id };
            postDto.Id = id;
            var result = await _mediator.Send(new PostEliminationEntryCmd { PostDto = postDto });
            return Ok(new
            {
                success = true,
                message = "Elimination entry posted successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "PostEliminationEntry", id);
        }
    }

    [HttpPost("{id}/reject")]
    [PerAuth("fnm.cons.eliminations.view")]
    [ProducesResponseType(typeof(EliminationEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectEliminationEntryDto dto)
    {
        try
        {
            dto.Id = id;
            var result = await _mediator.Send(new RejectEliminationEntryCmd { RejectDto = dto });
            return Ok(new
            {
                success = true,
                message = "Elimination entry rejected successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "RejectEliminationEntry", id);
        }
    }

    [HttpDelete("{id}")]
    [PerAuth("fnm.cons.eliminations.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteEliminationEntryCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Elimination entry with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteEliminationEntry", id);
        }
    }
}