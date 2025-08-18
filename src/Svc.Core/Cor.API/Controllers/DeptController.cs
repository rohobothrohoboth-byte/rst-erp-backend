using Asp.Versioning;
using Cor.App.Interfaces;
using Cor.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Cor.DTOs;
using System.Data;

namespace Cor.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class DeptController(IUnitOfWork uoW) : ControllerBase
    {
        [HttpGet(Name = "AllDepts")]
        public async Task<ActionResult<IEnumerable<Department>>> GetAll()
        {
            var depts = await uoW.Repository<Department>().GetAll();
            var allDepts = new List<DeptListDto>();
            foreach (var branch in depts)
            {
                allDepts.Add(await MapToDto(branch, true));
            }
            return Ok(allDepts);
        }

        [HttpGet("{id:guid}", Name = "GetDept")]
        public async Task<ActionResult<Branch>> GetById(Guid id)
        {
            var dept = await uoW.Repository<Department>().GetById(id);
            if (dept == null) return NotFound();
            return Ok(await MapToDto(dept, true));
        }

        [HttpPost(Name = "AddDept")]
        public async Task<ActionResult<Department>> Create(AddDeptDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var dept = new Department
            {
                Name = dto.Name,
                NameAm = dto.NameAm,
                BranchId = dto.BranchId
            };

            await uoW.Repository<Department>().Add(dept);
            return CreatedAtAction(nameof(GetById), new { id = dept.Id }, await MapToDto(dept, false));
        }

        [HttpPut("{id:guid}", Name = "EditDept")]
        public async Task<ActionResult<Branch>> Update(Guid id, EdtDeptDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var repo = uoW.Repository<Department>();
            var oldDept = await repo.GetById(id);

            if (oldDept == null) { return NotFound(); }

            oldDept.Name = dto.Name;
            oldDept.NameAm = dto.NameAm;
            oldDept.BranchId = dto.BranchId;

            try
            {
                oldDept.RowVersion = Convert.FromBase64String(dto.RowVersion);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid RowVersion format.");
            }

            uoW.Begin();
            try
            {
                var updated = await repo.Update(oldDept);
                uoW.Commit();
                return Ok(await MapToDto(updated, false));
            }
            catch (DBConcurrencyException)
            {
                uoW.Rollback();
                return Conflict("Concurrency conflict: entity has been modified by another user.");
            }
            catch (Exception ex)
            {
                uoW.Rollback();
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}", Name = "RemoveDept")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var repo = uoW.Repository<Department>();
            var existing = await repo.GetById(id);
            if (existing == null) { return NotFound(); }

            uoW.Begin();
            try
            {
                await repo.Delete(id);
                uoW.Commit();
                return Content("Department Deleted!");
            }
            catch (Exception ex)
            {
                uoW.Rollback();
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        private async Task<DeptListDto> MapToDto(Department b, bool e)
        {
            var repo = uoW.Repository<Branch>();
            var bra = await repo.GetById(b.BranchId);

            return new DeptListDto
            {
                Id = b.Id,
                Name = b.Name,
                NameAm = b.NameAm,
                BranchId = b.BranchId,
                Branch = bra?.Name ?? "",
                CreatedAt = b.CreatedAt,
                CreatedAtAm = b.CreatedAtAm,
                ModifiedAt = b.ModifiedAt,
                ModifiedAtAm = b.ModifiedAtAm,
                RowVersion = e ? Convert.ToBase64String(b.RowVersion) : ""
            };
        }
    }
}
