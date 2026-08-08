// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Authentication\ApiKeyAuthenticationOptions.cs
using Microsoft.AspNetCore.Authentication;

namespace Cor.Inventory.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public string HeaderName { get; set; } = "X-API-Key";
     public bool EnforceEndpointPermissions { get; set; } = true;
}