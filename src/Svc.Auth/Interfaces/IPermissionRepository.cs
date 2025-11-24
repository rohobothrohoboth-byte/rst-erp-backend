using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface IPermissionRepository : IGenericRepository<Permission>
{
    Task<Permission?> GetByNameAsync(string name);
    Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId);
}
