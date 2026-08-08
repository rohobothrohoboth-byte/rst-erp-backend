using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class PlanDevModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ===== DASHBOARD =====
            new() { ModKey = "mod.pld", Key = "pld.db", Label = "Dashboard", Path = "/plandev", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },

            // ===== STRATEGIC PLANNING =====
            new() { ModKey = "mod.pld", Key = "pld.strategic", Label = "Strategic Planning", Path = "", Icon = "Target", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.pld", Key = "pld.strategic.plans", Label = "Strategic Plans", Path = "/plandev/strategic-plans", Icon = "FileText", ParKey = "pld.strategic", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pld", Key = "pld.strategic.objectives", Label = "Objectives", Path = "/plandev/objectives", Icon = "Target", ParKey = "pld.strategic", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pld", Key = "pld.strategic.kpi", Label = "KPIs", Path = "/plandev/kpis", Icon = "Activity", ParKey = "pld.strategic", IsChild = true, Order = 3 },

            // ===== INITIATIVES =====
            new() { ModKey = "mod.pld", Key = "pld.initiatives", Label = "Initiatives", Path = "", Icon = "Rocket", ParKey = "", IsChild = false, Order = 3 },
            new() { ModKey = "mod.pld", Key = "pld.initiatives.active", Label = "Active Initiatives", Path = "/plandev/initiatives/active", Icon = "Play", ParKey = "pld.initiatives", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pld", Key = "pld.initiatives.completed", Label = "Completed Initiatives", Path = "/plandev/initiatives/completed", Icon = "CheckCircle", ParKey = "pld.initiatives", IsChild = true, Order = 2 },
            new() { ModKey = "mod.pld", Key = "pld.initiatives.budget", Label = "Initiative Budget", Path = "/plandev/initiatives/budget", Icon = "DollarSign", ParKey = "pld.initiatives", IsChild = true, Order = 3 },

            // ===== CALENDAR =====
            new() { ModKey = "mod.pld", Key = "pld.calendar", Label = "Planning Calendar", Path = "/plandev/calendar", Icon = "Calendar", ParKey = "", IsChild = false, Order = 4 },

            // ===== MILESTONES =====
            new() { ModKey = "mod.pld", Key = "pld.milestones", Label = "Milestones", Path = "/plandev/milestones", Icon = "Flag", ParKey = "", IsChild = false, Order = 5 },

            // ===== RISKS =====
            new() { ModKey = "mod.pld", Key = "pld.risks", Label = "Risk Management", Path = "/plandev/risks", Icon = "AlertTriangle", ParKey = "", IsChild = false, Order = 6 },

            // ===== REPORTS =====
            new() { ModKey = "mod.pld", Key = "pld.reports", Label = "Reports", Path = "", Icon = "BarChart4", ParKey = "", IsChild = false, Order = 7 },
            new() { ModKey = "mod.pld", Key = "pld.reports.progress", Label = "Progress Reports", Path = "/plandev/reports/progress", Icon = "TrendingUp", ParKey = "pld.reports", IsChild = true, Order = 1 },
            new() { ModKey = "mod.pld", Key = "pld.reports.performance", Label = "Performance Reports", Path = "/plandev/reports/performance", Icon = "Activity", ParKey = "pld.reports", IsChild = true, Order = 2 },
        };
    }
}