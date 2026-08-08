// Middleware/JwtCacheMiddleware.cs
using Microsoft.Extensions.Caching.Memory;
//using Cor.Inventory.Helpers;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Models.Enums;
using Cor.Inventory.Services;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.IdentityModel.Tokens;

namespace Cor.Inventory.Middlewares;

public class JwtCacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<JwtCacheMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public JwtCacheMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        ILogger<JwtCacheMiddleware> logger,
        IConfiguration configuration) // ✅ Add IConfiguration
    {
        _next = next;
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var cacheKey = $"jwt_validation_{token}";

            if (_cache.TryGetValue(cacheKey, out bool _))
            {
                _logger.LogDebug("📦 JWT Cache HIT");
                // Token already validated, skip validation
            }
            else
            {
                _logger.LogDebug("📦 JWT Cache MISS - Validating token");
                // Validate token and cache the result
                var isValid = await ValidateToken(token);
                _cache.Set(cacheKey, isValid, TimeSpan.FromMinutes(5));
            }
        }

        await _next(context);
    }

    // ✅ Add the ValidateToken method
    private async Task<bool> ValidateToken(string token)
    {
        try
        {
            // Get JWT settings from configuration
            var jwtSecret = _configuration["Jwt:SecretKey"] ?? "YourDefaultSecretKeyHereAtLeast32CharactersLong!";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "RST_ERP.Svc.Auth";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "RST_ERP";

            if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
            {
                _logger.LogWarning("⚠️ JWT secret is invalid or too short");
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            try
            {
                // Validate the token
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                }, out SecurityToken validatedToken);

                // Check if token is expired
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var isValid = jwtToken.ValidTo > DateTime.UtcNow;
                    _logger.LogDebug($"✅ Token validation result: {isValid}");
                    return isValid;
                }

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("⚠️ Token has expired");
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                _logger.LogWarning("⚠️ Token has invalid signature");
                return false;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                _logger.LogWarning("⚠️ Token has invalid issuer");
                return false;
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                _logger.LogWarning("⚠️ Token has invalid audience");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error validating token");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error validating token");
            return false;
        }
    }
}