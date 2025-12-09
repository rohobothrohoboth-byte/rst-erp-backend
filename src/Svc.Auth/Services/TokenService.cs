using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Svc.Auth.Constants;

namespace Svc.Auth.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _unitOfWork;
    //private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _userManager;

    //public TokenService(IUnitOfWork unitOfWork, IConfiguration config, UserManager<AppUser> userManager)
    public TokenService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        //_config = config;
        _userManager = userManager;
    }

    public async Task<string> GenerateAccessToken(AppUser user)
    {
        //var jwtAuthOp = _config.GetSection("Jwt").Get<JwtAuthDto>()!;
        await _unitOfWork.Begin();
        try
        {
            var pModule = await _unitOfWork.Repository<UserPerModule>().Find(p => p.UserId == user.Id);
            var pMenu = await _unitOfWork.Repository<UserPerMenu>().Find(p => p.UserId == user.Id);
            var pApi = await _unitOfWork.Repository<UserPerApi>().Find(p => p.UserId == user.Id);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(AuthCons.UserId, user.Id),
                new(AuthCons.EmployeeId, user.EmployeeId.ToString()!),
                new(AuthCons.UserName, user.UserName!)
            };
            claims.AddRange(roles.Select(p => new Claim(AuthCons.Role, p)));
            claims.AddRange(pModule.Select(p => new Claim(AuthCons.PerModule, p.PerModule.Key)));
            claims.AddRange(pMenu.Select(p => new Claim(AuthCons.PerMenu, p.PerMenu.Key)));
            claims.AddRange(pApi.Select(p => new Claim(AuthCons.PerApi, p.PerApi.Key)));

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                            issuer: JwtCons.Issuer,
                            audience: JwtCons.Audience,
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(JwtCons.ExpiryInMinutes),
                            signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(AppUser user)
    {
        await _unitOfWork.Begin();
        try
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.Now.AddDays(7),
                IsRevoked = false,
                UserId = user.Id
            };

            await _unitOfWork.Repository<RefreshToken>().Add(refreshToken);
            await _unitOfWork.Commit();
            return refreshToken;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<TokenDto> RefreshTokenAsync(string userId)
    {
        await _unitOfWork.Begin();
        try
        {
            var token = await _unitOfWork.Repository<RefreshToken>().GetFoD(rt => rt.UserId == userId);
            if (token == null) throw new SecurityTokenException("Invalid refresh token");

            var user = await _userManager.FindByIdAsync(userId);
            var newAccessToken = await GenerateAccessToken(user!);

            token.IsRevoked = true;
            token.RevokedDate = DateTime.UtcNow;
            await _unitOfWork.Repository<RefreshToken>().Update(token);
            var newRefresh = await GenerateRefreshTokenAsync(user!);
            await _unitOfWork.Commit();

            return new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefresh.Token,
                Expiry = newRefresh.ExpiryDate
            };
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task RevokeTokenAsync(string userId)
    {
        await _unitOfWork.Begin();
        try
        {
            var refreshToken = await _unitOfWork.Repository<RefreshToken>().GetFoD(rt => rt.UserId == userId);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedDate = DateTime.UtcNow;
                await _unitOfWork.Repository<RefreshToken>().Update(refreshToken);
                await _unitOfWork.Commit();
            }
            // For access token, consider blacklisting (e.g., via Redis cache, not implemented here for simplicity)
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }


    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            //var jwtAuthOp = _config.GetSection("Jwt").Get<JwtAuthDto>()!;
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = JwtCons.Issuer,
                ValidateAudience = true,
                ValidAudience = JwtCons.Audience,
                ClockSkew = TimeSpan.Zero
            }, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public UserDto GetUserFromToken(string token)
    {
        if (!ValidateToken(token)) throw new SecurityTokenException("Invalid token");

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var userId = jwtToken.Claims.First(c => c.Type == AuthCons.UserId).Value;
        var uName = jwtToken.Claims.First(c => c.Type == AuthCons.UserName).Value;
        var empId = jwtToken.Claims.First(c => c.Type == AuthCons.EmployeeId).Value;
        var role = jwtToken.Claims.First(c => c.Type == AuthCons.Role).Value;
        var perModule = jwtToken.Claims.Where(c => c.Type == AuthCons.PerModule).Select(c => c.Value).ToList();
        var perMenu = jwtToken.Claims.Where(c => c.Type == AuthCons.PerMenu).Select(c => c.Value).ToList();
        var perApi = jwtToken.Claims.Where(c => c.Type == AuthCons.PerApi).Select(c => c.Value).ToList();

        return new UserDto
        {
            EmployeeId = Guid.Parse(empId),
            UserId = userId,
            Username = uName,
            Role = role,
            PerModule = perModule,
            PerMenu = perMenu,
            PerApi = perApi
        };
    }
}