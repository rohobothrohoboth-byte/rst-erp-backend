using Svc.Auth.Extensions;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Repos;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;

    public IUserRepository Users { get; }
    public IPermissionRepository Permissions { get; }
    public IGenericRepository<RefreshToken> RefreshTokens { get; }

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;

        Users = new UserRepository(_connectionFactory);
        Permissions = new PermissionRepository(_connectionFactory);
        RefreshTokens = new GenericDapperRepository<RefreshToken>(_connectionFactory);
    }

    public async Task<int> CommitAsync()
    {
        // For Dapper/PostgreSQL, operations are executed immediately,
        // so we can optionally manage transactions here if needed.
        // For now, return 1 to indicate success.
        return await Task.FromResult(1);
    }

    public void Dispose()
    {
        // No persistent connection to dispose because each repository opens its own
    }
}