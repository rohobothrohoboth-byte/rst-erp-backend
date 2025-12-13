namespace Auth.Security;

public static class JwtCons
{
    public const string Issuer = "RST_ERP.Svc.Auth";
    public const string Audience = "RST_ERP";
    public const string SecretKey = "AQAAAAIAAYagAAAAEJt3GAbxb3pba+j+dAqiRoIoxyMZXBuqP+gw83FBiDy2Z7Cb7h+4UX5ySv0465P9Ww==";
    public const int ExpiryInMinutes = 30;
    public const int RefreshTokenExpireDays = 7;
}