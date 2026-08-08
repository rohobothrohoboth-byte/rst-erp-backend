// Svc.Auth/Interfaces/IRefreshTokenRevocable.cs
namespace Svc.Auth.Interfaces;

public interface IRefreshTokenRevocable
{
    Task RevokeRefreshTokenAsync(string userId, CancellationToken cancellationToken);
}