using Asp.Versioning;
using Cor.App.Interfaces;
using Cor.Domain.Entities;
using ERP.Shared.API.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cor.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BranchController(IUnitOfWork uoW) : ControllerBase
    {
        [HttpGet(Name = "AllBranches")]
        public async Task<ActionResult<IEnumerable<Branch>>> GetAll()
        {
            var bras = await uoW.Repository<Branch>().GetAll();
            var allBras = new List<BranchListDto>();
            foreach (var branch in bras)
            {
                allBras.Add(await MapToDto(branch, true));
            }
            return Ok(allBras);
        }

        [HttpGet("{id:guid}", Name = "GetBranch")]
        public async Task<ActionResult<Branch>> GetById(Guid id)
        {
            var branch = await uoW.Repository<Branch>().GetById(id);
            if (branch == null) return NotFound();
            return Ok(await MapToDto(branch, true));
        }

        [HttpPost(Name = "AddBranch")]
        public async Task<ActionResult<Branch>> Create(AddBranchDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var branch = new Branch
            {
                Name = dto.Name,
                NameAm = dto.NameAm,
                CompId = dto.CompId
            };

            await uoW.Repository<Branch>().Add(branch);
            return CreatedAtAction(nameof(GetById), new { id = branch.Id }, await MapToDto(branch, false));
        }

        [HttpPut("{id:guid}", Name = "EditBranch")]
        public async Task<ActionResult<Branch>> Update(Guid id, EditBranchDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var repo = uoW.Repository<Branch>();
            var oldBra = await repo.GetById(id);

            if (oldBra == null) { return NotFound(); }

            oldBra.Name = dto.Name;
            oldBra.NameAm = dto.NameAm;
            oldBra.CompId = dto.CompId;

            try
            {
                oldBra.RowVersion = Convert.FromBase64String(dto.RowVersion);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid RowVersion format.");
            }

            uoW.Begin();
            try
            {
                var updated = await repo.Update(oldBra);
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

        [HttpDelete("{id:guid}", Name = "RemoveBranch")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var repo = uoW.Repository<Branch>();
            var existing = await repo.GetById(id);
            if (existing == null) { return NotFound(); }

            uoW.Begin();
            try
            {
                await repo.Delete(id);
                uoW.Commit();
                return Content("Branch Deleted!");
            }
            catch (Exception ex)
            {
                uoW.Rollback();
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        private async Task<BranchListDto> MapToDto(Branch b, bool e)
        {
            var repoComp = uoW.Repository<Company>();
            var comp = await repoComp.GetById(b.CompId);

            return new BranchListDto
            {
                Id = b.Id,
                Name = b.Name,
                NameAm = b.NameAm,
                CompId = b.CompId,
                Comp = comp?.Name ?? "",
                CompAm = comp?.NameAm ?? "",
                CreatedAt = b.CreatedAt,
                CreatedAtAm = b.CreatedAtAm,
                ModifiedAt = b.ModifiedAt,
                ModifiedAtAm = b.ModifiedAtAm,
                RowVersion = e ? Convert.ToBase64String(b.RowVersion) : ""
            };
        }
    }
}
