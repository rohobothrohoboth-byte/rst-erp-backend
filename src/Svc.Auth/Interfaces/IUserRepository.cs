using Svc.Auth.Models.Entities;

namespace Svc.Auth.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> FindByUsernameOrEmailAsync(string username, string email);
    Task<IEnumerable<string>> GetRolesAsync(Guid userId);
    Task<IEnumerable<string>> GetPermissionsAsync(Guid userId);
}