namespace Svc.Auth.Models.Dtos;

public class JwtAuthDto
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public int ExpiryInMinutes { get; set; }
    public int RefreshTokenExpireDays { get; set; }
}