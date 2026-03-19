using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;

public class LoginCmd : IRequest<LoginResDto> { public LoginDto Login { get; set; } = default!; }



public class LoginCmdHandler : IRequestHandler<LoginCmd, LoginResDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;

    public LoginCmdHandler(UserManager<AppUser> userManager, ITokenService tokenService, IUnitOfWork uow)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _uow = uow;
    }

    public async Task<LoginResDto> Handle(LoginCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var user = await _userManager.FindByNameAsync(request.Login.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Login.Password))
            {
                throw new UnauthorizedException("Invalid credentials.");
            }

            var aToken = await _tokenService.GenerateAccessToken(user, ct);
            var rToken = await _tokenService.GenerateRefreshToken(user.Id);

            await _uow.Commit(ct);

            return new LoginResDto
            {
                AccessToken = aToken,
                RefreshToken = rToken.Token,
                ExpiresDate = DateTime.UtcNow.AddMinutes(JwtCons.ExpiryInMinutes)
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}