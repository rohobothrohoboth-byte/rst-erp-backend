using Dapper;
using Svc.Auth.Extensions;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Repos;

public class UserRepository : GenericDapperRepository<User>, IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> FindByUsernameOrEmailAsync(string username, string email)
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = @"SELECT * FROM users 
                          WHERE username = @Username OR email = @Email";
        return await conn.QuerySingleOrDefaultAsync<User>(query, new { Username = username, Email = email });
    }

    public async Task<IEnumerable<string>> GetRolesAsync(Guid userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = @"SELECT r.name
                          FROM roles r
                          INNER JOIN user_roles ur ON ur.role_id = r.id
                          WHERE ur.user_id = @UserId";
        return (await conn.QueryAsync<string>(query, new { UserId = userId })).ToList();
    }

    public async Task<IEnumerable<string>> GetPermissionsAsync(Guid userId)
    {
        using var conn = _connectionFactory.CreateConnection();

        // Direct permissions
        var directQuery = @"SELECT p.name
                                FROM permissions p
                                INNER JOIN user_permissions up ON up.permission_id = p.id
                                WHERE up.user_id = @UserId";

        // Permissions via roles
        var roleQuery = @"SELECT p.name
                              FROM permissions p
                              INNER JOIN role_permissions rp ON rp.permission_id = p.id
                              INNER JOIN user_roles ur ON ur.role_id = rp.role_id
                              WHERE ur.user_id = @UserId";

        var directPermissions = await conn.QueryAsync<string>(directQuery, new { UserId = userId });
        var rolePermissions = await conn.QueryAsync<string>(roleQuery, new { UserId = userId });

        return directPermissions.Concat(rolePermissions).Distinct().ToList();
    }
}