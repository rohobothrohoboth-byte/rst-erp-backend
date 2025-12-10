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
            new() { Name = "ceo", NormalizedName = "CEO", Desc = "Executive/CEO" },
            new() { Name = "vice.ceo", NormalizedName = "VICE.CEO", Desc = "Vice CEO" },
            new() { Name = "hrm.dir", NormalizedName = "HRM.DIR", Desc = "HR Director" },
            new() { Name = "hrm.mgr", NormalizedName = "HRM.MGR", Desc = "HR Manager" },
            new() { Name = "hrm.stf", NormalizedName = "HRM.STF", Desc = "HR Staff" },
            new() { Name = "fin.dir", NormalizedName = "FIN.DIR", Desc = "Finance Director" },
            new() { Name = "fin.mgr", NormalizedName = "FIN.MGR", Desc = "Finance Manager" },
            new() { Name = "fin.stf", NormalizedName = "FIN.STF", Desc = "Finance Staff" },
            new() { Name = "crm.dir", NormalizedName = "CRM.DIR", Desc = "CRM Director" },
            new() { Name = "crm.mgr", NormalizedName = "CRM.MGR", Desc = "CRM Manager" },
            new() { Name = "crm.stf", NormalizedName = "CRM.STF", Desc = "CRM Staff" },
            new() { Name = "inv.dir", NormalizedName = "INV.DIR", Desc = "Inventory Director" },
            new() { Name = "inv.mgr", NormalizedName = "INV.MGR", Desc = "Inventory Manager" },
            new() { Name = "inv.stf", NormalizedName = "INV.STF", Desc = "Inventory Staff" },
            new() { Name = "pro.dir", NormalizedName = "PRO.DIR", Desc = "Procurement Director" },
            new() { Name = "pro.mgr", NormalizedName = "PRO.MGR", Desc = "Procurement Manager" },
            new() { Name = "pro.stf", NormalizedName = "PRO.STF", Desc = "Procurement Staff" },
            new() { Name = "auditor", NormalizedName = "AUDITOR", Desc = "Auditor" },
            new() { Name = "emp", NormalizedName = "EMP", Desc = "Employee" }
        };
    }

    public static async Task SeedAdmin(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        const string adminUn = "Admin";
        const string adminPw = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminUn);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminUn,
                Email = adminUn,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPw);

            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(admin, "admin")) await userManager.AddToRoleAsync(admin, "admin");
        }
    }
}