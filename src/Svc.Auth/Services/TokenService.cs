using Common;
using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Svc.Auth.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDapperHelper _dapper;

    public TokenService(IUnitOfWork uow, UserManager<AppUser> userManager, IDapperHelper dapper)
    {
        _uow = uow;
        _userManager = userManager;
        _dapper = dapper;
    }

    private static string GenRefToken()
    {
        var rByte = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(rByte);
    }

    private static List<MenuTokenDto> SortMenus(List<MenuTokenDto> menus)
    {
        foreach (var menu in menus)
        {
            if (menu.C != null && menu.C.Count > 0)
            {
                menu.C = SortMenus(menu.C);
            }
        }

        menus.Sort((a, b) => a.O.CompareTo(b.O));
        return menus;
    }

    public async Task<string> GenerateAccessToken(AppUser user, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var qb = new QueryBuilder()
            .SelectAs<PerModule, FlatPermissionDto>("pm", x => x.Key, x => x.ModKey)
            .SelectAs<PerModule, FlatPermissionDto>("pm", x => x.Desc, x => x.ModDesc)
            .SelectAs<PerMenu, FlatPermissionDto>("mn", x => x.Key, x => x.MenuKey)
            .SelectAs<PerMenu, FlatPermissionDto>("mn", x => x.Id, x => x.MenuId)
            .Select<PerMenu>("mn", x => x.Label, x => x.Path, x => x.Icon, x => x.IsChild, x => x.Order, x => x.ParentId)
            .SelectAs<PerApi, FlatPermissionDto>("pa", x => x.Key, x => x.ApiKey)
            .From<UserPerModule>("upm")
            .Join<UserPerModule, PerModule>("upm", "pm", x => x.PerModuleId, x => x.Id)
            .ThenInclude<PerModule, PerMenu>("pm", "mn", x => x.Id, x => x.PerModuleId)
            .ThenInclude<PerMenu, UserPerMenu>("mn", "upn", x => x.Id, x => x.PerMenuId).AndOn<UserPerMenu>("upn", x => x.UserId == user.Id)
            .ThenInclude<PerMenu, PerApi>("mn", "pa", x => x.Id, x => x.PerMenuId)
            .ThenInclude<PerApi, UserPerApi>("pa", "upa", x => x.Id, x => x.PerApiId).AndOn<UserPerApi>("upa", x => x.UserId == user.Id)
            .Where<UserPerModule>("upm", x => x.UserId == user.Id)
            .OrderBy<PerModule>("pm", x => x.Key)
            .OrderBy<PerMenu>("mn", x => x.Key)
            .OrderBy<PerApi>("pa", x => x.Key);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var data = await reader.ToListAsync<FlatPermissionDto>(ct);

        var permissionBits = new byte[(PermissionMap.IndexMap.Count + 7) / 8];
        foreach (var api in data.Where(x => x.ApiKey != null).Select(x => x.ApiKey!).Distinct())
        {
            if (PermissionMap.IndexMap.TryGetValue(api, out var index))
            {
                permissionBits[index / 8] |= (byte)(1 << (index % 8));
            }
        }
        var permissionHash = Convert.ToBase64String(permissionBits);

        var modules = data.GroupBy(x => x.ModKey).Select(module =>
        {
            var moduleFirst = module.First();
            var menuDict = module.Where(x => x.MenuKey != null).GroupBy(x => x.MenuId).ToDictionary(g => g.Key, g =>
                {
                    var first = g.First();
                    return new MenuTokenDto
                    {
                        K = first.MenuKey,
                        L = first.Label,
                        P = first.Path,
                        I = first.Icon,
                        O = first.Order,
                        A = g.Where(x => x.ApiKey != null).Select(x => x.ApiKey!).Distinct().ToList()
                    };
                });

            var roots = new List<MenuTokenDto>();
            foreach (var item in module.GroupBy(x => x.MenuId))
            {
                var first = item.First();
                var current = menuDict[first.MenuId];
                if (first.ParentId == null)
                {
                    roots.Add(current);
                }
                else if (menuDict.TryGetValue(first.ParentId.Value, out var parent))
                {
                    parent.C ??= new List<MenuTokenDto>();
                    parent.C.Add(current);
                }
                else
                {
                    roots.Add(current);
                }
            }

            return new ModuleTokenDto
            {
                K = module.Key,
                L = moduleFirst.ModDesc,
                M = SortMenus(roots)
            };
        }).OrderBy(m => m.K).ToList();

        var claims = new List<Claim>
        {
            //new(AuthCons.UserId, user.Id),
            new(AuthCons.UserName, user.UserName ?? "")
        };

        if (user.EmployeeId != null) { claims.Add(new Claim(AuthCons.EmployeeId, user.EmployeeId.ToString()!)); }
        if (roles.Count > 0) { claims.AddRange(roles.Select(r => new Claim(AuthCons.Role, r))); }

        var permissionsJson = JsonSerializer.Serialize(modules);
        claims.Add(new Claim(AuthCons.Permissions, permissionsJson));
        claims.Add(new Claim("ph", permissionHash));

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: JwtCons.Issuer,
            audience: JwtCons.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(JwtCons.ExpiryInMinutes),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshToken(string userId)
    {
        await RevokeToken(userId);
        var refreshToken = new RefreshToken
        {
            Token = GenRefToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(JwtCons.RefreshTokenExpireDays),
            IsRevoked = false,
            IsDeleted = false,
            UserId = userId
        };

        await _uow.Add(refreshToken);
        return refreshToken;
    }

    public async Task<TokenDto> RefreshToken(AppUser user, CancellationToken ct)
    {
        var newAccessToken = await GenerateAccessToken(user, ct);
        var newRefresh = await GenerateRefreshToken(user.Id);

        return new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefresh.Token
        };
    }

    public async Task RevokeToken(string userId)
    {
        var rToken = _uow.Set<RefreshToken>().Where(p => p.UserId == userId).ToList();
        if (rToken.Count > 0)
        {
            foreach (var token in rToken)
            {
                token.IsRevoked = true;
                token.IsDeleted = true;
                token.RevokedDate = DateTime.UtcNow;
                await _uow.Update(token);
            }
        }
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
    }
}