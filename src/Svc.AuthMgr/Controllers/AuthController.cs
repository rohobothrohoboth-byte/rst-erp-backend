using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Svc.AuthMgr.Dtos;
using Svc.AuthMgr.Persistence;

namespace Svc.AuthMgr.Controllers;

public sealed class AuthController(UserManager<IdentityUser> userManager, AuthDbContext dbContext) : Controller
{
    public IActionResult Register(RegisterDto dto)
    {
        return View();
    }
}