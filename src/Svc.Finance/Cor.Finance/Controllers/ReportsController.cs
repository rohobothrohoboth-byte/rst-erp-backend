using Common;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

using Cor.Finance.Persistence;

using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ReportsController : BaseApiController
{
    public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// Get Income Statement
    /// </summary>
    [HttpGet("IncomeStatement")]
    [PerAuth("fnm.reports.view")]
    [ProducesResponseType(typeof(IncomeStatementDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIncomeStatement(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? branchId)
    {
        try
        {
            // Validate date range
            if (!IsValidDateRange(startDate, endDate))
            {
                return HandleBadRequest("Start date must be less than or equal to end date");
            }

            // Ensure dates are UTC
            var startDateUtc = EnsureUtc(startDate);
            var endDateUtc = EnsureUtc(endDate);

            var result = await Mediator.Send(new GetIncomeStatementQry
            {
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                BranchId = branchId
            });

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetIncomeStatement");
        }
    }

    /// <summary>
    /// Get Balance Sheet
    /// </summary>
    [HttpGet("BalanceSheet")]
    [PerAuth("fnm.reports.view")]
    [ProducesResponseType(typeof(BalanceSheetDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalanceSheet(
        [FromQuery] DateTime asOfDate,
        [FromQuery] Guid? branchId)
    {
        try
        {
            // Ensure date is UTC
            var asOfDateUtc = EnsureUtc(asOfDate);

            var result = await Mediator.Send(new GetBalanceSheetQry
            {
                AsOfDate = asOfDateUtc,
                BranchId = branchId
            });

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetBalanceSheet");
        }
    }

    /// <summary>
    /// Get Cash Flow Statement
    /// </summary>
    [HttpGet("CashFlow")]
    [PerAuth("fnm.reports.view")]
    [ProducesResponseType(typeof(CashFlowStatementDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCashFlow(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? branchId)
    {
        try
        {
            // Validate date range
            if (!IsValidDateRange(startDate, endDate))
            {
                return HandleBadRequest("Start date must be less than or equal to end date");
            }

            // Ensure dates are UTC
            var startDateUtc = EnsureUtc(startDate);
            var endDateUtc = EnsureUtc(endDate);

            var result = await Mediator.Send(new GetCashFlowStatementQry
            {
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                BranchId = branchId
            });

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetCashFlow");
        }
    }

    /// <summary>
    /// Get Expense Report
    /// </summary>
    [HttpGet("ExpenseReport")]
    [PerAuth("fnm.reports.view")]
    [ProducesResponseType(typeof(ExpenseReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenseReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? branchId,
        [FromQuery] Guid? departmentId)
    {
        try
        {
            // Validate date range
            if (!IsValidDateRange(startDate, endDate))
            {
                return HandleBadRequest("Start date must be less than or equal to end date");
            }

            // Ensure dates are UTC
            var startDateUtc = EnsureUtc(startDate);
            var endDateUtc = EnsureUtc(endDate);

            var result = await Mediator.Send(new GetExpenseReportQry
            {
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                BranchId = branchId,
                DepartmentId = departmentId
            });

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetExpenseReport");
        }
    }

    /// <summary>
    /// Get Budget vs Actual
    /// </summary>
    [HttpGet("BudgetVsActual")]
    [PerAuth("fnm.reports.view")]
    [ProducesResponseType(typeof(BudgetVsActualDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBudgetVsActual(
        [FromQuery] Guid budgetId,
        [FromQuery] DateTime? actualEndDate)
    {
        try
        {
            // Ensure date is UTC if provided
            var actualEndDateUtc = EnsureUtc(actualEndDate);

            var result = await Mediator.Send(new GetBudgetVsActualQry
            {
                BudgetId = budgetId,
                ActualEndDate = actualEndDateUtc
            });

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetBudgetVsActual");
        }
    }

    /// <summary>
    /// Get Trial Balance
    /// </summary>
    [HttpGet("TrialBalance")]
    [PerAuth("fnm.reports.view")]
    [ProducesResponseType(typeof(TrialBalanceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrialBalance(
        [FromQuery] DateTime asOfDate,
        [FromQuery] Guid? branchId)
    {
        try
        {
            // Ensure date is UTC
            var asOfDateUtc = EnsureUtc(asOfDate);

            var result = await Mediator.Send(new GetTrialBalanceQry
            {
                AsOfDate = asOfDateUtc,
                BranchId = branchId
            });

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetTrialBalance");
        }
    }

    /// <summary>
    /// Get General Ledger
    /// </summary>
    [HttpGet("GeneralLedger")]
    [PerAuth("fnm.reports.view")]
    [ProducesResponseType(typeof(GeneralLedgerDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGeneralLedger(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? accountId,
        [FromQuery] Guid? branchId)
    {
        try
        {
            // Validate date range
            if (!IsValidDateRange(startDate, endDate))
            {
                return HandleBadRequest("Start date must be less than or equal to end date");
            }

            // Ensure dates are UTC
            var startDateUtc = EnsureUtc(startDate);
            var endDateUtc = EnsureUtc(endDate);

            var result = await Mediator.Send(new GetGeneralLedgerQry
            {
                StartDate = startDateUtc,
                EndDate = endDateUtc,
                AccountId = accountId,
                BranchId = branchId
            });

            return SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetGeneralLedger");
        }
    }
}