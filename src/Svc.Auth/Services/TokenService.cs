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
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<string> GenerateAccessToken(AppUser user)
    {
        var pModule = (await _unitOfWork.Repository<UserPerModule>().Find(p => p.UserId == user.Id)).ToList();
        var pMenu = (await _unitOfWork.Repository<UserPerMenu>().Find(p => p.UserId == user.Id)).ToList();
        var pApi = (await _unitOfWork.Repository<UserPerApi>().Find(p => p.UserId == user.Id)).ToList();
        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        var claims = new List<Claim>
        {
            new(AuthCons.UserId, user.Id),
            new(AuthCons.UserName, user.UserName!),
            user.EmployeeId != null
                ? new Claim(AuthCons.EmployeeId, user.EmployeeId.ToString()!)
                : new Claim(AuthCons.EmployeeId, "")
        };

        if (roles.Count > 0)
        {
            claims.AddRange(roles.Select(p => new Claim(AuthCons.Role, p)));
        }

        if (pModule.Count > 0)
        {
            foreach (var api in pModule)
            {
                var per = await _unitOfWork.Repository<PerModule>().GetById(api.PerModuleId);
                if (per != null)
                {
                    claims.Add(new Claim(AuthCons.PerModule, per.Key));
                }
            }
        }
        else
        {
            claims.Add(new Claim(AuthCons.PerModule, ""));
        }

        if (pMenu.Count > 0)
        {
            foreach (var api in pMenu)
            {
                var per = await _unitOfWork.Repository<PerMenu>().GetById(api.PerMenuId);
                if (per != null)
                {
                    claims.Add(new Claim(AuthCons.PerMenu, per.Key));
                }
            }
        }
        else
        {
            claims.Add(new Claim(AuthCons.PerMenu, ""));
        }

        if (pApi.Count > 0)
        {
            foreach (var api in pApi)
            {
                var per = await _unitOfWork.Repository<PerApi>().GetById(api.PerApiId);
                if (per != null)
                {
                    claims.Add(new Claim(AuthCons.PerApi, per.Key));
                }
            }
        }
        else
        {
            claims.Add(new Claim(AuthCons.PerApi, ""));
        }

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: JwtCons.Issuer,
            audience: JwtCons.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(JwtCons.ExpiryInMinutes),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(AppUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiryDate = DateTime.Now.AddDays(JwtCons.RefreshTokenExpireDays),
            IsRevoked = false,
            UserId = user.Id
        };

        await _unitOfWork.Repository<RefreshToken>().Add(refreshToken);
        await _unitOfWork.Commit();
        return refreshToken;
    }

    public async Task<TokenDto> RefreshTokenAsync(AppUser user, RefreshToken refreshToken)
    {
        var newAccessToken = await GenerateAccessToken(user!);
        await RevokeTokenAsync(refreshToken);
        var newRefresh = await GenerateRefreshTokenAsync(user!);
        await _unitOfWork.Commit();

        return new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefresh.Token,
            Expiry = newRefresh.ExpiryDate
        };
    }

    public async Task RevokeTokenAsync(RefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        refreshToken.IsDeleted = true;
        refreshToken.RevokedDate = DateTime.UtcNow;
        await _unitOfWork.Repository<RefreshToken>().Update(refreshToken);
        await _unitOfWork.Commit();
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
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

        var res = new UserDto
        {
            UserId = userId,
            Username = uName,
            Role = role,
            PerModule = perModule,
            PerMenu = perMenu,
            PerApi = perApi,
            EmployeeId = empId.Length <= 0 ? null : Guid.Parse(empId)
        };

        return res;

        //return new UserDto
        //{
        //    EmployeeId = Guid.Parse(empId),
        //    UserId = userId,
        //    Username = uName,
        //    Role = role,
        //    PerModule = perModule,
        //    PerMenu = perMenu,
        //    PerApi = perApi
        //};
    }
}