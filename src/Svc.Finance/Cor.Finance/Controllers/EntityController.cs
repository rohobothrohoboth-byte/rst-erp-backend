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
public class EntityController : BaseApiController
{
    public EntityController(IMediator mediator, ILogger<EntityController> logger)
        : base(mediator, logger)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<EntityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
    {
        try
        {
            var result = await Mediator.Send(new GetAllEntitiesQry { IsActive = isActive });
            return Ok(result ?? new List<EntityDto>());
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllEntities");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EntityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetEntityByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetEntityById", id);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(EntityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddEntityDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid request body" });

            var result = await Mediator.Send(new AddEntityCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateEntity");
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(EntityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditEntityDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid request body" });

            var result = await Mediator.Send(new EditEntityCmd { EditDto = dto });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateEntity", dto.Id);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new DeleteEntityCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Entity with ID '{id}' not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteEntity", id);
        }
    }
}