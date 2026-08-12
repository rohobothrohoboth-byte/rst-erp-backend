// Cor.Finance/Controllers/AccountTypeController.cs
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
public class AccountTypeController : BaseApiController
{
    private readonly ILogger<AccountTypeController> _logger;

    public AccountTypeController(
        IMediator mediator,
        ILogger<AccountTypeController> logger)
        : base(mediator, logger)
    {
        _logger = logger;
    }

    // ============================================================
    // ACCOUNT TYPE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all account types with pagination and filtering
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(PaginatedResponse<AccountTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTypes(
        [FromQuery] bool? isActive = null,
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "Code",
        [FromQuery] string? sortOrder = "ASC")
    {
        try
        {
            var result = await Mediator.Send(new GetAllAccountTypesQry
            {
                IsActive = isActive,
                Category = category,
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
            return HandleException(ex, "GetAllAccountTypes");
        }
    }

    /// <summary>
    /// Get all account types with their subtypes
    /// </summary>
    [HttpGet("types/with-subtypes")]
    [ProducesResponseType(typeof(List<AccountTypeWithSubtypesDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTypesWithSubtypes()
    {
        try
        {
            var result = await Mediator.Send(new GetAllAccountTypesWithSubtypesQry());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllTypesWithSubtypes");
        }
    }

    /// <summary>
    /// Get account type by ID
    /// </summary>
    [HttpGet("types/{id}")]
    [ProducesResponseType(typeof(AccountTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTypeById(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountTypeByIdQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetTypeById", id);
        }
    }

    /// <summary>
    /// Get account type by code
    /// </summary>
    [HttpGet("types/by-code/{code}")]
    [ProducesResponseType(typeof(AccountTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTypeByCode(string code)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountTypeByCodeQry { Code = code });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetTypeByCode", code);
        }
    }

    /// <summary>
    /// Get account types by category
    /// </summary>
    [HttpGet("types/by-category/{category}")]
    [ProducesResponseType(typeof(List<AccountTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTypesByCategory(string category)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountTypesByCategoryQry { Category = category });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetTypesByCategory", category);
        }
    }

    /// <summary>
    /// Get account type with its subtypes
    /// </summary>
    [HttpGet("types/{id}/with-subtypes")]
    [ProducesResponseType(typeof(AccountTypeWithSubtypesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTypeWithSubtypes(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountTypeWithSubtypesQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetTypeWithSubtypes", id);
        }
    }

    /// <summary>
    /// Create a new account type
    /// </summary>
    [HttpPost("types")]
    [ProducesResponseType(typeof(AccountTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateType([FromBody] CreateAccountTypeDto dto)
    {
        try
        {
            var result = await Mediator.Send(new CreateAccountTypeCmd { Dto = dto });
            return CreatedAtAction(nameof(GetTypeById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateAccountType");
        }
    }

    /// <summary>
    /// Update an account type
    /// </summary>
    [HttpPut("types")]
    [ProducesResponseType(typeof(AccountTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateType([FromBody] UpdateAccountTypeDto dto)
    {
        try
        {
            var result = await Mediator.Send(new UpdateAccountTypeCmd { Dto = dto });
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
            return HandleException(ex, "UpdateAccountType", dto.Id);
        }
    }

    /// <summary>
    /// Delete an account type
    /// </summary>
    [HttpDelete("types/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteType(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new DeleteAccountTypeCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Account Type with ID '{id}' not found" });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteAccountType", id);
        }
    }

    /// <summary>
    /// Toggle account type active status
    /// </summary>
    [HttpPatch("types/{id}/toggle-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleTypeActive(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new ToggleAccountTypeActiveCmd { Id = id });
            return Ok(new
            {
                success = true,
                message = "Account Type status toggled successfully",
                isActive = result.IsActive
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleTypeActive", id);
        }
    }

    // ============================================================
    // ACCOUNT SUBTYPE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all account subtypes with pagination and filtering
    /// </summary>
    [HttpGet("subtypes")]
    [ProducesResponseType(typeof(PaginatedResponse<AccountSubtypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSubtypes(
        [FromQuery] bool? isActive = null,
        [FromQuery] Guid? accountTypeId = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "Code",
        [FromQuery] string? sortOrder = "ASC")
    {
        try
        {
            var result = await Mediator.Send(new GetAllAccountSubtypesQry
            {
                IsActive = isActive,
                AccountTypeId = accountTypeId,
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
            return HandleException(ex, "GetAllSubtypes");
        }
    }

    /// <summary>
    /// Get subtypes by account type ID
    /// </summary>
    [HttpGet("subtypes/by-type/{typeId}")]
    [ProducesResponseType(typeof(List<AccountSubtypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubtypesByTypeId(Guid typeId)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountSubtypesByTypeIdQry { AccountTypeId = typeId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetSubtypesByTypeId", typeId);
        }
    }

    /// <summary>
    /// Get subtypes by account type code
    /// </summary>
    [HttpGet("subtypes/by-type-code/{typeCode}")]
    [ProducesResponseType(typeof(List<AccountSubtypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubtypesByTypeCode(string typeCode)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountSubtypesByTypeCodeQry { TypeCode = typeCode });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetSubtypesByTypeCode", typeCode);
        }
    }

    /// <summary>
    /// Get subtype by ID
    /// </summary>
    [HttpGet("subtypes/{id}")]
    [ProducesResponseType(typeof(AccountSubtypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubtypeById(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetAccountSubtypeByIdQry { Id = id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetSubtypeById", id);
        }
    }

    /// <summary>
    /// Create a new account subtype
    /// </summary>
    [HttpPost("subtypes")]
    [ProducesResponseType(typeof(AccountSubtypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubtype([FromBody] CreateAccountSubtypeDto dto)
    {
        try
        {
            var result = await Mediator.Send(new CreateAccountSubtypeCmd { Dto = dto });
            return CreatedAtAction(nameof(GetSubtypeById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateAccountSubtype");
        }
    }

    /// <summary>
    /// Update an account subtype
    /// </summary>
    [HttpPut("subtypes")]
    [ProducesResponseType(typeof(AccountSubtypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubtype([FromBody] UpdateAccountSubtypeDto dto)
    {
        try
        {
            var result = await Mediator.Send(new UpdateAccountSubtypeCmd { Dto = dto });
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
            return HandleException(ex, "UpdateAccountSubtype", dto.Id);
        }
    }

    /// <summary>
    /// Delete an account subtype
    /// </summary>
    [HttpDelete("subtypes/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubtype(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new DeleteAccountSubtypeCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Account Subtype with ID '{id}' not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteAccountSubtype", id);
        }
    }

    /// <summary>
    /// Toggle account subtype active status
    /// </summary>
    [HttpPatch("subtypes/{id}/toggle-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleSubtypeActive(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new ToggleAccountSubtypeActiveCmd { Id = id });
            return Ok(new
            {
                success = true,
                message = "Account Subtype status toggled successfully",
                isActive = result.IsActive
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleSubtypeActive", id);
        }
    }
}