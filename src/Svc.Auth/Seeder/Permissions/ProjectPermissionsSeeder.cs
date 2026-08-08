using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class ProjectPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // Dashboard
            new() { MenuKey = "prm.db", Key = "prm.db.view", Desc = "View Project Dashboard" },
            new() { MenuKey = "prm.db", Key = "prm.db.export", Desc = "Export Dashboard Data" },

            // Projects
            new() { MenuKey = "prm.projects", Key = "prm.projects.view", Desc = "Access Projects" },
            new() { MenuKey = "prm.projects", Key = "prm.projects.manage", Desc = "Manage Projects" },

            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.view", Desc = "View Projects" },
            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.add", Desc = "Create Project" },
            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.mod", Desc = "Edit Project" },
            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.del", Desc = "Delete Project" },
            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.status", Desc = "Update Status" },
            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.archive", Desc = "Archive Project" },
            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.clone", Desc = "Clone Project" },
            new() { MenuKey = "prm.projects.list", Key = "prm.projects.list.export", Desc = "Export Projects" },

            new() { MenuKey = "prm.projects.create", Key = "prm.projects.create.add", Desc = "Create New Project" },
            new() { MenuKey = "prm.projects.create", Key = "prm.projects.create.wizard", Desc = "Project Setup Wizard" },

            new() { MenuKey = "prm.projects.template", Key = "prm.projects.template.view", Desc = "View Templates" },
            new() { MenuKey = "prm.projects.template", Key = "prm.projects.template.add", Desc = "Create Template" },
            new() { MenuKey = "prm.projects.template", Key = "prm.projects.template.mod", Desc = "Edit Template" },
            new() { MenuKey = "prm.projects.template", Key = "prm.projects.template.del", Desc = "Delete Template" },
            new() { MenuKey = "prm.projects.template", Key = "prm.projects.template.apply", Desc = "Apply Template" },

            // Tasks
            new() { MenuKey = "prm.tasks", Key = "prm.tasks.view", Desc = "Access Tasks" },
            new() { MenuKey = "prm.tasks", Key = "prm.tasks.manage", Desc = "Manage Tasks" },

            new() { MenuKey = "prm.tasks.my", Key = "prm.tasks.my.view", Desc = "View My Tasks" },
            new() { MenuKey = "prm.tasks.my", Key = "prm.tasks.my.update", Desc = "Update My Tasks" },
            new() { MenuKey = "prm.tasks.my", Key = "prm.tasks.my.complete", Desc = "Complete Task" },

            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.view", Desc = "View All Tasks" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.add", Desc = "Create Task" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.mod", Desc = "Edit Task" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.del", Desc = "Delete Task" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.assign", Desc = "Assign Task" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.priority", Desc = "Set Priority" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.dependency", Desc = "Set Dependencies" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.export", Desc = "Export Tasks" },
            new() { MenuKey = "prm.tasks.all", Key = "prm.tasks.all.bulk", Desc = "Bulk Operations" },

            new() { MenuKey = "prm.tasks.board", Key = "prm.tasks.board.view", Desc = "View Task Board" },
            new() { MenuKey = "prm.tasks.board", Key = "prm.tasks.board.drag", Desc = "Move Tasks" },
            new() { MenuKey = "prm.tasks.board", Key = "prm.tasks.board.config", Desc = "Configure Board" },

            new() { MenuKey = "prm.tasks.calendar", Key = "prm.tasks.calendar.view", Desc = "View Task Calendar" },
            new() { MenuKey = "prm.tasks.calendar", Key = "prm.tasks.calendar.export", Desc = "Export Calendar" },

            // Milestones
            new() { MenuKey = "prm.milestones", Key = "prm.milestones.view", Desc = "View Milestones" },
            new() { MenuKey = "prm.milestones", Key = "prm.milestones.add", Desc = "Create Milestone" },
            new() { MenuKey = "prm.milestones", Key = "prm.milestones.mod", Desc = "Edit Milestone" },
            new() { MenuKey = "prm.milestones", Key = "prm.milestones.del", Desc = "Delete Milestone" },
            new() { MenuKey = "prm.milestones", Key = "prm.milestones.complete", Desc = "Complete Milestone" },
            new() { MenuKey = "prm.milestones", Key = "prm.milestones.gantt", Desc = "Gantt Chart View" },

            // Team Management
            new() { MenuKey = "prm.team", Key = "prm.team.view", Desc = "Access Team Management" },
            new() { MenuKey = "prm.team", Key = "prm.team.manage", Desc = "Manage Team" },

            new() { MenuKey = "prm.team.members", Key = "prm.team.members.view", Desc = "View Team Members" },
            new() { MenuKey = "prm.team.members", Key = "prm.team.members.add", Desc = "Add Member" },
            new() { MenuKey = "prm.team.members", Key = "prm.team.members.remove", Desc = "Remove Member" },
            new() { MenuKey = "prm.team.members", Key = "prm.team.members.role", Desc = "Change Role" },

            new() { MenuKey = "prm.team.roles", Key = "prm.team.roles.view", Desc = "View Roles" },
            new() { MenuKey = "prm.team.roles", Key = "prm.team.roles.assign", Desc = "Assign Roles" },
            new() { MenuKey = "prm.team.roles", Key = "prm.team.roles.permissions", Desc = "Set Permissions" },

            new() { MenuKey = "prm.team.workload", Key = "prm.team.workload.view", Desc = "View Workload" },
            new() { MenuKey = "prm.team.workload", Key = "prm.team.workload.balance", Desc = "Balance Workload" },
            new() { MenuKey = "prm.team.workload", Key = "prm.team.workload.report", Desc = "Workload Report" },

            // Timeline & Budget
            new() { MenuKey = "prm.timeline", Key = "prm.timeline.view", Desc = "View Timeline" },
            new() { MenuKey = "prm.timeline", Key = "prm.timeline.mod", Desc = "Edit Timeline" },
            new() { MenuKey = "prm.timeline", Key = "prm.timeline.export", Desc = "Export Timeline" },
            new() { MenuKey = "prm.timeline", Key = "prm.timeline.critical", Desc = "Critical Path" },

            new() { MenuKey = "prm.budget", Key = "prm.budget.view", Desc = "View Budget" },
            new() { MenuKey = "prm.budget", Key = "prm.budget.mod", Desc = "Manage Budget" },
            new() { MenuKey = "prm.budget", Key = "prm.budget.track", Desc = "Track Costs" },
            new() { MenuKey = "prm.budget", Key = "prm.budget.variance", Desc = "Budget Variance" },
            new() { MenuKey = "prm.budget", Key = "prm.budget.forecast", Desc = "Cost Forecast" },

            new() { MenuKey = "prm.risks", Key = "prm.risks.view", Desc = "View Risks" },
            new() { MenuKey = "prm.risks", Key = "prm.risks.add", Desc = "Create Risk" },
            new() { MenuKey = "prm.risks", Key = "prm.risks.mod", Desc = "Edit Risk" },
            new() { MenuKey = "prm.risks", Key = "prm.risks.assess", Desc = "Assess Risk" },
            new() { MenuKey = "prm.risks", Key = "prm.risks.mitigate", Desc = "Mitigate Risk" },
            new() { MenuKey = "prm.risks", Key = "prm.risks.register", Desc = "Risk Register" },

            // Reports
            new() { MenuKey = "prm.reports", Key = "prm.reports.view", Desc = "Access Project Reports" },
            new() { MenuKey = "prm.reports", Key = "prm.reports.export", Desc = "Export Reports" },
            new() { MenuKey = "prm.reports", Key = "prm.reports.schedule", Desc = "Schedule Reports" },

            new() { MenuKey = "prm.reports.progress", Key = "prm.reports.progress.view", Desc = "View Progress Reports" },
            new() { MenuKey = "prm.reports.progress", Key = "prm.reports.progress.export", Desc = "Export Progress Reports" },
            new() { MenuKey = "prm.reports.progress", Key = "prm.reports.progress.status", Desc = "Status Reports" },

            new() { MenuKey = "prm.reports.time", Key = "prm.reports.time.view", Desc = "View Time Reports" },
            new() { MenuKey = "prm.reports.time", Key = "prm.reports.time.export", Desc = "Export Time Reports" },
            new() { MenuKey = "prm.reports.time", Key = "prm.reports.time.timesheet", Desc = "Timesheet Reports" },

            new() { MenuKey = "prm.reports.financial", Key = "prm.reports.financial.view", Desc = "View Financial Reports" },
            new() { MenuKey = "prm.reports.financial", Key = "prm.reports.financial.export", Desc = "Export Financial Reports" },
            new() { MenuKey = "prm.reports.financial", Key = "prm.reports.financial.cost", Desc = "Cost Reports" },
        };
    }
}