using MediatR;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Helpers;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;

public class RefreshTokenCmd : IRequest<LoginResDto> { public RefreshTokenDto Input { get; set; } = default!; }

public class RefreshTokenCmdHandler : IRequestHandler<RefreshTokenCmd, LoginResDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCmdHandler(UserManager<AppUser> userManager, ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResDto> Handle(RefreshTokenCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var tokenRec = await _unitOfWork.Repository<RefreshToken>().GetFoD(t => t.Token == request.Input.RefreshToken);
            if (tokenRec is not { IsRevoked: true }) { throw new DomainException($"REFRESH TOKEN with Token {request.Input.RefreshToken} NOT FOUND."); }

            var user = await _userManager.FindByIdAsync(tokenRec.UserId);
            if (user == null) { throw new UnauthorizedException("User NOT FOUND!"); }

            var newRefresh = await _tokenService.RefreshTokenAsync(user, tokenRec);
            return new LoginResDto
            {
                AccessToken = newRefresh.AccessToken,
                RefreshToken = newRefresh.RefreshToken,
                ExpiresDate = newRefresh.Expiry
            };
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}