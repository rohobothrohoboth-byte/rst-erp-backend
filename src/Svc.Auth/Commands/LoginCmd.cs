using Dapper;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Svc.Auth.Commands;

public class LoginCmd : IRequest<LoginResponseDto>
{
    public LoginDto Login { get; set; } = default!;
}

public class LogoutCmd : IRequest<OpResult>
{
    public string UserId { get; set; } = default!;
}

public class LoginHandler : IRequestHandler<LoginCmd, LoginResponseDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly IDapperHelper _dapper;

    public LoginHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IConfiguration configuration,
        IDapperHelper dapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _configuration = configuration;
        _dapper = dapper;
    }

    public async Task<LoginResponseDto> Handle(LoginCmd request, CancellationToken ct)
    {
        // 1. Find user by username
        var user = await _userManager.FindByNameAsync(request.Login.Username);

        if (user == null)
            throw new UnauthorizedException("Invalid username or password");

        // 2. Check password
        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, request.Login.Password, false);

        if (!passwordCheck.Succeeded)
            throw new UnauthorizedException("Invalid username or password");

        // 3. Check if account is active
        if (!user.IsActive)
            throw new UnauthorizedException("Account has been deactivated. Please contact your administrator.");

        // 4. Get user's org info from LOCAL COPY in Auth database
        var orgInfo = await GetUserOrgInfoFromLocalCopy(user.Id.ToString(), ct);

        // 5. Generate JWT with org claims
        var token = await _tokenService.GenerateAccessToken(user, ct);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            EmployeeId = user.EmployeeId?.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            BranchId = user.BranchId?.ToString(),
            DepartmentId = user.DepartmentId?.ToString(),
            PositionId = user.PositionId?.ToString(),
            BranchName = orgInfo?.BranchName,
            DepartmentName = orgInfo?.DepartmentName,
            PositionName = orgInfo?.PositionName
        };
    }

    /// <summary>
    /// ? Get user org info from LOCAL COPY tables in Auth database
    /// No cross-database queries needed!
    /// </summary>
    private async Task<OrgInfoDto?> GetUserOrgInfoFromLocalCopy(string userId, CancellationToken ct)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("authMgrCon");
            if (string.IsNullOrEmpty(connectionString))
                return null;

            using var connection = new NpgsqlConnection(connectionString);

            const string sql = @"
                SELECT
                    b.""Name"" as BranchName,
                    d.""Name"" as DepartmentName,
                    p.""Name"" as PositionName
                FROM ""AppUser"" u
                LEFT JOIN ""Branches"" b ON u.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
                LEFT JOIN ""Departments"" d ON u.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
                LEFT JOIN ""Positions"" p ON u.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
                WHERE u.""Id""::text = @UserId";

            return await connection.QueryFirstOrDefaultAsync<OrgInfoDto>(
                sql,
                new { UserId = userId });
        }
        catch (Exception ex)
        {
            // Log the exception but don't throw - we want login to succeed even if org info fails
            Console.WriteLine($"Error fetching org info: {ex.Message}");
            return null;
        }
    }
}

// ? Separate LogoutHandler with proper implementation
public class LogoutHandler : IRequestHandler<LogoutCmd, OpResult>
{
    private readonly ITokenService _tokenService;

    public LogoutHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<OpResult> Handle(LogoutCmd request, CancellationToken ct)
    {
        try
        {
            // ? Check if token service supports revocation
            // If it does, call it; otherwise, just return success
            if (_tokenService is IRefreshTokenRevocable revocableService)
            {
                await revocableService.RevokeRefreshTokenAsync(request.UserId, ct);
                return OpResult.Success("Logged out successfully.");
            }

            // If revocation is not supported, just return success
            return OpResult.Success("Logged out successfully.");
        }
        catch (Exception ex)
        {
            return OpResult.Fail($"Logout failed: {ex.Message}");
        }
    }
}

public class UserOrgIdsDto
{
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
}

public class OrgInfoDto
{
    public string? BranchName { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionName { get; set; }
}