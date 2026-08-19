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
        : base(mediator, logger) { }

    [HttpGet("GeneralLedger")]
    [PerAuth("fnm.gl.journal.view")]
    public async Task<IActionResult> GetGeneralLedger([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] Guid? periodId = null, [FromQuery] Guid? accountId = null, [FromQuery] Guid? branchId = null)
    {
        if (!periodId.HasValue && (!startDate.HasValue || !endDate.HasValue)) return HandleBadRequest("Provide periodId or both startDate and endDate.");
        if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value) return HandleBadRequest("EndDate must be greater than or equal to StartDate.");
        try { return Ok(await Mediator.Send(new GetGeneralLedgerQry { StartDate = startDate, EndDate = endDate, PeriodId = periodId, AccountId = accountId, BranchId = branchId })); }
        catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
        catch (ArgumentException ex) { return HandleBadRequest(ex.Message); }
        catch (Exception ex) { return HandleException(ex, "GetGeneralLedger"); }
    }

    [HttpGet("TrialBalance")]
    [PerAuth("fnm.gl.journal.view")]
    public async Task<IActionResult> GetTrialBalance([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] DateTime? asOfDate = null, [FromQuery] Guid? periodId = null, [FromQuery] Guid? branchId = null, [FromQuery] bool includeZeroBalances = false)
    {
        try { return Ok(await Mediator.Send(new GetTrialBalanceQry { StartDate = startDate, EndDate = endDate, AsOfDate = asOfDate, BranchId = branchId, PeriodId = periodId, IncludeZeroBalances = includeZeroBalances })); }
        catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
        catch (ArgumentException ex) { return HandleBadRequest(ex.Message); }
        catch (Exception ex) { return HandleException(ex, "GetTrialBalance"); }
    }
}
