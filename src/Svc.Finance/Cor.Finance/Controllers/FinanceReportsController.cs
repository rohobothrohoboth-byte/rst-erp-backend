using Common;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Shared.Helpers.Services;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class FinanceReportsController : BaseApiController
{
    public FinanceReportsController(IMediator mediator, ILogger<FinanceReportsController> logger)
        : base(mediator, logger)
    {
    }

    [HttpGet("GeneralLedger")]
    [PerAuth("fnm.gl.journal.view")]
    public async Task<IActionResult> GetGeneralLedger(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? accountId = null,
        [FromQuery] Guid? branchId = null)
    {
        if (endDate < startDate)
            return HandleBadRequest("EndDate must be greater than or equal to StartDate");

        try
        {
            var result = await Mediator.Send(new GetGeneralLedgerQry
            {
                StartDate = startDate,
                EndDate = endDate,
                AccountId = accountId,
                BranchId = branchId
            });

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetGeneralLedger");
        }
    }

    [HttpGet("TrialBalance")]
    [PerAuth("fnm.gl.journal.view")]
    public async Task<IActionResult> GetTrialBalance(
        [FromQuery] DateTime asOfDate,
        [FromQuery] Guid? branchId = null)
    {
        try
        {
            var result = await Mediator.Send(new GetTrialBalanceQry
            {
                AsOfDate = asOfDate,
                BranchId = branchId
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetTrialBalance");
        }
    }
}
