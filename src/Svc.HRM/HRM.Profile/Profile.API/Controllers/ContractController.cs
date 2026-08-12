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
/// EMPLOYEE CONTRACT management end points.
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/hrm/profile/v{version:apiVersion}/[controller]")]
public class ContractController(HrmProfileDbContext db) : ControllerBase
{
    private static ContractListDto ToDto(EmployeeContract e) => new()
    {
        Id = e.Id,
        EmployeeId = e.EmployeeId,
        ContractType = e.ContractType,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        Salary = e.Salary,
        Status = e.Status,
        Notes = e.Notes,
        DateAdd = e.DateAdd,
        DateMod = e.DateMod,
        IsDeleted = e.IsDeleted
    };

    [PerAuth("hr.emp.contract.view")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? employeeId, CancellationToken ct)
    {
        var query = db.EmployeeContract.AsNoTracking().Where(x => !x.IsDeleted);
        if (employeeId.HasValue)
            query = query.Where(x => x.EmployeeId == employeeId.Value);

        var list = await query
            .OrderByDescending(x => x.StartDate)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return Ok(ApiResponse<object>.Ok(list));
    }

    [PerAuth("hr.emp.contract.view")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var entity = await db.EmployeeContract.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE CONTRACT with given parameter NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(ToDto(entity)));
    }

    [PerAuth("hr.emp.contract.view")]
    [HttpGet("employee/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var list = await db.EmployeeContract.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.StartDate)
            .Select(x => ToDto(x))
            .ToListAsync(ct);
        return Ok(ApiResponse<object>.Ok(list));
    }

    [PerAuth("hr.emp.contract.add")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ContractAddDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var entity = new EmployeeContract
        {
            EmployeeId = dto.EmployeeId,
            ContractType = dto.ContractType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Salary = dto.Salary,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status,
            Notes = dto.Notes
        };

        db.EmployeeContract.Add(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(ToDto(entity), "New EMPLOYEE CONTRACT successfully created."));
    }

    [PerAuth("hr.emp.contract.mod")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContractModDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var entity = await db.EmployeeContract
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE CONTRACT with given parameter NOT FOUND."); }

        entity.EmployeeId = dto.EmployeeId;
        entity.ContractType = dto.ContractType;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Salary = dto.Salary;
        entity.Status = dto.Status;
        entity.Notes = dto.Notes;

        db.EmployeeContract.Update(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(ToDto(entity), "Selected EMPLOYEE CONTRACT successfully updated."));
    }

    [PerAuth("hr.emp.contract.del")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await db.EmployeeContract
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) { throw new DomainException("EMPLOYEE CONTRACT with given parameter NOT FOUND."); }

        entity.IsDeleted = true;
        db.EmployeeContract.Update(entity);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<string>.Ok(null!, "Selected EMPLOYEE CONTRACT successfully deleted."));
    }
}
