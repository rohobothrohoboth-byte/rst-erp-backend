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
/// EMPLOYEE TRANSFER management end points.
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/hrm/profile/v{version:apiVersion}/[controller]")]
public class TransferController(HrmProfileDbContext db) : ControllerBase
{
    private static TransferListDto ToDto(EmployeeTransfer e) => new()
    {
        Id = e.Id,
        EmployeeId = e.EmployeeId,
        FromBranch = e.FromBranch,
        ToBranch = e.ToBranch,
        FromDepartment = e.FromDepartment,
        ToDepartment = e.ToDepartment,
        EffectiveDate = e.EffectiveDate,
        Reason = e.Reason,
        Status = e.Status,
        DateAdd = e.DateAdd,
        DateMod = e.DateMod,
        IsDeleted = e.IsDeleted
    };

    [PerAuth("hr.emp.view")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, CancellationToken ct)
    {
        var query = db.EmployeeTransfer.AsNoTracking().Where(x => !x.IsDeleted);
        if (employeeId.HasValue)
            query = query.Where(x => x.EmployeeId == employeeId.Value);

        var list = await query
            .OrderByDescending(x => x.EffectiveDate)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return Ok(ApiResponse<object>.Ok(list));
    }

    [PerAuth("hr.emp.view")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var entity = await db.EmployeeTransfer.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE TRANSFER with given parameter NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(ToDto(entity)));
    }

    [PerAuth("hr.emp.view")]
    [HttpGet("employee/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var list = await db.EmployeeTransfer.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.EffectiveDate)
            .Select(x => ToDto(x))
            .ToListAsync(ct);
        return Ok(ApiResponse<object>.Ok(list));
    }

    [PerAuth("hr.emp.mod")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TransferAddDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var entity = new EmployeeTransfer
        {
            EmployeeId = dto.EmployeeId,
            FromBranch = dto.FromBranch,
            ToBranch = dto.ToBranch,
            FromDepartment = dto.FromDepartment,
            ToDepartment = dto.ToDepartment,
            EffectiveDate = dto.EffectiveDate,
            Reason = dto.Reason,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status
        };

        db.EmployeeTransfer.Add(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(ToDto(entity), "New EMPLOYEE TRANSFER successfully created."));
    }

    [PerAuth("hr.emp.mod")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TransferModDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var entity = await db.EmployeeTransfer
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE TRANSFER with given parameter NOT FOUND."); }

        entity.EmployeeId = dto.EmployeeId;
        entity.FromBranch = dto.FromBranch;
        entity.ToBranch = dto.ToBranch;
        entity.FromDepartment = dto.FromDepartment;
        entity.ToDepartment = dto.ToDepartment;
        entity.EffectiveDate = dto.EffectiveDate;
        entity.Reason = dto.Reason;
        entity.Status = dto.Status;

        db.EmployeeTransfer.Update(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(ToDto(entity), "Selected EMPLOYEE TRANSFER successfully updated."));
    }

    [PerAuth("hr.emp.mod")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await db.EmployeeTransfer
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE TRANSFER with given parameter NOT FOUND."); }

        entity.IsDeleted = true;
        db.EmployeeTransfer.Update(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<string>.Ok(null!, "Selected EMPLOYEE TRANSFER successfully deleted."));
    }
}
