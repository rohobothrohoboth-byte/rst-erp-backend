using Svc.Auth.Models.Entities;

namespace Svc.Auth.Seeder;

public static class PerModuleSeeder
{
    public static IEnumerable<PerModule> GetPerModule()
    {
        return new List<PerModule>
        {
            new() { Key = "mod.hrm", Desc = "HRM Module" },
            new() { Key = "mod.fnm", Desc = "Finance Module" },
            new() { Key = "mod.inv", Desc = "Inventory Module" },
            new() { Key = "mod.crm", Desc = "CRM Module" },
            new() { Key = "mod.pro", Desc = "Procurement Module" },
            new() { Key = "mod.flm", Desc = "File Module" }
        };
    }
}