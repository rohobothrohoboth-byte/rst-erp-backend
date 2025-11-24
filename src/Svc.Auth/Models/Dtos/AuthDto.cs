using System.ComponentModel.DataAnnotations;

namespace Svc.Auth.Models.Dtos;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = default!;

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }
}

public class LoginDto
{
    [Required]
    public string UsernameOrEmail { get; set; } = default!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = default!;
}

public class RefreshDto
{
    public Guid UserId { get; set; }
    public string? RefreshToken { get; set; }
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
    public string[] Permissions { get; set; } = Array.Empty<string>();
}

public class PermissionDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class LoginResultDto
{
    /// <summary>
    /// JWT Access Token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh Token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Optional: User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Permissions list
    /// </summary>
    public string[] Permissions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Optional: Token expiration in UTC
    /// </summary>
    public DateTime AccessTokenExpiresAt { get; set; }
}