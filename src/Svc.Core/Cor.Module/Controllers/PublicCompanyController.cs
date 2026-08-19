// Controllers/Public/PublicCompanyController.cs
using Asp.Versioning;
using Common;
using Cor.Module.Commands;
using Cor.Module.Models.DTOs;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourProject.Controllers.Public
{
    [ApiController]
    [Route("api/public/[controller]")]
    [AllowAnonymous] // ✅ No authentication required
    public class CompanyController : ControllerBase
    {
        private readonly IMediator _med;

        public CompanyController(IMediator med)
        {
            _med = med;
        }

        /// <summary>
        /// Get public company information for login page (no authentication required)
        /// </summary>
        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicCompanyInfo()
        {
            var result = await _med.Send(new GetPublicCompanyInfoQry());

            if (result == null)
            {
                return NotFound(ApiResponse<object>.Fail("No company found"));
            }

            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}