using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken(User user);
    bool ValidateToken(string token, out string? userId, out string[]? permissions);
}