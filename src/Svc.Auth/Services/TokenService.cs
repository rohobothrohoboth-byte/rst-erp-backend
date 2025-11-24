using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Services;

public class TokenService : ITokenService
{
    private readonly string _secret;
    private readonly int _accessTokenMinutes;
    private readonly int _refreshTokenDays;

    public TokenService(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret not configured");
        _accessTokenMinutes = int.Parse(configuration["Jwt:AccessTokenMinutes"] ?? "15");
        _refreshTokenDays = int.Parse(configuration["Jwt:RefreshTokenDays"] ?? "7");
    }

    public (string Token, DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("permissions", string.Join(",", user.Permissions ?? Array.Empty<string>()))
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public (string Token, DateTime ExpiresAt) GenerateRefreshToken(User user)
    {
        var expires = DateTime.UtcNow.AddDays(_refreshTokenDays);
        var token = Guid.NewGuid().ToString("N");
        return (token, expires);
    }

    public bool ValidateToken(string token, out string? userId, out string[]? permissions)
    {
        userId = null;
        permissions = null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        try
        {
            tokenHandler.ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            userId = jwt.Subject;
            var permClaim = jwt.Claims.FirstOrDefault(c => c.Type == "permissions")?.Value ?? "";
            permissions = permClaim.Split(",", StringSplitOptions.RemoveEmptyEntries);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
