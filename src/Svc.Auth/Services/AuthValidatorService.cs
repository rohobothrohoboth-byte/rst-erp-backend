// Svc.Auth.Services/AuthValidatorService.cs (UPDATED)

using Contracts;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;
using System.Security.Claims;

namespace Svc.Auth.Services;

public class AuthValidatorService : AuthValidator.AuthValidatorBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;

    public AuthValidatorService(ITokenService tokenService, UserManager<AppUser> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        var isValid = _tokenService.ValidateToken(request.Token);

        if (isValid)
        {
            try
            {
                var user = _tokenService.GetUserFromToken(request.Token);
                if (user != null && !string.IsNullOrEmpty(user.UserId))
                {
                    var appUser = await _userManager.FindByIdAsync(user.UserId);
                    if (appUser != null && !appUser.IsActive)
                    {
                        return new ValidateTokenResponse
                        {
                            IsValid = false,
                            ErrorMessage = "Account has been deactivated"
                        };
                    }
                }
            }
            catch
            {
                // If we can't check, still return original validity
            }
        }

        return new ValidateTokenResponse
        {
            IsValid = isValid,
            ErrorMessage = isValid ? "" : "Invalid token"
        };
    }

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        try
        {
            var user = _tokenService.GetUserFromToken(request.Token);

            // Check if user is still active
            if (!string.IsNullOrEmpty(user.UserId))
            {
                var appUser = await _userManager.FindByIdAsync(user.UserId);
                if (appUser != null && !appUser.IsActive)
                {
                    return new GetUserResponse { ErrorMessage = "Account has been deactivated" };
                }
            }

            // ?? NEW: Get full org info from token claims
            var jwtToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()
                .ReadJwtToken(request.Token);

            var branchId = jwtToken.Claims.FirstOrDefault(c => c.Type == "branchId")?.Value ?? "";
            var branchName = jwtToken.Claims.FirstOrDefault(c => c.Type == "branchName")?.Value ?? "";
            var departmentId = jwtToken.Claims.FirstOrDefault(c => c.Type == "departmentId")?.Value ?? "";
            var departmentName = jwtToken.Claims.FirstOrDefault(c => c.Type == "departmentName")?.Value ?? "";
            var positionId = jwtToken.Claims.FirstOrDefault(c => c.Type == "positionId")?.Value ?? "";
            var positionName = jwtToken.Claims.FirstOrDefault(c => c.Type == "positionName")?.Value ?? "";

            var res = new GetUserResponse
            {
                EmployeeId = user.EmployeeId == null ? "" : user.EmployeeId.ToString(),
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                PerModule = { user.PerModule ?? [] },
                PerMenu = { user.PerMenu ?? [] },
                PerApi = { user.PerApi ?? [] },

                // ?? NEW: Org fields
                BranchId = branchId,
                BranchName = branchName,
                DepartmentId = departmentId,
                DepartmentName = departmentName,
                PositionId = positionId,
                PositionName = positionName
            };

            return res;
        }
        catch (Exception ex)
        {
            return new GetUserResponse { ErrorMessage = ex.Message };
        }
    }
}