using Microsoft.AspNetCore.Authorization; // ✅ ይህን ይጨምሩ
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cor.Module.Authentication;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PerAuthAttribute : AuthorizeAttribute // ✅ አሁን ይሰራል
{
    public PerAuthAttribute(string permission)
    {
        Policy = permission;
         Console.WriteLine($"✅ PerAuthAttribute created for: {permission}"); // ← Add this
    }
}