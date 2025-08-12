using Microsoft.AspNetCore.Identity;

namespace RST.Auth.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
