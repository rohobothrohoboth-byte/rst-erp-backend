using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class ProjectModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ===== DASHBOARD =====
            new() { ModKey = "mod.prm", Key = "prm.db", Label = "Dashboard", Path = "/project-management", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },

            // ===== PROJECTS =====
            new() { ModKey = "mod.prm", Key = "prm.projects", Label = "Projects", Path = "", Icon = "Briefcase", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.prm", Key = "prm.projects.list", Label = "Project List", Path = "/project-management/projects", Icon = "List", ParKey = "prm.projects", IsChild = true, Order = 1 },
            new() { ModKey = "mod.prm", Key = "prm.projects.create", Label = "Create Project", Path = "/project-management/projects/create", Icon = "Plus", ParKey = "prm.projects", IsChild = true, Order = 2 },
            new() { ModKey = "mod.prm", Key = "prm.projects.template", Label = "Project Templates", Path = "/project-management/templates", Icon = "FileText", ParKey = "prm.projects", IsChild = true, Order = 3 },

            // ===== TASKS =====
            new() { ModKey = "mod.prm", Key = "prm.tasks", Label = "Task Management", Path = "", Icon = "ClipboardList", ParKey = "", IsChild = false, Order = 3 },
            new() { ModKey = "mod.prm", Key = "prm.tasks.my", Label = "My Tasks", Path = "/project-management/tasks/my", Icon = "User", ParKey = "prm.tasks", IsChild = true, Order = 1 },
            new() { ModKey = "mod.prm", Key = "prm.tasks.all", Label = "All Tasks", Path = "/project-management/tasks", Icon = "List", ParKey = "prm.tasks", IsChild = true, Order = 2 },
            new() { ModKey = "mod.prm", Key = "prm.tasks.board", Label = "Task Board", Path = "/project-management/tasks/board", Icon = "Layout", ParKey = "prm.tasks", IsChild = true, Order = 3 },
            new() { ModKey = "mod.prm", Key = "prm.tasks.calendar", Label = "Task Calendar", Path = "/project-management/tasks/calendar", Icon = "Calendar", ParKey = "prm.tasks", IsChild = true, Order = 4 },

            // ===== MILESTONES =====
            new() { ModKey = "mod.prm", Key = "prm.milestones", Label = "Milestones", Path = "/project-management/milestones", Icon = "Flag", ParKey = "", IsChild = false, Order = 4 },

            // ===== TEAM =====
            new() { ModKey = "mod.prm", Key = "prm.team", Label = "Team Management", Path = "", Icon = "Users", ParKey = "", IsChild = false, Order = 5 },
            new() { ModKey = "mod.prm", Key = "prm.team.members", Label = "Team Members", Path = "/project-management/team", Icon = "Users", ParKey = "prm.team", IsChild = true, Order = 1 },
            new() { ModKey = "mod.prm", Key = "prm.team.roles", Label = "Roles & Responsibilities", Path = "/project-management/team/roles", Icon = "Shield", ParKey = "prm.team", IsChild = true, Order = 2 },
            new() { ModKey = "mod.prm", Key = "prm.team.workload", Label = "Workload", Path = "/project-management/team/workload", Icon = "BarChart", ParKey = "prm.team", IsChild = true, Order = 3 },

            // ===== TIMELINE =====
            new() { ModKey = "mod.prm", Key = "prm.timeline", Label = "Timeline", Path = "/project-management/timeline", Icon = "Clock", ParKey = "", IsChild = false, Order = 6 },

            // ===== BUDGET =====
            new() { ModKey = "mod.prm", Key = "prm.budget", Label = "Budget & Costs", Path = "/project-management/budget", Icon = "DollarSign", ParKey = "", IsChild = false, Order = 7 },

            // ===== RISKS =====
            new() { ModKey = "mod.prm", Key = "prm.risks", Label = "Risk Register", Path = "/project-management/risks", Icon = "AlertTriangle", ParKey = "", IsChild = false, Order = 8 },

            // ===== REPORTS =====
            new() { ModKey = "mod.prm", Key = "prm.reports", Label = "Reports", Path = "", Icon = "BarChart4", ParKey = "", IsChild = false, Order = 9 },
            new() { ModKey = "mod.prm", Key = "prm.reports.progress", Label = "Progress Reports", Path = "/project-management/reports/progress", Icon = "TrendingUp", ParKey = "prm.reports", IsChild = true, Order = 1 },
            new() { ModKey = "mod.prm", Key = "prm.reports.time", Label = "Time Reports", Path = "/project-management/reports/time", Icon = "Clock", ParKey = "prm.reports", IsChild = true, Order = 2 },
            new() { ModKey = "mod.prm", Key = "prm.reports.financial", Label = "Financial Reports", Path = "/project-management/reports/financial", Icon = "DollarSign", ParKey = "prm.reports", IsChild = true, Order = 3 },
        };
    }
}