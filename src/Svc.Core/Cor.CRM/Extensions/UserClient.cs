using Cor.CRM.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Shared.Helpers.Services;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cor.CRM.Extensions;

public class UserClient : IUserClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;  // ✅ Add cache

    public UserClient(
        HttpClient httpClient,
        ILogService logService,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ICacheService cacheService)  // ✅ Inject cache
    {
        _httpClient = httpClient;
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _cacheService = cacheService;

        var baseUrl = _configuration["ServiceUrls:AuthApi"] ?? "https://192.168.1.6:7000";
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    public async Task<UserResponse> GetUser(string userId, CancellationToken ct = default)
    {
        try
        {
            // ✅ Check cache first
            var cacheKey = $"user_{userId}";
            var cachedUser = await _cacheService.GetAsync<UserData>(cacheKey);
            if (cachedUser != null)
            {
                _logService.LogInformation("User retrieved from cache: {UserId}", userId);
                return new UserResponse { Success = true, Res = cachedUser, Message = "User retrieved from cache" };
            }

            var token = GetTokenFromContext();
            if (string.IsNullOrEmpty(token))
            {
                return new UserResponse { Success = false, Message = "No authentication token" };
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/auth/v1/User/{userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                return new UserResponse { Success = false, Message = $"User not found: {response.StatusCode}" };
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserData>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                // ✅ Cache for 5 minutes
                await _cacheService.SetAsync(cacheKey, apiResponse.Data, TimeSpan.FromMinutes(5));
                return new UserResponse
                {
                    Success = true,
                    Res = apiResponse.Data,
                    Message = "User retrieved successfully"
                };
            }

            return new UserResponse { Success = false, Message = apiResponse?.Message ?? "Failed to fetch user" };
        }
        catch (TaskCanceledException)
        {
            return new UserResponse { Success = false, Message = "Request timed out" };
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, "Failed to fetch user: {UserId}", userId);
            return new UserResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<List<UserData>> GetUsers(CancellationToken ct = default)
    {
        try
        {
            // ✅ Cache all users
            var cacheKey = "all_users";
            var cachedUsers = await _cacheService.GetAsync<List<UserData>>(cacheKey);
            if (cachedUsers != null)
            {
                _logService.LogInformation("All users retrieved from cache");
                return cachedUsers;
            }

            var token = GetTokenFromContext();
            if (string.IsNullOrEmpty(token))
            {
                return new List<UserData>();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/v1/User");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                return new List<UserData>();
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<UserData>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                // ✅ Cache for 10 minutes
                await _cacheService.SetAsync(cacheKey, apiResponse.Data, TimeSpan.FromMinutes(10));
                return apiResponse.Data;
            }

            return new List<UserData>();
        }
        catch
        {
            return new List<UserData>();
        }
    }

    private string? GetTokenFromContext()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return null;

        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring(7);
        }

        return null;
    }
}
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; }
}