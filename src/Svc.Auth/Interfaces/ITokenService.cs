using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessToken(AppUser user);
    Task<RefreshToken> GenerateRefreshTokenAsync(AppUser user);
    Task<TokenDto> RefreshTokenAsync(AppUser user, RefreshToken refreshToken);
    Task RevokeTokenAsync(RefreshToken refreshToken);
    bool ValidateToken(string token);
    UserDto GetUserFromToken(string token);
}