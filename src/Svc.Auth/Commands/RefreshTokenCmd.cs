using MediatR;

namespace Svc.Auth.Commands;
/// <summary>
/// Command to refresh an access token using a valid refresh token.
/// </summary>
public class RefreshTokenCmd : IRequest<LoginResultDto>
{
    public RefreshDto Dto { get; }

    public RefreshTokenCmd(RefreshDto dto)
    {
        Dto = dto ?? throw new ArgumentNullException(nameof(dto));
    }
}

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCmd, LoginResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<LoginResultDto> Handle(RefreshTokenCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (dto.UserId == Guid.Empty || string.IsNullOrWhiteSpace(dto.RefreshToken))
            throw new DomainException("Invalid refresh token request.", 400);

        // Retrieve the refresh token
        var tokenEntity = await _unitOfWork.RefreshTokens.FindByTokenAsync(dto.UserId, dto.RefreshToken);

        if (tokenEntity == null || tokenEntity.ExpiresAt <= DateTime.UtcNow)
            throw new DomainException("Refresh token is invalid or expired.", 401);

        // Retrieve user
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new DomainException("User not found.", 404);

        // Generate new tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user);

        // Replace old refresh token with new one (rotation)
        tokenEntity.Token = newRefreshToken.Token;
        tokenEntity.ExpiresAt = newRefreshToken.ExpiresAt;
        tokenEntity.RevokedAt = null; // reset revoked
        tokenEntity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitAsync();

        return new LoginResultDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            },
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
        };
    }
}