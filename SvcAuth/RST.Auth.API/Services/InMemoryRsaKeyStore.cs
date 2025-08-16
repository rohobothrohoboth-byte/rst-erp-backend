using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace RST.Auth.API.Services
{
    public class InMemoryRsaKeyStore : IKeyStore
    {
        private readonly ConcurrentDictionary<string, RsaSecurityKey> _keys = new();
        private string _activeKid = "";

        public InMemoryRsaKeyStore()
        {
            var (key, kid) = Generate("dev-k1");
            _keys[kid] = key;
            _activeKid = kid;
        }

        public void LoadFromPem(string privatePem, string? kid = null)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privatePem);
            var key = new RsaSecurityKey(rsa.ExportParameters(true));
            key.KeyId = kid ?? $"kid-{Guid.NewGuid():N}";
            _keys[key.KeyId] = key;
            _activeKid = key.KeyId;
        }

        public (RsaSecurityKey Key, string Kid) GetActiveSigningKey() => (_keys[_activeKid], _activeKid);

        public IEnumerable<(RsaSecurityKey Key, string Kid)> GetAllPublicKeys()
        {
            foreach (var kv in _keys)
            {
                yield return (new RsaSecurityKey(kv.Value.Rsa.ExportParameters(false)) { KeyId = kv.Key }, kv.Key);
            }
        }

        public (RsaSecurityKey Key, string Kid) Rotate()
        {
            var (key, kid) = Generate($"kid-{DateTime.UtcNow:yyyyMMddHHmmss}");
            _keys[kid] = key;
            _activeKid = kid;
            return (key, kid);
        }

        private static (RsaSecurityKey Key, string Kid) Generate(string kid)
        {
            var rsa = RSA.Create(2048);
            return (new RsaSecurityKey(rsa.ExportParameters(true)) { KeyId = kid }, kid);
        }
    }
}
