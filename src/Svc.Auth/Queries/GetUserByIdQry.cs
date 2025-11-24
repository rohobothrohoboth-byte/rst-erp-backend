using MediatR;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Queries;

/// <summary>
/// Query to retrieve a user by their unique Id
/// </summary>
public class GetUserByIdQry : IRequest<UserDto>
{
    public Guid UserId { get; }

    public GetUserByIdQry(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        UserId = userId;
    }
}

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQry, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<UserDto> Handle(GetUserByIdQry request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

        if (user == null)
            return null; // Controller will handle NotFound

        // Optionally include roles and permissions
        var roles = await _unitOfWork.Users.GetRolesAsync(user.Id);
        var permissions = await _unitOfWork.Users.GetPermissionsAsync(user.Id);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = roles,
            Permissions = permissions
        };
    }
}