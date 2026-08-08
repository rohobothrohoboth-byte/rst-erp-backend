// Cor.CRM/Interfaces/IUserContextService.cs
using System;

namespace Cor.CRM.Interfaces;

public interface IUserContextService
{
    Guid? GetCurrentUserId();
    string? GetCurrentUserName();
    string? GetCurrentUserEmail();
    string? GetCurrentUserRole();
    string? GetCurrentUserEmployeeId();
}