using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;

/// <summary>
/// Command to revoke a refresh token or all refresh tokens for a user.
/// </summary>
public class RevokeRefreshTokenCmd : IRequest
{
    public RefreshDto Dto { get; }

    public RevokeRefreshTokenCmd(RefreshDto dto)
    {
        Dto = dto ?? throw new ArgumentNullException(nameof(dto));
    }
}

public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCmd, RefreshDto>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeRefreshTokenHandler(
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RevokeRefreshTokenCmd request)
    {
        // Find refresh token by token string
        var tokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(t => t.Token == request.Token);

        if (tokenEntity == null)
        {
            return false; // Token not found
        }

        // Optional: Check userId matches if provided
        if (!string.IsNullOrEmpty(request.UserId) && tokenEntity.UserId != request.UserId)
        {
            return false; // Cannot revoke token for another user
        }

        tokenEntity.RevokedAt = DateTime.UtcNow;

        _refreshTokenRepository.Update(tokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}