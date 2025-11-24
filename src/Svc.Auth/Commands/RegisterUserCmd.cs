using MediatR;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;
/// <summary>
/// Command to register a new user
/// </summary>
public class RegisterUserCmd : IRequest<RegisterUserResultDto>
{
    public RegisterDto Dto { get; }

    public RegisterUserCmd(RegisterDto dto)
    {
        Dto = dto ?? throw new ArgumentNullException(nameof(dto));
    }
}

public class RegisterUserHandler : IRequestHandler<RegisterUserCmd, RegisterUserResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<> _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterUserHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<RegisterUserResultDto> Handle(RegisterUserCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Check if username/email already exists
        var existingUser = await _unitOfWork.Users.FindByUsernameOrEmailAsync(dto.Username, dto.Email);
        if (existingUser != null)
            throw new DomainException("Username or Email already exists.", 400);

        // Hash password
        var hashedPassword = _passwordHasher.HashPassword(dto.Password);

        // Create user entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CommitAsync();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        // Save refresh token
        await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        });

        await _unitOfWork.CommitAsync();

        return new RegisterUserResultDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            },
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }
}