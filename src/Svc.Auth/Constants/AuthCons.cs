namespace Svc.Auth.Constants;

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
    public const string SecretKey = "http://localhost:1212/";
    public const int ExpiryInMinutes = 30;
    public const int RefreshTokenExpireDays = 7;
}