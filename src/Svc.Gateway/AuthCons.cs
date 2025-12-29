namespace Svc.Gateway;

public static class AuthCons
{
    public const string EmployeeId = "employeeId";
    public const string UserId = "userId";
    public const string UserName = "userName";
    public const string Role = "role";
    public const string PerModule = "perModule";
    public const string PerMenu = "perMenu";
    public const string PerApi = "perApi";
}

public static class JwtCons
{
    public const string Issuer = "RST_ERP.Svc.Auth";
    public const string Audience = "RST_ERP";
    public const string SecretKey = "AQAAAAIAAYagAAAAEJt3GAbxb3pba+j+dAqiRoIoxyMZXBuqP+gw83FBiDy2Z7Cb7h+4UX5ySv0465P9Ww==";
    public const int ExpiryInMinutes = 30;
    public const int RefreshTokenExpireDays = 7;
}