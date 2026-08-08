
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class CoreModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            new() { ModKey = "mod.core", Key = "core.db", Label = "Dashboard", Path = "/core", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },
            new() { ModKey = "mod.core", Key = "core.company", Label = "Companies", Path = "/core/company", Icon = "Building", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.core", Key = "core.branch", Label = "Branches", Path = "/core/branch", Icon = "MapPin", ParKey = "", IsChild = false, Order = 3 },
            new() { ModKey = "mod.core", Key = "core.dept", Label = "Departments", Path = "/core/department", Icon = "Network", ParKey = "", IsChild = false, Order = 4 },
            new() { ModKey = "mod.core", Key = "core.fiscal", Label = "Fiscal Year", Path = "/core/fiscal-year", Icon = "Calendar", ParKey = "", IsChild = false, Order = 5 },
            new() { ModKey = "mod.core", Key = "core.roles", Label = "Roles & Permissions", Path = "/core/roles", Icon = "Shield", ParKey = "", IsChild = false, Order = 6 },
            new() { ModKey = "mod.core", Key = "core.users", Label = "User Management", Path = "/core/users", Icon = "Users", ParKey = "", IsChild = false, Order = 7 },
            new() { ModKey = "mod.core", Key = "core.audit", Label = "Audit Trail", Path = "/core/audit", Icon = "History", ParKey = "", IsChild = false, Order = 8 },
            new() { ModKey = "mod.core", Key = "core.settings", Label = "System Settings", Path = "/core/settings", Icon = "Settings", ParKey = "", IsChild = false, Order = 9 },
            new() { ModKey = "mod.core", Key = "core.backup", Label = "Backup & Restore", Path = "/core/backup", Icon = "Database", ParKey = "", IsChild = false, Order = 10 },
        };
    }
}