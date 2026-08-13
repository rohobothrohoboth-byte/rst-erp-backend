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
        // The endpoint is [Authorize], so the caller already presented a valid
        // access token (identity in request.UserId). The client refreshes with an
        // empty body + Bearer header, so a stored refresh token is optional.
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) { throw new UnauthorizedException("UNABLE to REFRESH current user TOKEN.!"); }

        // When a refresh token is supplied, validate it; a revoked token is rejected.
        if (!string.IsNullOrWhiteSpace(request.Input?.Token))
        {
            var rToken = await _uow.Set<RefreshToken>()
                .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.Token == request.Input.Token, ct);
            if (rToken != null && rToken.IsRevoked)
                throw new UnauthorizedException("UNABLE to REFRESH current user TOKEN.!");
        }

        await _uow.Begin(ct);
        try
        {
            var newRefresh = await _tokenService.RefreshToken(user, ct);
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