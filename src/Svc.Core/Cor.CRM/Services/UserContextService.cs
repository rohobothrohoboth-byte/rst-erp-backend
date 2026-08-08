// Cor.CRM/Services/UserContextService.cs
using Cor.CRM.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Cor.CRM.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        return Guid.TryParse(userId, out var id) ? id : null;
    }

    public string? GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("userName")?.Value;
    }

    public string? GetCurrentUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;
    }

    public string? GetCurrentUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value;
    }

    public string? GetCurrentUserEmployeeId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("employeeId")?.Value;
    }
}