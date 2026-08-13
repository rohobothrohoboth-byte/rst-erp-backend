using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Cor.Module.Services;

namespace Cor.Module.Authentication;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PermissionPolicyProvider> _logger;

    public PermissionPolicyProvider(
        IServiceProvider serviceProvider,
        ILogger<PermissionPolicyProvider> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        _logger.LogInformation("📌 GetDefaultPolicyAsync called");
        return Task.FromResult(new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build());
    }

   public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
   {


    Console.WriteLine("========================================");
    Console.WriteLine($"🚨🚨🚨 GetPolicyAsync CALLED for: '{policyName}'");
    Console.WriteLine($"🚨🚨🚨 Stack trace: {Environment.StackTrace}");
    Console.WriteLine("========================================");
       _logger.LogInformation($"🔍🔍🔍 GetPolicyAsync called for: '{policyName}'");

       // If policyName is empty or null, return default
       if (string.IsNullOrEmpty(policyName))
       {
           _logger.LogWarning("⚠️ Policy name is null or empty, returning default");
           return await GetDefaultPolicyAsync();
       }

       // ✅ Strip "api:" prefix if present (from PerAuthAttribute)
       var permission = policyName;
       if (policyName.StartsWith("api:"))
       {
           permission = policyName.Substring(4);
           _logger.LogInformation($"📌 Stripped 'api:' prefix -> permission: {permission}");
       }
       else
       {
           _logger.LogInformation($"📌 No 'api:' prefix found, using: {permission}");
       }

       using var scope = _serviceProvider.CreateScope();
       var externalSystemService = scope.ServiceProvider.GetRequiredService<IExternalSystemService>();

       var policy = new AuthorizationPolicyBuilder()
           .RequireAssertion(async context =>
           {
               _logger.LogInformation($"🔍🔍🔍 ASSERTION STARTED for: {permission}");

               // ✅ Log EVERYTHING
               _logger.LogInformation($"📋 IsAuthenticated: {context.User.Identity?.IsAuthenticated}");
               _logger.LogInformation($"📋 Identity Name: {context.User.Identity?.Name}");

               // Log all claims
               var allClaims = string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"));
               _logger.LogInformation($"📋 All Claims: {allClaims}");

               // 1. Check if authenticated
               if (!context.User.Identity?.IsAuthenticated ?? true)
               {
                   _logger.LogWarning("⚠️ User is not authenticated");
                   return false;
               }

               // 2. Check if Internal - THIS SHOULD WORK
               var isInternal = context.User.HasClaim("IsInternal", "true");
               _logger.LogInformation($"📌 IsInternal claim found: {isInternal}");

               if (isInternal)
               {
                   _logger.LogInformation($"✅✅✅ INTERNAL CLIENT - GRANTING PERMISSION: {permission}");
                   return true;
               }

               // 3. Check exact permission claim
               var hasPermission = context.User.HasClaim("Permission", permission);
               _logger.LogInformation($"📌 Has exact permission '{permission}': {hasPermission}");

               if (hasPermission)
               {
                   _logger.LogInformation($"✅✅✅ PERMISSION GRANTED FROM CLAIM: {permission}");
                   return true;
               }

               // 4. Check wildcard
               var hasWildcard = context.User.HasClaim("Permission", "*");
               _logger.LogInformation($"📌 Has wildcard: {hasWildcard}");

               if (hasWildcard)
               {
                   _logger.LogInformation($"✅✅✅ PERMISSION GRANTED FROM WILDCARD: {permission}");
                   return true;
               }

               _logger.LogWarning($"❌❌❌ PERMISSION DENIED for: {permission}");
               return false;
           })
           .Build();

       return policy;
   }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        _logger.LogInformation("📌 GetFallbackPolicyAsync called");
        return Task.FromResult(new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build());
    }
}