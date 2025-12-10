using Svc.Auth.Models.Entities;

namespace Svc.Auth.Seeder;

public static class PerMenuSeeder
{
    public static IEnumerable<PerMenu> GetPerMenu()
    {
        return new List<PerMenu>
        {
            new() { Key = "menu.hrm", Desc = "HRM Module" },
            new() { Key = "menu.fnm", Desc = "Finance Module" },
            new() { Key = "menu.inv", Desc = "Inventory Module" },
            new() { Key = "menu.crm", Desc = "CRM Module" },
            new() { Key = "menu.pro", Desc = "Procurement Module" },
            new() { Key = "menu.flm", Desc = "File Module" }
        };
    }
}