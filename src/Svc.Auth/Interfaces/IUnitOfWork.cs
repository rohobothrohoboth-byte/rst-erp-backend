using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IPermissionRepository Permissions { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }

    Task<int> CommitAsync();
}