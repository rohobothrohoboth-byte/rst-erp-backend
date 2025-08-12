using Asp.Versioning;
using Cor.App.Interfaces;
using Cor.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ERP.Shared.API.Core.DTOs;

namespace Cor.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/company")]
    [ApiVersion("1.0")]
    public class CompanyController(IUnitOfWork uoW) : ControllerBase
    {
        [HttpGet(Name = "AllCompanies")]
        public async Task<ActionResult<IEnumerable<Company>>> GetAll()
        {
            var comps = await uoW.Repository<Company>().GetAll();
            var allComps = comps.Select(com => MapToDto(com, true)).ToList();
            return Ok(allComps);
        }

        [HttpGet("{id:guid}", Name = "GetCompany")]
        public async Task<ActionResult<Company>> GetById(Guid id)
        {
            var comp = await uoW.Repository<Company>().GetById(id);
            if (comp == null) return NotFound();
            return Ok(MapToDto(comp, true));
        }

        [HttpPost(Name = "AddCompany")]
        public async Task<ActionResult<Company>> Create(AddCompDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var comp = new Company
            {
                Name = dto.Name,
                NameAm = dto.NameAm
            };

            await uoW.Repository<Company>().Add(comp);
            return CreatedAtAction(nameof(GetById), new { id = comp.Id }, MapToDto(comp, false));
        }

        [HttpPut("{id:guid}", Name = "EditCompany")]
        public async Task<ActionResult<Company>> Update(Guid id, EditCompDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var repo = uoW.Repository<Company>();
            var oldComp = await repo.GetById(id);

            if (oldComp == null) { return NotFound(); }

            oldComp.Name = dto.Name;
            oldComp.NameAm = dto.NameAm;

            try
            {
                oldComp.RowVersion = Convert.FromBase64String(dto.RowVersion);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid RowVersion format.");
            }

            uoW.Begin();
            try
            {
                var updated = await repo.Update(oldComp);
                uoW.Commit();
                return Ok(MapToDto(updated, false));
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

        [HttpDelete("{id:guid}", Name = "RemoveCompany")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var repo = uoW.Repository<Company>();
            var existing = await repo.GetById(id);
            if (existing == null) { return NotFound(); }

            uoW.Begin();
            try
            {
                await repo.Delete(id);
                uoW.Commit();
                return Content("Company Deleted!");
            }
            catch (Exception ex)
            {
                uoW.Rollback();
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        private static CompListDto MapToDto(Company b, bool e)
        {
            return new CompListDto
            {
                Id = b.Id,
                Name = b.Name,
                NameAm = b.NameAm,
                CreatedAt = b.CreatedAt,
                CreatedAtAm = b.CreatedAtAm,
                ModifiedAt = b.ModifiedAt,
                ModifiedAtAm = b.ModifiedAtAm,
                RowVersion = e ? Convert.ToBase64String(b.RowVersion) : ""
            };
        }
    }
}
