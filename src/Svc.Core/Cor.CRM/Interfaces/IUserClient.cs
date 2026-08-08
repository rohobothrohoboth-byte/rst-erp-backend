using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cor.CRM.Interfaces;

public interface IUserClient
{
    Task<UserResponse> GetUser(string userId, CancellationToken ct = default);
    Task<List<UserData>> GetUsers(CancellationToken ct = default);
}

public class UserResponse
{
    public bool Success { get; set; }
    public UserData? Res { get; set; }
    public string? Message { get; set; }
}

public class UserData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public string? Department { get; set; }
}