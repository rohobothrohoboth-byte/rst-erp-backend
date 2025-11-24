using MediatR;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;

/// <summary>
/// Command for user login (username/email + password)
/// </summary>
public class LoginCmd : IRequest<LoginResultDto>
{
    public LoginDto Dto { get; }

    public LoginCmd(LoginDto dto)
    {
        Dto = dto ?? throw new ArgumentNullException(nameof(dto));
    }
}

public class LoginHandler : IRequestHandler<LoginCmd, LoginResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<> _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<LoginResultDto> Handle(LoginCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Find user by username or email
        var user = await _unitOfWork.Users.FindByUsernameOrEmailAsync(dto.UsernameOrEmail, dto.UsernameOrEmail);
        if (user == null)
            throw new DomainException("Invalid username/email or password.", 401);

        // Verify password
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new DomainException("Invalid username/email or password.", 401);

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

        return new LoginResultDto
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