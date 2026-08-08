using Contracts;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Common;

public interface IAuthClient
{
    Task<GetUserResponse> GetUser(string token, CancellationToken ct = default);
    Task<bool> ValidateToken(string token, CancellationToken ct = default);
}

public class AuthClient : IAuthClient
{
    private readonly string _authUrl;

    public AuthClient(IConfiguration config)
    {
        _authUrl = config["AuthUrl"] ?? throw new InvalidOperationException("AUTH Service Address not configured");
    }

    public async Task<GetUserResponse> GetUser(string token, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_authUrl);
        var client = new AuthValidator.AuthValidatorClient(channel);
        return await client.GetUserAsync(new GetUserRequest { Token = token });
    }

    public async Task<bool> ValidateToken(string token, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_authUrl);
        var client = new AuthValidator.AuthValidatorClient(channel);
        var request = new ValidateTokenRequest { Token = token };
        var res = await client.ValidateTokenAsync(request);
        return res.IsValid;
    }



}