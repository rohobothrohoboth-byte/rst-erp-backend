using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class PlanDevPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // Dashboard
            new() { MenuKey = "pld.db", Key = "pld.db.view", Desc = "View Plan & Development Dashboard" },
            new() { MenuKey = "pld.db", Key = "pld.db.export", Desc = "Export Dashboard Data" },

            // Strategic Planning
            new() { MenuKey = "pld.strategic", Key = "pld.strategic.view", Desc = "Access Strategic Planning" },
            new() { MenuKey = "pld.strategic", Key = "pld.strategic.manage", Desc = "Manage Strategic Planning" },

            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.view", Desc = "View Strategic Plans" },
            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.add", Desc = "Create Plan" },
            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.mod", Desc = "Edit Plan" },
            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.del", Desc = "Delete Plan" },
            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.approve", Desc = "Approve Plan" },
            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.archive", Desc = "Archive Plan" },
            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.version", Desc = "Manage Versions" },
            new() { MenuKey = "pld.strategic.plans", Key = "pld.strategic.plans.export", Desc = "Export Plan" },

            new() { MenuKey = "pld.strategic.objectives", Key = "pld.strategic.objectives.view", Desc = "View Objectives" },
            new() { MenuKey = "pld.strategic.objectives", Key = "pld.strategic.objectives.add", Desc = "Create Objective" },
            new() { MenuKey = "pld.strategic.objectives", Key = "pld.strategic.objectives.mod", Desc = "Edit Objective" },
            new() { MenuKey = "pld.strategic.objectives", Key = "pld.strategic.objectives.del", Desc = "Delete Objective" },
            new() { MenuKey = "pld.strategic.objectives", Key = "pld.strategic.objectives.align", Desc = "Align with Plan" },
            new() { MenuKey = "pld.strategic.objectives", Key = "pld.strategic.objectives.progress", Desc = "Update Progress" },

            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.view", Desc = "View KPIs" },
            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.add", Desc = "Create KPI" },
            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.mod", Desc = "Edit KPI" },
            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.del", Desc = "Delete KPI" },
            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.track", Desc = "Track KPI" },
            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.target", Desc = "Set Targets" },
            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.dashboard", Desc = "KPI Dashboard" },
            new() { MenuKey = "pld.strategic.kpi", Key = "pld.strategic.kpi.export", Desc = "Export KPIs" },

            // Initiatives
            new() { MenuKey = "pld.initiatives", Key = "pld.initiatives.view", Desc = "Access Initiatives" },
            new() { MenuKey = "pld.initiatives", Key = "pld.initiatives.manage", Desc = "Manage Initiatives" },

            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.view", Desc = "View Active Initiatives" },
            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.add", Desc = "Create Initiative" },
            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.mod", Desc = "Edit Initiative" },
            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.del", Desc = "Delete Initiative" },
            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.update", Desc = "Update Progress" },
            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.assign", Desc = "Assign Resources" },
            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.complete", Desc = "Mark Complete" },
            new() { MenuKey = "pld.initiatives.active", Key = "pld.initiatives.active.export", Desc = "Export Initiatives" },

            new() { MenuKey = "pld.initiatives.completed", Key = "pld.initiatives.completed.view", Desc = "View Completed Initiatives" },
            new() { MenuKey = "pld.initiatives.completed", Key = "pld.initiatives.completed.review", Desc = "Review Completed" },
            new() { MenuKey = "pld.initiatives.completed", Key = "pld.initiatives.completed.lessons", Desc = "Capture Lessons Learned" },

            new() { MenuKey = "pld.initiatives.budget", Key = "pld.initiatives.budget.view", Desc = "View Initiative Budget" },
            new() { MenuKey = "pld.initiatives.budget", Key = "pld.initiatives.budget.mod", Desc = "Manage Budget" },
            new() { MenuKey = "pld.initiatives.budget", Key = "pld.initiatives.budget.track", Desc = "Track Expenses" },
            new() { MenuKey = "pld.initiatives.budget", Key = "pld.initiatives.budget.variance", Desc = "Budget Variance" },

            // Calendar & Milestones
            new() { MenuKey = "pld.calendar", Key = "pld.calendar.view", Desc = "View Planning Calendar" },
            new() { MenuKey = "pld.calendar", Key = "pld.calendar.mod", Desc = "Edit Calendar" },
            new() { MenuKey = "pld.calendar", Key = "pld.calendar.export", Desc = "Export Calendar" },
            new() { MenuKey = "pld.calendar", Key = "pld.calendar.sync", Desc = "Sync Calendar" },

            new() { MenuKey = "pld.milestones", Key = "pld.milestones.view", Desc = "View Milestones" },
            new() { MenuKey = "pld.milestones", Key = "pld.milestones.add", Desc = "Create Milestone" },
            new() { MenuKey = "pld.milestones", Key = "pld.milestones.mod", Desc = "Edit Milestone" },
            new() { MenuKey = "pld.milestones", Key = "pld.milestones.del", Desc = "Delete Milestone" },
            new() { MenuKey = "pld.milestones", Key = "pld.milestones.complete", Desc = "Complete Milestone" },
            new() { MenuKey = "pld.milestones", Key = "pld.milestones.dependency", Desc = "Set Dependencies" },

            // Risk Management
            new() { MenuKey = "pld.risks", Key = "pld.risks.view", Desc = "View Risks" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.add", Desc = "Create Risk" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.mod", Desc = "Edit Risk" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.del", Desc = "Delete Risk" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.assess", Desc = "Assess Risk" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.mitigate", Desc = "Mitigate Risk" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.matrix", Desc = "Risk Matrix" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.register", Desc = "Risk Register" },
            new() { MenuKey = "pld.risks", Key = "pld.risks.export", Desc = "Export Risks" },

            // Reports
            new() { MenuKey = "pld.reports", Key = "pld.reports.view", Desc = "Access Plan & Dev Reports" },
            new() { MenuKey = "pld.reports", Key = "pld.reports.export", Desc = "Export Reports" },
            new() { MenuKey = "pld.reports", Key = "pld.reports.schedule", Desc = "Schedule Reports" },

            new() { MenuKey = "pld.reports.progress", Key = "pld.reports.progress.view", Desc = "View Progress Reports" },
            new() { MenuKey = "pld.reports.progress", Key = "pld.reports.progress.export", Desc = "Export Progress Reports" },
            new() { MenuKey = "pld.reports.progress", Key = "pld.reports.progress.dashboard", Desc = "Progress Dashboard" },

            new() { MenuKey = "pld.reports.performance", Key = "pld.reports.performance.view", Desc = "View Performance Reports" },
            new() { MenuKey = "pld.reports.performance", Key = "pld.reports.performance.export", Desc = "Export Performance Reports" },
            new() { MenuKey = "pld.reports.performance", Key = "pld.reports.performance.scorecard", Desc = "Balanced Scorecard" },
        };
    }
}