using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessToken(AppUser user, CancellationToken ct = default);
    Task<RefreshToken> GenerateRefreshToken(string userId);
    Task<TokenDto> RefreshToken(AppUser user, CancellationToken ct = default);
    Task RevokeToken(string userId);
    bool ValidateToken(string token);
    UserDto GetUserFromToken(string token);
}