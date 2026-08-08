// Controllers/AccountCategoryController.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using Cor.Finance.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class AccountCategoryController : BaseApiController
{
    private readonly ILogger<AccountCategoryController> _logger;

    public AccountCategoryController(
        IMediator mediator,
        ILogger<AccountCategoryController> logger)
        : base(mediator, logger)
    {
        _logger = logger;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all account categories with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<AccountCategoryDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any,
        VaryByQueryKeys = new[] { "isActive", "type", "search", "page", "pageSize", "sortBy", "sortOrder" })]
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

            // ✅ Use await properly
            var result = await Mediator.Send(new GetAllAccountCategoriesQry
            {
                IsActive = isActive,
                Type = type,
                Search = search,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllAccountCategories");
        }
    }

    /// <summary>
    /// Get account category hierarchy
    /// </summary>
    [HttpGet("Hierarchy")]
    [ProducesResponseType(typeof(List<AccountCategoryHierarchyDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetHierarchy()
    {
        try
        {
            var result = await Mediator.Send(new GetAccountCategoryHierarchyQry());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAccountCategoryHierarchy");
        }
    }

    /// <summary>
    /// Get account category by code
    /// </summary>
    [HttpGet("ByCode/{code}")]
    [ProducesResponseType(typeof(AccountCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountCategoryByCodeQry { Code = code });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAccountCategoryByCode", code);
        }
    }

    /// <summary>
    /// Get account categories by type
    /// </summary>
    [HttpGet("ByType/{type}")]
    [ProducesResponseType(typeof(List<AccountCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByType(string type)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountCategoryByTypeQry { Type = type });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAccountCategoryByType", type);
        }
    }

    /// <summary>
    /// Get category usage
    /// </summary>
    [HttpGet("{id}/usage")]
    [ProducesResponseType(typeof(CategoryUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsage(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetCategoryUsageQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetCategoryUsage", id);
        }
    }

    /// <summary>
    /// Check if account category can be deleted
    /// </summary>
    [HttpGet("{id}/can-delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CanDelete(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new CanDeleteAccountCategoryQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CanDeleteAccountCategory", id);
        }
    }

    /// <summary>
    /// Get account category by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AccountCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountCategoryByIdQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAccountCategoryById", id);
        }
    }

    /// <summary>
    /// Export account categories
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
            var categories = await Mediator.Send(new ExportAccountCategoriesQry
            {
                Type = type,
                IsActive = isActive
            });

            if (format.ToLower() == "csv")
            {
                var csv = ConvertToCsv(categories);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"account-categories-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            else if (format.ToLower() == "json")
            {
                var json = System.Text.Json.JsonSerializer.Serialize(categories,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return File(bytes, "application/json", $"account-categories-{DateTime.UtcNow:yyyyMMdd}.json");
            }

            return BadRequest(new { message = "Unsupported format. Use 'csv' or 'json'." });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ExportAccountCategories");
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new account category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddAccountCategoryDto dto)
    {
        try
        {
            var result = await Mediator.Send(new AddAccountCategoryCmd { AddDto = dto });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateAccountCategory");
        }
    }

    /// <summary>
    /// Bulk create account categories
    /// </summary>
    [HttpPost("Bulk")]
    [ProducesResponseType(typeof(List<AccountCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] List<AddAccountCategoryDto> dtos)
    {
        try
        {
            var result = await Mediator.Send(new BulkAddAccountCategoryCmd { AddDtos = dtos });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "BulkCreateAccountCategories");
        }
    }

    /// <summary>
    /// Bulk delete account categories
    /// </summary>
    [HttpPost("BulkDelete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteDto dto)
    {
        try
        {
            var result = await Mediator.Send(new BulkDeleteAccountCategoryCmd { Ids = dto.Ids });
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
            return HandleException(ex, "BulkDeleteAccountCategories");
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update an account category
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(AccountCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditAccountCategoryDto dto)
    {
        try
        {
            var result = await Mediator.Send(new EditAccountCategoryCmd { EditDto = dto });
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
            return HandleException(ex, "UpdateAccountCategory", dto.Id);
        }
    }

    // ============================================================
    // PATCH ENDPOINTS
    // ============================================================

    /// <summary>
    /// Toggle account category active status
    /// </summary>
    [HttpPatch("{id}/toggle-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new ToggleAccountCategoryActiveCmd { Id = id });
            return Ok(new
            {
                success = true,
                message = "Account Category status toggled successfully",
                isActive = result.IsActive
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleAccountCategoryActive", id);
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete an account category
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new DeleteAccountCategoryCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Account Category with ID '{id}' not found" });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteAccountCategory", id);
        }
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private string ConvertToCsv(List<AccountCategoryDto> categories)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Code,Name,NameAm,Type,Description,IsActive,ParentId,CreatedDate,ModifiedDate");

        foreach (var cat in categories)
        {
            sb.AppendLine($"{cat.Id},{cat.Code},{cat.Name},{cat.NameAm},{cat.Type},{cat.Description},{cat.IsActive},{cat.ParentId},{cat.DateAdd:yyyy-MM-dd HH:mm:ss},{cat.DateMod:yyyy-MM-dd HH:mm:ss}");
        }

        return sb.ToString();
    }
}