using System.Net.Http;
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
        // Accept whichever key the host service configured (AuthUrl is the legacy
        // gRPC-common key; ServiceUrls:AuthApi is the modern one). Fall back to the
        // standard local port so a missing key degrades gracefully.
        _authUrl = config["AuthUrl"]
            ?? config["ServiceUrls:AuthApi"]
            ?? "https://localhost:7000";
    }

    // The gRPC targets run over HTTPS with a self-signed dev certificate whose name
    // does not match the configured host, so validate leniently (dev/self-hosted).
    private GrpcChannel CreateChannel() => GrpcChannel.ForAddress(_authUrl, new GrpcChannelOptions
    {
        HttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }
    });

    public async Task<GetUserResponse> GetUser(string token, CancellationToken ct = default)
    {
        using var channel = CreateChannel();
        var client = new AuthValidator.AuthValidatorClient(channel);
        return await client.GetUserAsync(new GetUserRequest { Token = token }, cancellationToken: ct);
    }

    public async Task<bool> ValidateToken(string token, CancellationToken ct = default)
    {
        using var channel = CreateChannel();
        var client = new AuthValidator.AuthValidatorClient(channel);
        var request = new ValidateTokenRequest { Token = token };
        var res = await client.ValidateTokenAsync(request, cancellationToken: ct);
        return res.IsValid;
    }



}