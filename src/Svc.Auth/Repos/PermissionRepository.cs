using Dapper;
using Svc.Auth.Extensions;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Repos;

public class PermissionRepository : GenericDapperRepository<Permission>, IPermissionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PermissionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = "SELECT * FROM permissions WHERE name = @Name";
        return await conn.QuerySingleOrDefaultAsync<Permission>(query, new { Name = name });
    }

    public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = @"SELECT p.*
                          FROM permissions p
                          INNER JOIN user_permissions up ON up.permission_id = p.id
                          WHERE up.user_id = @UserId";
        return (await conn.QueryAsync<Permission>(query, new { UserId = userId })).ToList();
    }

    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId)
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = @"SELECT p.*
                          FROM permissions p
                          INNER JOIN role_permissions rp ON rp.permission_id = p.id
                          WHERE rp.role_id = @RoleId";
        return (await conn.QueryAsync<Permission>(query, new { RoleId = roleId })).ToList();
    }
}
