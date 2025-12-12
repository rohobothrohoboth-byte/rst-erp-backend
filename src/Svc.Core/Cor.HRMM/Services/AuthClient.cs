using Contracts;
using Cor.HRMM.Interfaces;
using Grpc.Net.Client;

namespace Cor.HRMM.Services;

public class AuthClient : IAuthClient
{
    private readonly string _authUrl;

    public AuthClient(IConfiguration config)
    {
        _authUrl = config["AuthUrl"] ?? throw new InvalidOperationException("ProductServiceAddress not configured");
    }

    public async Task<GetUserResponse> GetUser(string token, CancellationToken ct = default)
    {
        using var channel = GrpcChannel.ForAddress(_authUrl);
        var client = new AuthValidator.AuthValidatorClient(channel);
        return await client.GetUserAsync(new GetUserRequest { Token = token });
    }

    
}
