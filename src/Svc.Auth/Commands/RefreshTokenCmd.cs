using Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Helpers;
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
            var rToken = await _unitOfWork.Repository<RefreshToken>().GetFoD(p => p.UserId == request.UserId && p.Token == request.Input.Token);
            if (rToken == null || rToken.IsRevoked) { throw new UnauthorizedException("UNABLE to REFRESH current user TOKEN.!"); }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) { throw new UnauthorizedException("UNABLE to REFRESH current user TOKEN.!"); }

            var newRefresh = await _tokenService.RefreshToken(user);
            await _unitOfWork.Commit();

            return new LoginResDto
            {
                AccessToken = newRefresh.AccessToken,
                RefreshToken = newRefresh.RefreshToken,
                ExpiresDate = DateTime.UtcNow.AddMinutes(JwtCons.ExpiryInMinutes)
            };
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}