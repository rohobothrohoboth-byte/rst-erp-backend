using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Queries;

/// <summary>
/// Query to retrieve all users
/// </summary>
public class GetUsersQry : IRequest<IEnumerable<UserDto>>
{
    // Optional filtering/pagination can be added here
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public GetUsersQry() { }
}

public class GetUsersHandler : IRequestHandler<GetUsersQry, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetUsersQry request)
    {
        // Retrieve users with optional filtering
        var users = await _userRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(request.Role))
        {
            users = users.Where(u => u.Roles.Any(r => r.Name == request.Role)).ToList();
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            users = users.Where(u => u.Email.Contains(request.Email)).ToList();
        }

        // Map to DTO
        var result = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Roles = u.Roles.Select(r => r.Name).ToArray(),
            Permissions = u.Roles.SelectMany(r => r.Permissions).Select(p => p.Name).ToArray()
        }).ToList();

        return result;
    }

    public Task<List<UserDto>> Handle(GetUsersQry request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}