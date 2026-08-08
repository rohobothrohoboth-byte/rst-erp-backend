// Controllers/ChartOfAccountsController.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using Cor.Finance.Commands;
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
public class ChartOfAccountsController : BaseApiController
{
    private readonly CachedReferenceDataService _cachedService;
    private readonly ILogger<ChartOfAccountsController> _logger;

    public ChartOfAccountsController(
        IMediator mediator,
        ILogger<ChartOfAccountsController> logger,
        CachedReferenceDataService cachedService)
        : base(mediator, logger)
    {
        _logger = logger;
        _cachedService = cachedService;
    }

    // ============================================================
    // GET ENDPOINTS - SPECIFIC ROUTES MUST COME FIRST
    // ============================================================

   // Controllers/ChartOfAccountsController.cs

   /// <summary>
   /// Get all chart of accounts with pagination and filtering
   /// </summary>
   [HttpGet]
   [ProducesResponseType(typeof(PaginatedResponse<ChartOfAccountsDto>), StatusCodes.Status200OK)]
   // ✅ Remove or disable ResponseCache during development
   // [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any,
   //     VaryByQueryKeys = new[] { "isActive", "type", "search", "page", "pageSize", "sortBy", "sortOrder" })]
   public async Task<IActionResult> GetAll(
       [FromQuery] bool? isActive = null,
       [FromQuery] string? type = null,
       [FromQuery] string? search = null,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20,
       [FromQuery] string? sortBy = "Code",
       [FromQuery] string? sortOrder = "ASC")
   {
       try
       {
           if (pageSize > 100) pageSize = 100;
           if (page < 1) page = 1;

           // ✅ USE THE MEDIATR QUERY INSTEAD OF CACHE
           var result = await Mediator.Send(new GetAllChartOfAccountsQry
           {
               IsActive = isActive,
               Type = type,
               Search = search,
               Page = page,
               PageSize = pageSize,
               SortBy = sortBy,
               SortOrder = sortOrder
           });

           _logger.LogInformation("✅ Retrieved {Count} chart of accounts (Page {Page}/{TotalPages})",
               result.Data?.Count ?? 0, result.Page, result.TotalPages);

           return Ok(result);
       }
       catch (Exception ex)
       {
           return HandleException(ex, "GetAllChartOfAccounts");
       }
   }

