using Asp.Versioning;
using Common;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using Cor.HRMM.Services;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;  // ✅ Add this
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers.Services;
namespace Cor.HRMM.Controllers;

/// <summary>
/// Employees Position end points
/// </summary>

[Authorize(AuthenticationSchemes = "ApiKey,Bearer")]  // ✅ UNCOMMENT and add schemes
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/Position")]
[ApiVersion("1.0")]
public class PositionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICacheService _cache;
    private readonly ILogger<PositionController> _logger;

    public PositionController(
        IMediator mediator,
        ICacheService cache,
        ILogger<PositionController> logger)
    {
        _mediator = mediator;
        _cache = cache;
        _logger = logger;
    }

    [PerAuth("hr.emp.view|hr.db.view|hr.recruit.requisition.view")]
    [HttpGet("AllPosition")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPosition()
    {
        const string cacheKey = "AllPositions";

        var response = await _cache.GetOrCreateWithLockAsync(
            cacheKey,
            async (ct) =>
            {
                _logger.LogInformation("🔍 Cache MISS - Loading AllPosition from database");
                return await _mediator.Send(new PositionAllQry(), ct);
            },
            TimeSpan.FromMinutes(5)
        );

        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.view|hr.db.view|hr.recruit.requisition.view")]
    [HttpGet("GetPosition/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPosition(Guid id)
    {
        var cacheKey = $"Position:{id}";

        var response = await _cache.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                _logger.LogInformation("🔍 Cache MISS - Loading Position {Id} from database", id);
                var result = await _mediator.Send(new PositionByIdQry { Id = id }, ct);
                if (result == null)
                {
                    throw new DomainException($"POSITION with id [{id}] NOT FOUND.");
                }
                return result;
            },
            TimeSpan.FromMinutes(10)
        );

        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddPosition")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PositionAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PositionAddCmd { AddDto = addDto };
        var response = await _mediator.Send(command);

        await _cache.RemoveAsync("AllPositions");
        _logger.LogInformation("🗑️ Cache invalidated: AllPositions");

        return Ok(ApiResponse<object>.Ok(response, "New POSITION successfully created."));
    }

    [HttpPut("ModPosition/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PositionModCmd { ModDto = modDto };
        var response = await _mediator.Send(command);

        await _cache.RemoveAsync("AllPositions");
        await _cache.RemoveAsync($"Position:{id}");
        _logger.LogInformation("🗑️ Cache invalidated: AllPositions and Position:{Id}", id);

        return Ok(ApiResponse<object>.Ok(response, "Selected POSITION successfully updated."));
    }

    [HttpDelete("DelPosition/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new PositionDelCmd { Id = id };
        await _mediator.Send(command);

        await _cache.RemoveAsync("AllPositions");
        await _cache.RemoveAsync($"Position:{id}");
        _logger.LogInformation("🗑️ Cache invalidated: AllPositions and Position:{Id}", id);

        return Ok(ApiResponse<string>.Ok(null!, $"POSITION with Id {id} successfully deleted."));
    }
}
