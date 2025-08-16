namespace RST.Auth.API.Models
{
    public class RevokedAccessToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Jti { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
    }
}
