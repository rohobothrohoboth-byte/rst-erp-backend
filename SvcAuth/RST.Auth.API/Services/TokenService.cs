using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RST.Auth.API.Data;
using RST.Auth.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RST.Auth.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _db;
        private readonly IConfiguration _cfg;
        private readonly IKeyStore _keyStore;

        public TokenService(UserManager<ApplicationUser> um, AuthDbContext db, IConfiguration cfg, IKeyStore ks)
        { _userManager = um; _db = db; _cfg = cfg; _keyStore = ks; }

        public async Task<(string AccessToken, string RefreshToken, string Jti)> GenerateTokensAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleIds = _db.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Id).ToList();
            var perms = await _db.RolePermissions.Where(rp => roleIds.Contains(rp.RoleId)).Select(rp => rp.Permission.Name).Distinct().ToListAsync();

            var jti = Guid.NewGuid().ToString("N");
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, jti),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            claims.AddRange(perms.Select(p => new Claim("permission", p)));

            var (rsaKey, kid) = _keyStore.GetActiveSigningKey();
            var creds = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],       // e.g., "microservices"
                claims: claims,
                notBefore: DateTime.UtcNow.AddSeconds(-5),
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);
            token.Header["kid"] = kid;

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var refresh = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            return (jwt, refresh.Token, jti);
        }

        public async Task<string?> RefreshAccessTokenAsync(string refreshToken)
        {
            var token = await _db.RefreshTokens.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (token is null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow) return null;

            var (access, _, _) = await GenerateTokensAsync(token.User);
            return access;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (token is null || token.IsRevoked) return false;
            token.IsRevoked = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
