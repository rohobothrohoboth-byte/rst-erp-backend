// src/Svc.Core/Cor.PlanDev/Controllers/TimelineController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Models.Entities;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.PlanDev.Controllers;

[ApiController]
[Route("api/plandev/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class TimelineController : BaseApiController
{
    private readonly PlanDevDbContext _context;

    public TimelineController(PlanDevDbContext context, ILogger<TimelineController> logger)
        : base(logger)
    {
        _context = context;
    }

    /// <summary>
    /// Get project timeline
    /// </summary>
    [HttpGet("project/{projectId}")]
    [ProducesResponseType(typeof(List<TimelineDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectTimeline(Guid projectId)
    {
        try
        {
            var timeline = await _context.Timelines
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .OrderBy(t => t.Order)
                .Select(t => new TimelineDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    Name = t.Name,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Status = t.Status,
                    TimelineType = t.TimelineType,
                    Order = t.Order,
                    ParentTimelineId = t.ParentTimelineId
                })
                .ToListAsync();

            return Ok(timeline);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetProjectTimeline));
        }
    }

    /// <summary>
    /// Get timeline by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TimelineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var timeline = await _context.Timelines
                .Where(t => t.Id == id && !t.IsDeleted)
                .Select(t => new TimelineDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    Name = t.Name,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Status = t.Status,
                    TimelineType = t.TimelineType,
                    Order = t.Order,
                    ParentTimelineId = t.ParentTimelineId
                })
                .FirstOrDefaultAsync();

            if (timeline == null)
                return NotFound(new { message = $"Timeline with ID '{id}' not found" });

            return Ok(timeline);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    /// <summary>
    /// Create a new timeline entry
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TimelineDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTimelineDto createDto)
    {
        try
        {
            var timeline = new Timeline
            {
                Id = Guid.NewGuid(),
                ProjectId = createDto.ProjectId,
                Name = createDto.Name,
                Description = createDto.Description,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                Status = createDto.Status ?? "Planning",
                TimelineType = createDto.TimelineType,
                Order = createDto.Order,
                ParentTimelineId = createDto.ParentTimelineId,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            timeline.UpdateRowVersion();

            await _context.Timelines.AddAsync(timeline);
            await _context.SaveChangesAsync();

            var result = new TimelineDto
            {
                Id = timeline.Id,
                ProjectId = timeline.ProjectId,
                Name = timeline.Name,
                Description = timeline.Description,
                StartDate = timeline.StartDate,
                EndDate = timeline.EndDate,
                Status = timeline.Status,
                TimelineType = timeline.TimelineType,
                Order = timeline.Order,
                ParentTimelineId = timeline.ParentTimelineId
            };

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    /// <summary>
    /// Update a timeline entry
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(TimelineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateTimelineDto updateDto)
    {
        try
        {
            var timeline = await _context.Timelines
                .FirstOrDefaultAsync(t => t.Id == updateDto.Id && !t.IsDeleted);

            if (timeline == null)
                return NotFound(new { message = $"Timeline with ID '{updateDto.Id}' not found" });

            if (!string.IsNullOrEmpty(updateDto.Name))
                timeline.Name = updateDto.Name;

            if (!string.IsNullOrEmpty(updateDto.Description))
                timeline.Description = updateDto.Description;

            if (updateDto.StartDate.HasValue)
                timeline.StartDate = updateDto.StartDate.Value;

            if (updateDto.EndDate.HasValue)
                timeline.EndDate = updateDto.EndDate.Value;

            if (!string.IsNullOrEmpty(updateDto.Status))
                timeline.Status = updateDto.Status;

            if (!string.IsNullOrEmpty(updateDto.TimelineType))
                timeline.TimelineType = updateDto.TimelineType;

            if (updateDto.Order.HasValue)
                timeline.Order = updateDto.Order.Value;

            if (updateDto.ParentTimelineId.HasValue)
                timeline.ParentTimelineId = updateDto.ParentTimelineId;

            timeline.DateMod = DateTime.UtcNow;
            timeline.UpdateRowVersion();

            await _context.SaveChangesAsync();

            var result = new TimelineDto
            {
                Id = timeline.Id,
                ProjectId = timeline.ProjectId,
                Name = timeline.Name,
                Description = timeline.Description,
                StartDate = timeline.StartDate,
                EndDate = timeline.EndDate,
                Status = timeline.Status,
                TimelineType = timeline.TimelineType,
                Order = timeline.Order,
                ParentTimelineId = timeline.ParentTimelineId
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    /// <summary>
    /// Delete a timeline entry
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var timeline = await _context.Timelines
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (timeline == null)
                return NotFound(new { message = $"Timeline with ID '{id}' not found" });

            timeline.IsDeleted = true;
            timeline.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }
}