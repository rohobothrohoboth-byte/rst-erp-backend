using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;

public class RefreshTokenCmd : IRequest<LoginResDto>
{
    public string UserId { get; set; } = default!;
    public RefreshTokenDto Input { get; set; } = default!;
}



public class RefreshTokenCmdHandler : IRequestHandler<RefreshTokenCmd, LoginResDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;

    public RefreshTokenCmdHandler(UserManager<AppUser> userManager, ITokenService tokenService, IUnitOfWork uow)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _uow = uow;
    }

    public async Task<LoginResDto> Handle(RefreshTokenCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var rToken = await _uow.Set<RefreshToken>().FirstOrDefaultAsync(p => p.UserId == request.UserId && p.Token == request.Input.Token, ct);
            if (rToken == null || rToken.IsRevoked) { throw new UnauthorizedException("UNABLE to REFRESH current user TOKEN.!"); }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) { throw new UnauthorizedException("UNABLE to REFRESH current user TOKEN.!"); }

            var newRefresh = await _tokenService.RefreshToken(user);
            await _uow.Commit(ct);

            return new LoginResDto
            {
                AccessToken = newRefresh.AccessToken,
                RefreshToken = newRefresh.RefreshToken,
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