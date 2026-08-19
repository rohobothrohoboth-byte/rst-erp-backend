// Svc.Auth.Services/TokenService.cs - Complete updated implementation

using Common;
using Dapper;
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
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Svc.Auth.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDapperHelper _dapper;
    private readonly IConfiguration _configuration;

    public TokenService(
        IConfiguration configuration,
        IUnitOfWork uow,
        UserManager<AppUser> userManager,
        IDapperHelper dapper)
    {
        _uow = uow;
        _userManager = userManager;
        _dapper = dapper;
        _configuration = configuration;
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
                menu.C = SortMenus(menu.C);
        }
        menus.Sort((a, b) => a.O.CompareTo(b.O));
        return menus;
    }

    private async Task<UserOrgInfoDto> GetUserOrgInfoAsync(AppUser user, CancellationToken ct)
    {
        var orgInfo = new UserOrgInfoDto();

        if (user.EmployeeId == null)
            return orgInfo;

        try
        {
            var profileConnectionString = _configuration.GetConnectionString("HRMProDbCon");

            if (string.IsNullOrEmpty(profileConnectionString))
            {
                profileConnectionString = _configuration.GetConnectionString("coreHRMMDbCon");
            }

            if (string.IsNullOrEmpty(profileConnectionString))
                return orgInfo;

            using var profileConnection = new NpgsqlConnection(profileConnectionString);

            const string empSql = @"
                SELECT
                    e.""BranchId"" as BranchId,
                    e.""DepartmentId"" as DepartmentId,
                    e.""PositionId"" as PositionId,
                    e.""JobGradeId"" as JobGradeId,
                    b.""Name"" AS BranchName,
                    b.""Code"" AS BranchCode,
                    d.""Name"" AS DepartmentName,
                    p.""Name"" AS PositionName,
                    jg.""Name"" AS JobGradeName
                FROM ""Employee"" e
                LEFT JOIN ""Branch"" b ON e.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
                LEFT JOIN ""Department"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
                LEFT JOIN ""Position"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
                LEFT JOIN ""JobGrade"" jg ON e.""JobGradeId"" = jg.""Id"" AND jg.""IsDeleted"" = false
                WHERE e.""Id"" = @EmpId";

            var empInfo = await profileConnection.QueryFirstOrDefaultAsync<dynamic>(empSql, new { EmpId = user.EmployeeId });

            if (empInfo != null)
            {
                orgInfo.BranchId = empInfo.BranchId?.ToString() ?? string.Empty;
                orgInfo.DepartmentId = empInfo.DepartmentId?.ToString() ?? string.Empty;
                orgInfo.PositionId = empInfo.PositionId?.ToString() ?? string.Empty;
                orgInfo.JobGradeId = empInfo.JobGradeId?.ToString() ?? string.Empty;
                orgInfo.BranchName = empInfo.BranchName?.ToString() ?? string.Empty;
                orgInfo.BranchCode = empInfo.BranchCode?.ToString() ?? string.Empty;
                orgInfo.DepartmentName = empInfo.DepartmentName?.ToString() ?? string.Empty;
                orgInfo.PositionName = empInfo.PositionName?.ToString() ?? string.Empty;
                orgInfo.JobGradeName = empInfo.JobGradeName?.ToString() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail token generation
            Console.WriteLine($"Error fetching org info: {ex.Message}");
        }

        return orgInfo;
    }

    public async Task<string> GenerateAccessToken(AppUser user, CancellationToken ct)
    {
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account has been deactivated. Please contact your administrator.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        // ========== GET USER'S ACTUAL API PERMISSIONS ==========
        var userApiKeys = _uow.Set<UserPerApi>()
            .Where(x => x.UserId == user.Id)
            .Select(x => x.PerApi.Key)
            .ToList();
        var userApiKeySet = new HashSet<string>(userApiKeys);

        // ========== GET POSITION-BASED PERMISSIONS ==========
        var userPositionId = await _uow.Set<AppUser>()
            .Where(u => u.Id == user.Id)
            .Select(u => u.PositionId)
            .FirstOrDefaultAsync(ct);

        var positionApiKeys = new List<string>();
        if (userPositionId.HasValue)
        {
            positionApiKeys = await _uow.Set<PositionPerApi>()
                .Where(ppa => ppa.PositionId == userPositionId.Value)
                .Select(ppa => ppa.PerApi.Key)
                .ToListAsync(ct);
        }

        var allApiKeys = userApiKeys.Union(positionApiKeys).ToList();

        // ========== GET EMPLOYEE ORG INFO ==========
        var orgInfo = await GetUserOrgInfoAsync(user, ct);

        // ========== QUERY 1: Get user's permitted menus ==========
        var qb = new QueryBuilder()
            .SelectAs<PerModule, FlatPermissionDto>("pm", x => x.Key, x => x.ModKey)
            .SelectAs<PerModule, FlatPermissionDto>("pm", x => x.Desc, x => x.ModDesc)
            .SelectAs<PerMenu, FlatPermissionDto>("mn", x => x.Key, x => x.MenuKey)
            .SelectAs<PerMenu, FlatPermissionDto>("mn", x => x.Id, x => x.MenuId)
           .Select<PerMenu>("mn", x => x.Label!, x => x.Path!, x => x.Icon, x => x.IsChild, x => x.Order, x => x.ParentId!)
            .SelectAs<PerApi, FlatPermissionDto>("pa", x => x.Key, x => x.ApiKey)
            .From<UserPerModule>("upm")
            .Join<UserPerModule, PerModule>("upm", "pm", x => x.PerModuleId, x => x.Id)
            .Join<PerModule, PerMenu>("pm", "mn", x => x.Id, x => x.PerModuleId)
            .Join<PerMenu, UserPerMenu>("mn", "upn", x => x.Id, x => x.PerMenuId)
            .Where<UserPerMenu>("upn", x => x.UserId == user.Id)
            .ThenInclude<PerMenu, PerApi>("mn", "pa", x => x.Id, x => x.PerMenuId)
            .ThenInclude<PerApi, UserPerApi>("pa", "upa", x => x.Id, x => x.PerApiId)
            .AndOn<UserPerApi>("upa", x => x.UserId == user.Id)
            .Where<UserPerModule>("upm", x => x.UserId == user.Id)
            .OrderBy<PerModule>("pm", x => x.Key)
            .OrderBy<PerMenu>("mn", x => x.Key)
            .OrderBy<PerApi>("pa", x => x.Key);

        var (sql, parameters) = qb.Build();

        List<FlatPermissionDto> data;
        await using (var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct))
        {
            data = await reader.ToListAsync<FlatPermissionDto>(ct);
        }

        // ========== PERMISSION BITS ==========
        var permissionBits = new byte[(PermissionMap.IndexMap.Count + 7) / 8];

        foreach (var api in allApiKeys.Distinct())
        {
            if (PermissionMap.IndexMap.TryGetValue(api, out var index))
                permissionBits[index / 8] |= (byte)(1 << (index % 8));
        }
        var permissionHash = Convert.ToBase64String(permissionBits);

        // ========== COLLECT MISSING PARENT IDs ==========
        var allParentIds = new HashSet<Guid>();
        foreach (var item in data.Where(x => x.ParentId != null))
        {
            if (item.ParentId.HasValue)
                allParentIds.Add(item.ParentId.Value);
        }

        var existingMenuIds = new HashSet<Guid>(data.Select(x => x.MenuId).Distinct());
        var missingParentIds = allParentIds.Where(pid => !existingMenuIds.Contains(pid)).ToList();

        // ========== QUERY 2: Fetch missing parent menus ==========
        List<FlatPermissionDto> parentData = new();
        if (missingParentIds.Any())
        {
            var parentSql = @"
                SELECT
                    pm.""Key"" AS ""ModKey"", pm.""Desc"" AS ""ModDesc"",
                    mn.""Key"" AS ""MenuKey"", mn.""Id"" AS ""MenuId"",
                    mn.""Label"", mn.""Path"", mn.""Icon"",
                    mn.""IsChild"", mn.""Order"", mn.""ParentId"",
                    CAST(NULL AS text) AS ""ApiKey""
                FROM ""PerMenu"" mn
                JOIN ""PerModule"" pm ON mn.""PerModuleId"" = pm.""Id""
                WHERE mn.""Id"" = ANY(@ParentIds)";

            var parentParams = new DynamicParameters();
            parentParams.Add("ParentIds", missingParentIds.ToArray());

            await using (var parentReader = await _dapper.ExecuteReaderAsync(parentSql, parentParams, ct))
            {
                parentData = await parentReader.ToListAsync<FlatPermissionDto>(ct);
            }
        }

        // ========== COMBINE DATA ==========
        var allData = data.Concat(parentData).ToList();

        // ========== BUILD MENU TREE WITH FILTERED ACTIONS ==========
        var modules = allData.GroupBy(x => x.ModKey).Select(module =>
        {
            var moduleFirst = module.First();

            var menuDict = module.Where(x => x.MenuKey != null).GroupBy(x => x.MenuId).ToDictionary(g => g.Key, g =>
            {
                var first = g.First();

                var userActions = g
                    .Where(x => x.ApiKey != null)
                    .Select(x => x.ApiKey!)
                    .Distinct()
                    .Where(api => allApiKeys.Contains(api))
                    .ToList();

                return new MenuTokenDto
                {
                    K = first.MenuKey,
                    L = first.Label,
                    P = first.Path,
                    I = first.Icon,
                    O = first.Order,
                    A = userActions
                };
            });

            var roots = new List<MenuTokenDto>();
            foreach (var item in module.GroupBy(x => x.MenuId))
            {
                var first = item.First();
                if (!menuDict.TryGetValue(first.MenuId, out var current)) continue;

                if (first.ParentId == null)
                {
                    roots.Add(current);
                }
                else if (menuDict.TryGetValue(first.ParentId.Value, out var parent))
                {
                    parent.C ??= new List<MenuTokenDto>();
                    if (!parent.C.Any(c => c.K == current.K))
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

        // ========== BUILD JWT CLAIMS ==========
        var claims = new List<Claim>
        {
            new Claim(AuthCons.UserId, user.Id),
            new Claim(AuthCons.UserName, user.UserName ?? ""),
            new Claim("email", user.Email ?? ""),
        };

        if (user.EmployeeId != null)
            claims.Add(new Claim(AuthCons.EmployeeId, user.EmployeeId.ToString()!));

        if (roles.Count > 0)
            claims.AddRange(roles.Select(r => new Claim(AuthCons.Role, r)));

        // ========== ADD ORGANIZATIONAL CLAIMS ==========
        if (!string.IsNullOrEmpty(orgInfo.BranchId))
        {
            claims.Add(new Claim("branchId", orgInfo.BranchId));
            claims.Add(new Claim("branchName", orgInfo.BranchName ?? ""));
            claims.Add(new Claim("branchCode", orgInfo.BranchCode ?? ""));
        }

        if (!string.IsNullOrEmpty(orgInfo.DepartmentId))
        {
            claims.Add(new Claim("departmentId", orgInfo.DepartmentId));
            claims.Add(new Claim("departmentName", orgInfo.DepartmentName ?? ""));
        }


        if (!string.IsNullOrEmpty(orgInfo.PositionId))
        {
            claims.Add(new Claim("positionId", orgInfo.PositionId));
            claims.Add(new Claim("positionName", orgInfo.PositionName ?? ""));
        }

        if (!string.IsNullOrEmpty(orgInfo.JobGradeId))
        {
            claims.Add(new Claim("jobGradeId", orgInfo.JobGradeId));
            claims.Add(new Claim("jobGradeName", orgInfo.JobGradeName ?? ""));
        }

        if (positionApiKeys.Any())
        {
            claims.Add(new Claim("positionPermissions", string.Join(",", positionApiKeys)));
        }

        claims.Add(new Claim("ph", permissionHash));

        // ========== GENERATE TOKEN ==========
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtCons.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtCons.Issuer,
            audience: JwtCons.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(JwtCons.ExpiryInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ? Implement GenerateRefreshToken that takes AppUser
    public async Task<string> GenerateRefreshToken(AppUser user, CancellationToken cancellationToken)
    {
        var refreshToken = await GenerateRefreshTokenInternal(user.Id);
        return refreshToken.Token;
    }

    // Internal method for refresh token generation
    private async Task<RefreshToken> GenerateRefreshTokenInternal(string userId)
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
       if (!user.IsActive)
       {
           throw new UnauthorizedAccessException("Account has been deactivated. Please contact your administrator.");
       }

       // FIX: Generate the new tokens
       var newAccessToken = await GenerateAccessToken(user, ct);

       // FIX: Ensure we use a new transaction for the refresh token
       // Use a separate UnitOfWork instance or ensure connection is open
       var refreshToken = await GenerateRefreshTokenInternal(user.Id);

       return new TokenDto
       {
           AccessToken = newAccessToken,
           RefreshToken = refreshToken.Token
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
        var empId = jwtToken.Claims.FirstOrDefault(c => c.Type == AuthCons.EmployeeId)?.Value ?? "";
        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == AuthCons.Role)?.Value ?? "";
        var perModule = jwtToken.Claims.Where(c => c.Type == AuthCons.PerModule).Select(c => c.Value).ToList();
        var perMenu = jwtToken.Claims.Where(c => c.Type == AuthCons.PerMenu).Select(c => c.Value).ToList();
        var perApi = jwtToken.Claims.Where(c => c.Type == AuthCons.PerApi).Select(c => c.Value).ToList();

        return new UserDto
        {
            UserId = userId,
            Username = uName,
            Role = role,
            PerModule = perModule,
            PerMenu = perMenu,
            PerApi = perApi,
            EmployeeId = string.IsNullOrEmpty(empId) ? null : Guid.Parse(empId)
        };
    }
}