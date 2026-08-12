using Asp.Versioning;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Utility.Persistence;

namespace Profile.API.Controllers;

/// <summary>
/// EMPLOYEE PROMOTION management end points.
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/hrm/profile/v{version:apiVersion}/[controller]")]
public class PromotionController(HrmProfileDbContext db) : ControllerBase
{
    private static PromotionListDto ToDto(EmployeePromotion e) => new()
    {
        Id = e.Id,
        EmployeeId = e.EmployeeId,
        FromPosition = e.FromPosition,
        ToPosition = e.ToPosition,
        EffectiveDate = e.EffectiveDate,
        Reason = e.Reason,
        Status = e.Status,
        ApprovedBy = e.ApprovedBy,
        DateAdd = e.DateAdd,
        DateMod = e.DateMod,
        IsDeleted = e.IsDeleted
    };

    [PerAuth("hr.emp.promotion.view")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, CancellationToken ct)
    {
        var query = db.EmployeePromotion.AsNoTracking().Where(x => !x.IsDeleted);
        if (employeeId.HasValue)
            query = query.Where(x => x.EmployeeId == employeeId.Value);

        var list = await query
            .OrderByDescending(x => x.EffectiveDate)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return Ok(ApiResponse<object>.Ok(list));
    }

    [PerAuth("hr.emp.promotion.view")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var entity = await db.EmployeePromotion.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE PROMOTION with given parameter NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(ToDto(entity)));
    }

    [PerAuth("hr.emp.promotion.view")]
    [HttpGet("employee/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var list = await db.EmployeePromotion.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.EffectiveDate)
            .Select(x => ToDto(x))
            .ToListAsync(ct);
        return Ok(ApiResponse<object>.Ok(list));
    }

    [PerAuth("hr.emp.promotion.add")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PromotionAddDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var entity = new EmployeePromotion
        {
            EmployeeId = dto.EmployeeId,
            FromPosition = dto.FromPosition,
            ToPosition = dto.ToPosition,
            EffectiveDate = dto.EffectiveDate,
            Reason = dto.Reason,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status
        };

        db.EmployeePromotion.Add(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(ToDto(entity), "New EMPLOYEE PROMOTION successfully created."));
    }

    [PerAuth("hr.emp.promotion.mod")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PromotionModDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var entity = await db.EmployeePromotion
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE PROMOTION with given parameter NOT FOUND."); }

        entity.EmployeeId = dto.EmployeeId;
        entity.FromPosition = dto.FromPosition;
        entity.ToPosition = dto.ToPosition;
        entity.EffectiveDate = dto.EffectiveDate;
        entity.Reason = dto.Reason;
        entity.Status = dto.Status;
        entity.ApprovedBy = dto.ApprovedBy;

        db.EmployeePromotion.Update(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(ToDto(entity), "Selected EMPLOYEE PROMOTION successfully updated."));
    }

    [PerAuth("hr.emp.promotion.approve")]
    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] PromotionApproveDto dto, CancellationToken ct)
    {
        if (dto.Id != id)
        {
            throw new ValException(new List<string> { "Route id does not match payload id." });
        }

        var entity = await db.EmployeePromotion
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE PROMOTION with given parameter NOT FOUND."); }

        entity.Status = "Approved";
        entity.ApprovedBy = dto.ApprovedBy;

        db.EmployeePromotion.Update(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(ToDto(entity), "Selected EMPLOYEE PROMOTION successfully approved."));
    }

    [PerAuth("hr.emp.promotion.del")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await db.EmployeePromotion
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE PROMOTION with given parameter NOT FOUND."); }

        entity.IsDeleted = true;
        db.EmployeePromotion.Update(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<string>.Ok(null!, "Selected EMPLOYEE PROMOTION successfully deleted."));
    }
}
