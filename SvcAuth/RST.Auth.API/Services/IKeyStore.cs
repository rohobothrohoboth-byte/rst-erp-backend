using Microsoft.IdentityModel.Tokens;

namespace RST.Auth.API.Services
{
    public interface IKeyStore
    {
        (RsaSecurityKey Key, string Kid) GetActiveSigningKey();
        IEnumerable<(RsaSecurityKey Key, string Kid)> GetAllPublicKeys();
        (RsaSecurityKey Key, string Kid) Rotate();
        void LoadFromPem(string privatePem, string? kid = null);
    }
}
