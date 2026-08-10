using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Queries;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Commands;
using Dapper;
using Svc.Auth.Interfaces;



namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/auth/v{version:apiVersion}/Permission")]
[ApiVersion("1.0")]
public class PermissionController : ControllerBase
{
    private readonly IMediator _med;
    private readonly IDapperHelper _dapper;

    public PermissionController(IMediator med, IDapperHelper dapper)
    {
        _med = med;
        _dapper = dapper;
    }

    // ==================== ROLE ENDPOINTS ====================

    [HttpGet("AllRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllRole()
    {
        var response = await _med.Send(new RoleAllQry());
        return Ok(response);
    }

    [HttpGet("GetRole/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRole(string id)
    {
        var response = await _med.Send(new RoleByIdQry { Id = id });
        if (response == null) { throw new DomainException($"ROLE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ==================== MODULE ENDPOINTS ====================

    [HttpGet("AllModule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllModule()
    {
        var response = await _med.Send(new ModuleAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetModule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModule(Guid id)
    {
        var response = await _med.Send(new ModuleByIdQry { Id = id });
        if (response == null) { throw new DomainException($"MODULE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("AllPerModule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPerModule()
    {
        var response = await _med.Send(new PerModuleAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPerModule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerModule(Guid id)
    {
        var response = await _med.Send(new PerModuleByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"MODULE with id [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ==================== NAME LIST ENDPOINTS ====================

    [HttpGet("AllModuleName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllModuleName()
    {
        var res = await _med.Send(new ModuleNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetModuleName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModuleName(Guid id)
    {
        var res = await _med.Send(new ModuleNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"MODULE with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllPerMenuName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPerMenuName()
    {
        var res = await _med.Send(new PerMenuNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetPerMenuName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerMenuName(Guid id)
    {
        var res = await _med.Send(new PerMenuNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"MENU PERMISSION with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    [HttpGet("AllPerApiName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPerApiName()
    {
        var res = await _med.Send(new PerApiNameAllQry());
        return Ok(res);
    }

    [HttpGet("GetPerApiName/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerApiName(Guid id)
    {
        var res = await _med.Send(new PerApiNameByIdQry { Id = id });
        if (res == null) { throw new DomainException($"ACCESS PERMISSION with id [{id}] NOT FOUND."); }
        return Ok(res);
    }

    // ==================== MENU ENDPOINTS ====================

    [HttpGet("GetMenuTree")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMenuTree()
    {
        var response = await _med.Send(new GetMenuTreeQry());
        return Ok(ApiResponse<object>.Ok(response));
    }
 [HttpGet("AllPerMenu")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<IActionResult> AllPerMenu()
   {
       var response = await _med.Send(new PerMenuAllQry());

       // DEBUG: Log the first few items
       var myLeave = response.FirstOrDefault(x => x.Key == "my.leave");
       if (myLeave != null)
       {

       }

       return Ok(ApiResponse<object>.Ok(response));
   }

    [HttpGet("GetPerMenu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerMenu(Guid id)
    {
        var response = await _med.Send(new PerMenuByIdQry { Id = id });
        if (response == null) { throw new DomainException($"MENU PERMISSION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPerMenuByMod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerMenuByMod(Guid id)
    {
        var response = await _med.Send(new PerMenuByModIdQry { Id = id });
        if (response == null) { throw new DomainException($"MENU PERMISSION with Module id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ==================== API ENDPOINTS ====================


[HttpGet("AllPerApi")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> AllPerApi()
{
    var response = await _med.Send(new PerApiAllQry());
    return Ok(ApiResponse<object>.Ok(response));
}

[HttpGet("GetPerApi/{id:guid}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetPerApi(Guid id)
{
    var response = await _med.Send(new PerApiByIdQry { Id = id });
    if (response == null)
        throw new DomainException($"ACCESS PERMISSION with id [{id}] NOT FOUND.");
    return Ok(ApiResponse<object>.Ok(response));
}

[HttpGet("GetPerApiByMenu/{id:guid}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetPerApiByMenu(Guid id)
{
    var response = await _med.Send(new PerApiByMenuIdQry { Id = id });
    if (response == null)
        throw new DomainException($"ACCESS PERMISSION with Menu id [{id}] NOT FOUND.");
    return Ok(ApiResponse<object>.Ok(response));
}

[HttpPost("AddPerApi")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> AddPerApi([FromBody] PerApiAddDto dto)
{
    var response = await _med.Send(new PerApiAddCmd { AddDto = dto });
    return Ok(ApiResponse<object>.Ok(response, "API permission added successfully."));
}

[HttpPut("ModPerApi/{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> UpdatePerApi(Guid id, [FromBody] PerApiModDto dto)
{
    if (id != dto.Id)
        throw new DomainException("ID mismatch");

    var response = await _med.Send(new PerApiModCmd { ModDto = dto });
    return Ok(ApiResponse<object>.Ok(response, "API permission updated successfully."));
}

[HttpDelete("DelPerApi/{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeletePerApi(Guid id)
{
    await _med.Send(new PerApiDelCmd { Id = id });
    return Ok(ApiResponse<object>.Ok(null, "API permission deleted successfully."));
}


[HttpGet("GetPerMenuByUser/{employeeId}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetPerMenuByUser(string employeeId)
{
    // Log the incoming identifier
    Console.WriteLine($"?? Looking up user with identifier: {employeeId}");

    // First try to find by Id
    const string getAppUserIdSql = @"
        SELECT ""Id"", ""EmployeeId"", ""UserName""
        FROM ""AppUser""
        WHERE ""Id""::text = @EmployeeId";

    var appUser = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
        getAppUserIdSql,
        new { EmployeeId = employeeId }
    );

    // If not found by Id, try by EmployeeId
    if (appUser == null)
    {
        const string getByEmployeeIdSql = @"
            SELECT ""Id"", ""EmployeeId"", ""UserName""
            FROM ""AppUser""
            WHERE ""EmployeeId""::text = @EmployeeId";

        appUser = await _dapper.QueryFirstOrDefaultAsync<dynamic>(
            getByEmployeeIdSql,
            new { EmployeeId = employeeId }
        );
    }

    // Log what we found
    if (appUser != null)
    {
        Console.WriteLine($"? Found user: Id={appUser.Id}, EmployeeId={appUser.EmployeeId}, UserName={appUser.UserName}");
    }
    else
    {
        Console.WriteLine($"? No user found with identifier: {employeeId}");
        return Ok(ApiResponse<object>.Ok(new
        {
            UserId = employeeId,
            EmployeeId = employeeId,
            Modules = new List<Guid>(),
            Menus = new List<Guid>()
        }));
    }

    var appUserId = appUser.Id.ToString();

    // Get modules and menus
    const string getModulesSql = @"
        SELECT ""PerModuleId""
        FROM ""UserPerModule""
        WHERE ""UserId"" = @UserId
        AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

    var modules = await _dapper.QueryAsync<Guid>(getModulesSql, new { UserId = appUserId });

    const string getMenusSql = @"
        SELECT ""PerMenuId""
        FROM ""UserPerMenu""
        WHERE ""UserId"" = @UserId
        AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

    var menus = await _dapper.QueryAsync<Guid>(getMenusSql, new { UserId = appUserId });

    var result = new
    {
        UserId = appUserId,
        EmployeeId = employeeId,
        Modules = modules.ToList(),
        Menus = menus.ToList()
    };

    return Ok(ApiResponse<object>.Ok(result));
}

    [HttpGet("GetPerApiByUser/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerApiByUser(string employeeId)
    {
        const string getAppUserIdSql = @"
            SELECT ""Id""
            FROM ""AppUser""
            WHERE ""EmployeeId""::text = @EmployeeId";

        var appUserId = await _dapper.QueryFirstOrDefaultAsync<string>(
            getAppUserIdSql,
            new { EmployeeId = employeeId }
        );

        if (string.IsNullOrEmpty(appUserId))
        {
            return Ok(ApiResponse<object>.Ok(new List<Guid>()));
        }

        const string getApisSql = @"
            SELECT ""PerApiId""
            FROM ""UserPerApi""
            WHERE ""UserId"" = @UserId
            AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

        var apis = await _dapper.QueryAsync<Guid>(getApisSql, new { UserId = appUserId });

        return Ok(ApiResponse<object>.Ok(apis.ToList()));
    }

    // ==================== PERMISSION STRUCTURE ENDPOINTS ====================

    [HttpGet("GetPermissionStructure")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissionStructure()
    {
        var response = await _med.Send(new GetPermissionStructureQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("GetFilteredPermissionsForUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilteredPermissionsForUser([FromBody] FilteredPermissionsReq request)
    {
        var response = await _med.Send(new PerMenuFilteredByUserQry
        {
            UserId = request.UserId,
            ModuleIds = request.ModuleIds
        });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("GetFilteredPerApisForUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilteredPerApisForUser([FromBody] FilteredApiPermissionsReq request)
    {
        var response = await _med.Send(new PerApiFilteredByUserQry
        {
            UserId = request.UserId,
            MenuIds = request.MenuIds
        });
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ==================== SAVE USER PERMISSIONS ====================
[HttpPost("SaveUserPermissions")]
[ProducesResponseType(StatusCodes.Status200OK)]
public async Task<IActionResult> SaveUserPermissions([FromBody] SaveUserPermissionsReq request)
{
    // ? Log the request
    Console.WriteLine($"=== SAVE USER PERMISSIONS ===");
    Console.WriteLine($"UserId: {request.UserId}");
    Console.WriteLine($"ModuleIds: {(request.ModuleIds == null ? "NULL" : request.ModuleIds.Count.ToString())}");
    Console.WriteLine($"MenuIds: {(request.MenuIds == null ? "NULL" : request.MenuIds.Count.ToString())}");
    Console.WriteLine($"ApiActionIds: {(request.ApiActionIds == null ? "NULL" : request.ApiActionIds.Count.ToString())}");

   string? appUserId = null;

    // First try to find by Id (UserId)
    const string getByUserIdSql = @"
        SELECT ""Id""
        FROM ""AppUser""
        WHERE ""Id""::text = @UserId";

    appUserId = await _dapper.QueryFirstOrDefaultAsync<string?>(
         getByUserIdSql,
         new { UserId = request.UserId }
     );

    // If not found by Id, try by EmployeeId
    if (string.IsNullOrEmpty(appUserId))
    {
        const string getByEmployeeIdSql = @"
            SELECT ""Id""
            FROM ""AppUser""
            WHERE ""EmployeeId""::text = @EmployeeId";

      appUserId = await _dapper.QueryFirstOrDefaultAsync<string?>(
          getByEmployeeIdSql,
          new { EmployeeId = request.UserId }
      );
    }

    Console.WriteLine($"Mapped AppUserId: {appUserId} (from input: {request.UserId})");

    if (string.IsNullOrEmpty(appUserId))
    {
        throw new DomainException($"User with Id/EmployeeId '{request.UserId}' not found in AppUser table");
    }

    // ? Helper to parse string IDs to Guids
    List<Guid>? ParseIds(List<string>? ids)
    {
        if (ids == null || ids.Count == 0) return null;
        var result = ids
            .Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();
        return result.Any() ? result : null;
    }

    var command = new UpdateUserPermissionsCmd
    {
        UserId = appUserId,
        ModuleIds = ParseIds(request.ModuleIds),
        MenuIds = ParseIds(request.MenuIds),
        ApiActionIds = ParseIds(request.ApiActionIds)
    };

    var response = await _med.Send(command);
    return Ok(ApiResponse<object>.Ok(response, "User permissions saved successfully"));
}

    // ==================== UTILITY ENDPOINTS ====================

    [HttpGet("GetAppUserIdByEmployeeId/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppUserIdByEmployeeId(string employeeId)
    {
        const string sql = @"
            SELECT ""Id"" FROM ""AppUser"" WHERE ""EmployeeId""::text = @EmployeeId";

        var appUserId = await _dapper.QueryFirstOrDefaultAsync<string>(sql, new { EmployeeId = employeeId });

        return Ok(ApiResponse<object>.Ok(new { AppUserId = appUserId, EmployeeId = employeeId }));
    }

// In PermissionController.cs - Add this endpoint

[HttpPut("ModPerMenu/{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> UpdateMenuPermission(Guid id, [FromBody] UpdateMenuPermissionDto dto)
{
    if (id != dto.Id)
        throw new DomainException("ID mismatch");

    var result = await _med.Send(new UpdateMenuPermissionCmd { Dto = dto });
    if (!result)
        throw new DomainException("Failed to update menu permission");

    return Ok(ApiResponse<object>.Ok(null, "Menu permission updated successfully."));
}

[HttpDelete("DelPerMenuByKey/{key}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeletePerMenuByKey(string key)
{
    // First get the permission by key
    var permission = await _med.Send(new PerMenuByKeyQry { Key = key });
    if (permission == null)
        throw new DomainException($"MENU PERMISSION with key [{key}] NOT FOUND.");

    // Then delete by ID
    await _med.Send(new PerMenuDelCmd { Id = permission.Id });
    return Ok(ApiResponse<object>.Ok(null, "Menu permission deleted successfully."));
}

[HttpPost("AddPerMenu")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> AddPerMenu([FromBody] PerMenuAddDto dto)
{
    var response = await _med.Send(new PerMenuAddCmd { AddDto = dto });
    return Ok(ApiResponse<object>.Ok(response, "Menu permission added successfully."));
}


[HttpPost("AddPerModule")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> AddPerModule([FromBody] PerModuleAddDto dto)
{
    var response = await _med.Send(new PerModuleAddCmd { AddDto = dto });
    return Ok(ApiResponse<object>.Ok(response, "Module added successfully."));
}

[HttpPut("ModPerModule/{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> UpdatePerModule(Guid id, [FromBody] PerModuleModDto dto)
{
    if (id != dto.Id)
        throw new DomainException("ID mismatch");

    var response = await _med.Send(new PerModuleModCmd { ModDto = dto });
    return Ok(ApiResponse<object>.Ok(response, "Module updated successfully."));
}

[HttpDelete("DelPerModule/{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeletePerModule(Guid id)
{
    await _med.Send(new PerModuleDelCmd { Id = id });
    return Ok(ApiResponse<object>.Ok(null, "Module deleted successfully."));
}

// ==================== PERMISSION REGISTRY (enforcement foundation) ====================

// Returns the deterministically-ordered canonical permission registry. Other
// services can call this at startup and PermissionMap.Initialize(keys) to get an
// identical IndexMap, enabling consistent cross-service [PerAuth] enforcement.
[HttpGet("Registry")]
[ProducesResponseType(StatusCodes.Status200OK)]
public IActionResult PermissionRegistry()
{
    return Ok(ApiResponse<object>.Ok(new
    {
        Count = PermissionMap.IndexMap.Count,
        Keys = PermissionMap.OrderedKeys
    }));
}

// Verifies that the caller's JWT `ph` bitmask correctly encodes a given
// permission (read-only; does not change access). Proves the registry + bitmask
// pipeline end-to-end before [PerAuth] is rolled out to real endpoints.
[HttpGet("MyPermissionCheck/{permission}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status200OK)]
public IActionResult MyPermissionCheck(string permission)
{
    var ph = User.FindFirst("ph")?.Value;
    var granted = false;

    if (!string.IsNullOrEmpty(ph) && PermissionMap.IndexMap.TryGetValue(permission, out var index))
    {
        try
        {
            var bytes = System.Convert.FromBase64String(ph);
            var byteIndex = index / 8;
            granted = byteIndex < bytes.Length && (bytes[byteIndex] & (1 << (index % 8))) != 0;
        }
        catch
        {
            granted = false;
        }
    }

    return Ok(ApiResponse<object>.Ok(new
    {
        Permission = permission,
        Granted = granted,
        InRegistry = PermissionMap.IndexMap.ContainsKey(permission),
        RegistrySize = PermissionMap.IndexMap.Count
    }));
}

}