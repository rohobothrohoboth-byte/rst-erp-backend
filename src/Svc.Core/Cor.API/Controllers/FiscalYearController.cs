using Asp.Versioning;
using Cor.App.Interfaces;
using Cor.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.Api.Cor.DTOs;
using System.Data;
using YesNo = Cor.Domain.Entities.YesNo;

namespace Cor.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class FiscalYearController(IUnitOfWork uoW) : ControllerBase
    {
        [HttpGet(Name = "AllFiscYears")]
        public async Task<ActionResult<IEnumerable<FiscalYear>>> GetAll()
        {
            var fYears = await uoW.Repository<FiscalYear>().GetAll();
            var allFiscYears = fYears.Select(fy => MapToDto(fy, true)).ToList();
            return Ok(allFiscYears);
        }

        [HttpGet("{id:guid}", Name = "GetFiscYear")]
        public async Task<ActionResult<FiscalYear>> GetById(Guid id)
        {
            var fy = await uoW.Repository<FiscalYear>().GetById(id);
            if (fy == null) return NotFound();
            return Ok(MapToDto(fy, true));
        }

        [HttpPost(Name = "AddFiscYear")]
        public async Task<ActionResult<FiscalYear>> Create(AddFiscYearDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var fy = new FiscalYear
            {
                Name = dto.Name,
                DateStart = dto.DateStart,
                DateEnd = dto.DateEnd,
                IsActive = (YesNo)dto.IsActive
            };

            await uoW.Repository<FiscalYear>().Add(fy);
            return CreatedAtAction(nameof(GetById), new { id = fy.Id }, MapToDto(fy, false));
        }

        [HttpPut("{id:guid}", Name = "EditFiscYear")]
        public async Task<ActionResult<FiscalYear>> Update(Guid id, EditFiscYearDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var repo = uoW.Repository<FiscalYear>();
            var oldFy = await repo.GetById(id);

            if (oldFy == null) { return NotFound(); }

            oldFy.Name = dto.Name;
            oldFy.DateStart = dto.DateStart;
            oldFy.DateEnd = dto.DateEnd;
            oldFy.IsActive = (YesNo)dto.IsActive;

            try
            {
                oldFy.RowVersion = Convert.FromBase64String(dto.RowVersion);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid RowVersion format.");
            }

            uoW.Begin();
            try
            {
                var updated = await repo.Update(oldFy);
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

        [HttpDelete("{id:guid}", Name = "RemoveFiscYear")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var repo = uoW.Repository<FiscalYear>();
            var existing = await repo.GetById(id);
            if (existing == null) { return NotFound(); }

            uoW.Begin();
            try
            {
                await repo.Delete(id);
                uoW.Commit();
                return Content("Fiscal Year Deleted!");
            }
            catch (Exception ex)
            {
                uoW.Rollback();
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        private static FiscYearListDto MapToDto(FiscalYear b, bool e)
        {
            return new FiscYearListDto
            {
                Id = b.Id,
                Name = b.Name,
                IsActive = b.IsActive.ToString()!,
                StartDate = b.StartDate,
                StartDateAm = b.StartDateAm,
                EndDate = b.EndDate,
                EndDateAm = b.EndDateAm,
                CreatedAt = b.CreatedAt,
                CreatedAtAm = b.CreatedAtAm,
                ModifiedAt = b.ModifiedAt,
                ModifiedAtAm = b.ModifiedAtAm,
                RowVersion = e ? Convert.ToBase64String(b.RowVersion) : ""
            };
        }
    }
}
