using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Constants;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers.Services;
using Microsoft.AspNetCore.Authorization;
namespace Cor.HRMM.Controllers;

/// <summary>
/// Job Grade Step endpoints
/// </summary>

[Authorize(AuthenticationSchemes = "ApiKey,Bearer")]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/JgStep")]
[ApiVersion("1.0")]

public class JgStepController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICacheService _cache;
    private readonly ILogger<JgStepController> _logger;

    public JgStepController(
        IMediator mediator,
        ICacheService cache,
        ILogger<JgStepController> logger)
    {
        _mediator = mediator;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// End point to get list of ALL Job Grade Steps
    /// </summary>
    [HttpGet("AllJgSteps")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJgSteps()
    {
        var response = await _cache.GetOrCreateAsync(
            CacheKeys.AllJgSteps,
            async (ct) =>
            {
                _logger.LogInformation("🔍 Cache MISS - Loading AllJgSteps from database");
                return await _mediator.Send(new JgStepAllQry(), ct);
            },
            TimeSpan.FromMinutes(5)
        );

        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// End point to get list of Job Grade Steps by JobGradeId
    /// </summary>
    [HttpGet("AllJgStepsByJobGrade/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJgStepsByJobGrade(Guid id)
    {
        var cacheKey = CacheKeys.JgStepsByJobGrade(id);

        var response = await _cache.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                _logger.LogInformation("🔍 Cache MISS - Loading JgSteps for JobGrade {Id} from database", id);
                return await _mediator.Send(new JgStepByJobGradeIdQry { JobGradeId = id }, ct);
            },
            TimeSpan.FromMinutes(5)
        );

        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetJgStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJgStep(Guid id)
    {
        var cacheKey = CacheKeys.JgStep(id);

        var response = await _cache.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                _logger.LogInformation("🔍 Cache MISS - Loading JgStep {Id} from database", id);
                var result = await _mediator.Send(new JgStepByIdQry { Id = id }, ct);
                if (result == null)
                    throw new DomainException($"JOB GRADE STEP with id [{id}] NOT FOUND.");
                return result;
            },
            TimeSpan.FromMinutes(10)
        );

        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddJgStep")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] JgStepAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JgStepAddCmd { AddDto = addDto };
        var response = await _mediator.Send(command);

        // ✅ Invalidate all related caches
        await _cache.RemoveAsync(CacheKeys.AllJgSteps);
        if (addDto.JobGradeId != Guid.Empty)
        {
            await _cache.RemoveAsync(CacheKeys.JgStepsByJobGrade(addDto.JobGradeId));
        }
        _logger.LogInformation("🗑️ Cache invalidated: {AllCache}", CacheKeys.AllJgSteps);

        return Ok(ApiResponse<object>.Ok(response, "New JOB GRADE STEP successfully created."));
    }

    [HttpPut("ModJgStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] JgStepModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        // Get the existing entity to know its JobGradeId for cache invalidation
        var existing = await _mediator.Send(new JgStepByIdQry { Id = id });
        var oldJobGradeId = existing?.JobGradeId ?? Guid.Empty;

        var command = new JgStepModCmd { ModDto = modDto };
        var response = await _mediator.Send(command);

        // ✅ Invalidate all related caches
        await _cache.RemoveAsync(CacheKeys.AllJgSteps);
        await _cache.RemoveAsync(CacheKeys.JgStep(id));

        // Invalidate old JobGrade cache
        if (oldJobGradeId != Guid.Empty)
        {
            await _cache.RemoveAsync(CacheKeys.JgStepsByJobGrade(oldJobGradeId));
        }

        // Invalidate new JobGrade cache if it changed
        if (modDto.JobGradeId != Guid.Empty && modDto.JobGradeId != oldJobGradeId)
        {
            await _cache.RemoveAsync(CacheKeys.JgStepsByJobGrade(modDto.JobGradeId));
        }
        _logger.LogInformation("🗑️ Cache invalidated for JgStep {Id}", id);

        return Ok(ApiResponse<object>.Ok(response, "Selected JOB GRADE STEP successfully updated."));
    }

    [HttpDelete("DelJgStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Get the existing entity to know its JobGradeId for cache invalidation
        var existing = await _mediator.Send(new JgStepByIdQry { Id = id });
        var jobGradeId = existing?.JobGradeId ?? Guid.Empty;

        var command = new JgStepDelCmd { Id = id };
        await _mediator.Send(command);

        // ✅ Invalidate all related caches
        await _cache.RemoveAsync(CacheKeys.AllJgSteps);
        await _cache.RemoveAsync(CacheKeys.JgStep(id));
        if (jobGradeId != Guid.Empty)
        {
            await _cache.RemoveAsync(CacheKeys.JgStepsByJobGrade(jobGradeId));
        }
        _logger.LogInformation("🗑️ Cache invalidated for JgStep {Id}", id);

        return Ok(ApiResponse<string>.Ok(null!, $"JOB GRADE STEP with Id {id} successfully deleted."));
    }
}

