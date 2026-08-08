using Helpers;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Seeder;

public class RoleSeeder
{
    public static IEnumerable<AppRole> GetRoles()
    {
        return new List<AppRole>
        {
            new() { Name = "admin", NormalizedName = "ADMIN", Desc = "System Administrator" },
            new() { Name = "pre", NormalizedName = "pre", Desc = "President" },
            new() { Name = "ceo", NormalizedName = "CEO", Desc = "Executive/CEO" },
            new() { Name = "vice", NormalizedName = "VICE", Desc = "Vice" },
            new() { Name = "dir", NormalizedName = "DIR", Desc = "Director" },
            new() { Name = "mgr", NormalizedName = "MGR", Desc = "Manager" },
            new() { Name = "emp", NormalizedName = "EMP", Desc = "Employee" },
            new() { Name = "inte", NormalizedName = "INTE", Desc = "Intern" }
        };
    }

    public static async Task SeedAdmin(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        const string adminUn = "Admin";
        const string adminPw = "Admin123!";

        var admin = await userManager.FindByNameAsync(adminUn);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminUn,
                IsActive = true,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPw);

            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.admin);
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(admin, "admin")) await userManager.AddToRoleAsync(admin, Roles.admin);
        }
    }
}