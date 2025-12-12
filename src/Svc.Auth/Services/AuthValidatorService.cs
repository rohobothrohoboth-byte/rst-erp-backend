using Contracts;
using Grpc.Core;
using Svc.Auth.Interfaces;

namespace Svc.Auth.Services;

public class AuthValidatorService : AuthValidator.AuthValidatorBase
{
    private readonly ITokenService _tokenService;

    public AuthValidatorService(ITokenService tokenService) => _tokenService = tokenService;

    public override Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        var isValid = _tokenService.ValidateToken(request.Token);
        return Task.FromResult(new ValidateTokenResponse { IsValid = isValid, ErrorMessage = isValid ? "" : "Invalid token" });
    }

    public override Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        try
        {
            var user = _tokenService.GetUserFromToken(request.Token);
            var res = new GetUserResponse
            {
                EmployeeId = user.EmployeeId == null ? "" : user.EmployeeId.ToString(),
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                PerModule = { user.PerModule ?? [] },
                PerMenu = { user.PerMenu ?? [] },
                PerApi = { user.PerApi ?? [] }
            };

            return Task.FromResult(res);
        }
        catch (Exception ex)
        {
            return Task.FromResult(new GetUserResponse { ErrorMessage = ex.Message });
        }
    }
}