    /// <summary>
    /// Get chart of accounts hierarchy (CACHED)
    /// </summary>
    /// Get chart of accounts hierarchy (CACHED)
       /// </summary>
       [HttpGet("Hierarchy")]
       [ProducesResponseType(typeof(List<ChartOfAccountsHierarchyDto>), StatusCodes.Status200OK)]
       [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
       public async Task<IActionResult> GetHierarchy()
       {
           try
           {
               var accounts = await _cachedService.GetCachedAccountsAsync();
               var hierarchy = BuildHierarchy(accounts, null);

               _logger.LogInformation("✅ Retrieved chart of accounts hierarchy from cache ({Count} root nodes)", hierarchy.Count);
               return Ok(hierarchy);
           }
           catch (Exception ex)
           {
               return HandleException(ex, "GetChartOfAccountsHierarchy");
           }
       }

       /// <summary>
       /// Build hierarchy tree from flat list of accounts
       /// </summary>
       private List<ChartOfAccountsHierarchyDto> BuildHierarchy(List<ChartOfAccountsDto> accounts, Guid? parentId)
       {
           return accounts
               .Where(x => x.ParentId == parentId)
               .OrderBy(x => x.Code)
               .Select(x => new ChartOfAccountsHierarchyDto
               {
                   Id = x.Id,
                   Code = x.Code ?? string.Empty,
                   Name = x.Name ?? string.Empty,
                   NameAm = x.NameAm,
                   AccountType = x.AccountType,
                   AccountSubType = x.AccountSubType,
                   Description = x.Description,
                   Level = x.Level,
                   IsActive = x.IsActive,
                   ParentId = x.ParentId,
                   OpeningBalance = x.OpeningBalance,
                   CurrentBalance = x.CurrentBalance,
                   OpeningBalanceDate = x.OpeningBalanceDate,
                   CategoryId = x.CategoryId, // ✅ ADD THIS
                   CategoryName = x.CategoryName, // ✅ ADD THIS
                   Children = BuildHierarchy(accounts, x.Id),
                   DateAdd = x.DateAdd,
                   DateMod = x.DateMod
               })
               .ToList();
       }


    /// <summary>
    /// Get chart of accounts by code
    /// </summary>
    [HttpGet("ByCode/{code}")]
    [ProducesResponseType(typeof(ChartOfAccountsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var accounts = await _cachedService.GetCachedAccountsAsync();
            var account = accounts.FirstOrDefault(x => x.Code == code);

            if (account == null)
            {
                var result = await Mediator.Send(new GetChartOfAccountsByCodeQry { Code = code });
                return Ok(result);
            }

            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetChartOfAccountsByCode", code);
        }
    }

    /// <summary>
    /// Get chart of accounts by type
    /// </summary>
    [HttpGet("ByType/{accountType}")]
    [ProducesResponseType(typeof(List<ChartOfAccountsDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetByType(string accountType)
    {
        try
        {
            var accounts = await _cachedService.GetCachedAccountsAsync();
            var filtered = accounts.Where(x => x.AccountType == accountType).ToList();

            _logger.LogInformation("✅ Retrieved {Count} chart of accounts of type '{Type}' from cache", filtered.Count, accountType);
            return Ok(filtered);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetChartOfAccountsByType", accountType);
        }
    }

    /// <summary>
    /// Get chart of accounts usage
    /// </summary>
    [HttpGet("{id}/usage")]
    [ProducesResponseType(typeof(ChartOfAccountsUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsage(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetChartOfAccountsUsageQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetChartOfAccountsUsage", id);
        }
    }

    /// <summary>
    /// Check if chart of accounts can be deleted
    /// </summary>
    [HttpGet("{id}/can-delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CanDelete(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new CanDeleteChartOfAccountsQry { Id = id });
            return Ok(new
            {
                canDelete = result.CanDelete,
                reason = result.Reason,
                count = result.ChartOfAccountsCount
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CanDeleteChartOfAccounts", id);
        }
    }

    /// <summary>
    /// Get chart of accounts by ID (MUST BE LAST)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ChartOfAccountsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var accounts = await _cachedService.GetCachedAccountsAsync();
            var account = accounts.FirstOrDefault(x => x.Id == id);

            if (account == null)
            {
                var result = await Mediator.Send(new GetChartOfAccountsByIdQry { Id = id });
                return Ok(result);
            }

            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetChartOfAccountsById", id);
        }
    }

    /// <summary>
    /// Export chart of accounts
    /// </summary>
    [HttpGet("Export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromQuery] string? type = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string format = "csv")
    {
        try
        {
            var accounts = await _cachedService.GetCachedAccountsAsync();

            if (isActive.HasValue)
                accounts = accounts.Where(x => x.IsActive == isActive.Value).ToList();

            if (!string.IsNullOrEmpty(type))
                accounts = accounts.Where(x => x.AccountType == type).ToList();

            var accountDtos = accounts.Select(a => new ChartOfAccountsDto
            {
                Id = a.Id,
                Code = a.Code ?? string.Empty,
                Name = a.Name ?? string.Empty,
                NameAm = a.NameAm,
                Description = a.Description,
                AccountType = a.AccountType ?? string.Empty,
                AccountSubType = a.AccountSubType,
                IsActive = a.IsActive,
                ParentId = a.ParentId,
                Level = a.Level,
                OpeningBalance = a.OpeningBalance,
                CurrentBalance = a.CurrentBalance,
                OpeningBalanceDate = a.OpeningBalanceDate,
                PeriodId = a.PeriodId,
                DateAdd = a.DateAdd,
                DateMod = a.DateMod
            }).ToList();

            if (format.ToLower() == "csv")
            {
                var csv = ConvertToCsv(accountDtos);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"chart-of-accounts-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            else if (format.ToLower() == "json")
            {
                var json = System.Text.Json.JsonSerializer.Serialize(accountDtos,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return File(bytes, "application/json", $"chart-of-accounts-{DateTime.UtcNow:yyyyMMdd}.json");
            }

            return BadRequest(new { message = "Unsupported format. Use 'csv' or 'json'." });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ExportChartOfAccounts");
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new chart of accounts
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ChartOfAccountsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddChartOfAccountsDto dto)
    {
        try
        {
            var result = await Mediator.Send(new AddChartOfAccountsCmd { AddDto = dto });
            await _cachedService.InvalidateAccountsCacheAsync();

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateChartOfAccounts");
        }
    }

    /// <summary>
    /// Bulk create chart of accounts
    /// </summary>
    [HttpPost("Bulk")]
    [ProducesResponseType(typeof(List<ChartOfAccountsDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] List<AddChartOfAccountsDto> dtos)
    {
        try
        {
            var result = await Mediator.Send(new BulkAddChartOfAccountsCmd { AddDtos = dtos });
            await _cachedService.InvalidateAccountsCacheAsync();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "BulkCreateChartOfAccounts");
        }
    }

    /// <summary>
    /// Bulk delete chart of accounts
    /// </summary>
    [HttpPost("BulkDelete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteDto dto)
    {
        try
        {
            var result = await Mediator.Send(new BulkDeleteChartOfAccountsCmd { Ids = dto.Ids });
            await _cachedService.InvalidateAccountsCacheAsync();

            return Ok(new
            {
                success = true,
                deleted = result.DeletedCount,
                failed = result.FailedCount,
                errors = result.Errors
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "BulkDeleteChartOfAccounts");
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a chart of accounts
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ChartOfAccountsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditChartOfAccountsDto dto)
    {
        try
        {
            var result = await Mediator.Send(new EditChartOfAccountsCmd { EditDto = dto });
            await _cachedService.InvalidateAccountsCacheAsync();

            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateChartOfAccounts", dto.Id);
        }
    }

    // ============================================================
    // PATCH ENDPOINTS
    // ============================================================

    /// <summary>
    /// Toggle chart of accounts active status
    /// </summary>
    [HttpPatch("{id}/toggle-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new ToggleChartOfAccountsActiveCmd { Id = id });
            await _cachedService.InvalidateAccountsCacheAsync();

            return Ok(new
            {
                success = true,
                message = "Chart of Accounts status toggled successfully",
                isActive = result.IsActive
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleChartOfAccountsActive", id);
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete a chart of accounts
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new DeleteChartOfAccountsCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Chart of Accounts with ID '{id}' not found" });

            await _cachedService.InvalidateAccountsCacheAsync();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteChartOfAccounts", id);
        }
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================



    private string ConvertToCsv(List<ChartOfAccountsDto> accounts)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Code,Name,NameAm,Type,SubType,Level,IsActive,ParentId,OpeningBalance,CurrentBalance,CreatedDate,ModifiedDate");

        foreach (var acc in accounts)
        {
            sb.AppendLine($"{acc.Id},{acc.Code},{acc.Name},{acc.NameAm},{acc.AccountType},{acc.AccountSubType},{acc.Level},{acc.IsActive},{acc.ParentId},{acc.OpeningBalance},{acc.CurrentBalance},{acc.DateAdd:yyyy-MM-dd HH:mm:ss},{acc.DateMod:yyyy-MM-dd HH:mm:ss}");
        }

        return sb.ToString();
    }
}