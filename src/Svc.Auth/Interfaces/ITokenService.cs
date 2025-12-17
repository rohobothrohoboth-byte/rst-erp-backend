using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessToken(AppUser user);
    Task<RefreshToken> GenerateRefreshToken(string userId);
    Task<TokenDto> RefreshToken(AppUser user);
    Task RevokeToken(string userId);
    bool ValidateToken(string token);
    UserDto GetUserFromToken(string token);
}