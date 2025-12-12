using Contracts;

namespace Cor.HRMM.Interfaces;

public interface IAuthClient
{
    Task<GetUserResponse> GetUser(string token, CancellationToken ct = default);
}