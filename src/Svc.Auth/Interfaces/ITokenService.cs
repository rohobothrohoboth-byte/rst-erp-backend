using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessToken(AppUser user);
    Task<RefreshToken> GenerateRefreshTokenAsync(AppUser user);
    Task<TokenDto> RefreshTokenAsync(string userId);
    Task RevokeTokenAsync(string userId);
    bool ValidateToken(string token);  // For microservices
    UserDto GetUserFromToken(string token);  // For microservices
}