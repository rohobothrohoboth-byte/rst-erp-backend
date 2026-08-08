using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class ReportsModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ===== DASHBOARD =====
            new() { ModKey = "mod.rpt", Key = "rpt.db", Label = "Dashboard", Path = "/reports", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },

            // ===== REPORT TYPES =====
            new() { ModKey = "mod.rpt", Key = "rpt.hr", Label = "HR Reports", Path = "/reports/hr", Icon = "Users", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.rpt", Key = "rpt.finance", Label = "Finance Reports", Path = "/reports/finance", Icon = "DollarSign", ParKey = "", IsChild = false, Order = 3 },
            new() { ModKey = "mod.rpt", Key = "rpt.inventory", Label = "Inventory Reports", Path = "/reports/inventory", Icon = "Package", ParKey = "", IsChild = false, Order = 4 },
            new() { ModKey = "mod.rpt", Key = "rpt.crm", Label = "CRM Reports", Path = "/reports/crm", Icon = "Heart", ParKey = "", IsChild = false, Order = 5 },
            new() { ModKey = "mod.rpt", Key = "rpt.procurement", Label = "Procurement Reports", Path = "/reports/procurement", Icon = "ShoppingCart", ParKey = "", IsChild = false, Order = 6 },
            new() { ModKey = "mod.rpt", Key = "rpt.project", Label = "Project Reports", Path = "/reports/project", Icon = "Briefcase", ParKey = "", IsChild = false, Order = 7 },

            // ===== CUSTOM REPORTS =====
            new() { ModKey = "mod.rpt", Key = "rpt.custom", Label = "Custom Reports", Path = "/reports/custom", Icon = "Settings", ParKey = "", IsChild = false, Order = 8 },

            // ===== SCHEDULER =====
            new() { ModKey = "mod.rpt", Key = "rpt.scheduler", Label = "Report Scheduler", Path = "/reports/scheduler", Icon = "Calendar", ParKey = "", IsChild = false, Order = 9 },
        };
    }
}