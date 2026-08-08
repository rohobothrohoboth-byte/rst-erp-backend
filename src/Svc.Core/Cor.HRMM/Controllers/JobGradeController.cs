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
/// Job Grade end points
/// </summary>

[Authorize(AuthenticationSchemes = "ApiKey,Bearer")]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/JobGrade")]
[ApiVersion("1.0")]

public class JobGradeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICacheService _cache;
    private readonly ILogger<JobGradeController> _logger;

    public JobGradeController(
        IMediator mediator,
        ICacheService cache,
        ILogger<JobGradeController> logger)
    {
        _mediator = mediator;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet("AllJobGrade")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJobGrade()
    {
        var response = await _cache.GetOrCreateAsync(
            CacheKeys.AllJobGrades,
            async (ct) =>
            {
                _logger.LogInformation("🔍 Cache MISS - Loading AllJobGrade from database");
                return await _mediator.Send(new JobGradeAllQry(), ct);
            },
            TimeSpan.FromMinutes(5)
        );

        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetJobGrade/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobGrade(Guid id)
    {
        var cacheKey = CacheKeys.JobGrade(id);

        var response = await _cache.GetOrCreateAsync(
            cacheKey,
            async (ct) =>
            {
                _logger.LogInformation("🔍 Cache MISS - Loading JobGrade {Id} from database", id);
                var result = await _mediator.Send(new JobGradeByIdQry { Id = id }, ct);
                if (result == null)
                {
                    throw new DomainException($"JOB GRADE with id [{id}] NOT FOUND.");
                }
                return result;
            },
            TimeSpan.FromMinutes(10)
        );

        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddJobGrade")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] JobGradeAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobGradeAddCmd { AddDto = addDto };
        var response = await _mediator.Send(command);

        // ✅ Invalidate cache after adding
        await _cache.RemoveAsync(CacheKeys.AllJobGrades);
        _logger.LogInformation("🗑️ Cache invalidated: {CacheKey}", CacheKeys.AllJobGrades);

        return Ok(ApiResponse<object>.Ok(response, "New JOB GRADE successfully created."));
    }

    [HttpPut("ModJobGrade/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] JobGradeModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobGradeModCmd { ModDto = modDto };
        var response = await _mediator.Send(command);

        // ✅ Invalidate cache after updating
        await _cache.RemoveAsync(CacheKeys.AllJobGrades);
        await _cache.RemoveAsync(CacheKeys.JobGrade(id));
        _logger.LogInformation("🗑️ Cache invalidated: {AllCache} and {SingleCache}",
            CacheKeys.AllJobGrades, CacheKeys.JobGrade(id));

        return Ok(ApiResponse<object>.Ok(response, "Selected JOB GRADE successfully updated."));
    }

    [HttpDelete("DelJobGrade/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new JobGradeDelCmd { Id = id };
        await _mediator.Send(command);

        // ✅ Invalidate cache after deleting
        await _cache.RemoveAsync(CacheKeys.AllJobGrades);
        await _cache.RemoveAsync(CacheKeys.JobGrade(id));
        _logger.LogInformation("🗑️ Cache invalidated: {AllCache} and {SingleCache}",
            CacheKeys.AllJobGrades, CacheKeys.JobGrade(id));

        return Ok(ApiResponse<string>.Ok(null!, $"JOB GRADE with Id {id} successfully deleted."));
    }
}

