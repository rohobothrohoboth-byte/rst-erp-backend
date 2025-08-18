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
    public class HierarchyController(IUnitOfWork uoW) : ControllerBase
    {
        [HttpGet(Name = "AllHierarchies")]
        public async Task<ActionResult<IEnumerable<Hierarchy>>> GetAll()
        {
            var hier = await uoW.Repository<Hierarchy>().GetAll();
            var allHier = new List<HierListDto>();
            foreach (var branch in hier)
            {
                allHier.Add(await MapToDto(branch, true));
            }
            return Ok(allHier);
        }

        [HttpGet("{id:guid}", Name = "GetHierarchy")]
        public async Task<ActionResult<Hierarchy>> GetById(Guid id)
        {
            var hier = await uoW.Repository<Hierarchy>().GetById(id);
            if (hier == null) return NotFound();
            return Ok(await MapToDto(hier, true));
        }

        [HttpPost(Name = "AddHierarchy")]
        public async Task<ActionResult<Hierarchy>> Create(AddHierDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var hier = new Hierarchy
            {
                ParentId = dto.ParentId,
                ChildId = dto.ChildId
            };

            await uoW.Repository<Hierarchy>().Add(hier);
            return CreatedAtAction(nameof(GetById), new { id = hier.Id }, await MapToDto(hier, false));
        }

        [HttpPut("{id:guid}", Name = "EditHierarchy")]
        public async Task<ActionResult<Hierarchy>> Update(Guid id, EditHierDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var repo = uoW.Repository<Hierarchy>();
            var oldHier = await repo.GetById(id);

            if (oldHier == null) { return NotFound(); }

            oldHier.ParentId = dto.ParentId;
            oldHier.ChildId = dto.ChildId;

            try
            {
                oldHier.RowVersion = Convert.FromBase64String(dto.RowVersion);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid RowVersion format.");
            }

            uoW.Begin();
            try
            {
                var updated = await repo.Update(oldHier);
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

        [HttpDelete("{id:guid}", Name = "RemoveHierarchy")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var repo = uoW.Repository<Hierarchy>();
            var existing = await repo.GetById(id);
            if (existing == null) { return NotFound(); }

            uoW.Begin();
            try
            {
                await repo.Delete(id);
                uoW.Commit();
                return Content("Company Hierarchy Deleted!");
            }
            catch (Exception ex)
            {
                uoW.Rollback();
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        private async Task<HierListDto> MapToDto(Hierarchy b, bool e)
        {
            var repo = uoW.Repository<Company>();
            var parent = await repo.GetById(b.ParentId);
            var child = await repo.GetById(b.ChildId);

            return new HierListDto
            {
                Id = b.Id,
                ParentId = b.ParentId,
                ChildId = b.ChildId,
                Parent = parent?.Name ?? "",
                Child = child?.NameAm ?? "",
                CreatedAt = b.CreatedAt,
                CreatedAtAm = b.CreatedAtAm,
                ModifiedAt = b.ModifiedAt,
                ModifiedAtAm = b.ModifiedAtAm,
                RowVersion = e ? Convert.ToBase64String(b.RowVersion) : ""
            };
        }
    }
}
