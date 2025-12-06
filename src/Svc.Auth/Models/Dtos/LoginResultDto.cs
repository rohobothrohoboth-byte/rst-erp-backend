namespace Svc.Auth.Models.Dtos;

public class LoginResultDto
{
    public Guid? EmployeeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public string Username { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
    public string[] PerModule { get; set; } = Array.Empty<string>();
    public string[] PerMenu { get; set; } = Array.Empty<string>();
    public string[] PerApi { get; set; } = Array.Empty<string>();
}