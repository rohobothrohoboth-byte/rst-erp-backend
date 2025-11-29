namespace Svc.Identity.Dtos;

public class RevokeTokenDto
{
    public string Token { get; set; } = default!;  // Access or Refresh token
}
