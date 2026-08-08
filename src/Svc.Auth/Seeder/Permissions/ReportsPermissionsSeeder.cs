using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class ReportsPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // Dashboard
            new() { MenuKey = "rpt.db", Key = "rpt.db.view", Desc = "View Reports Dashboard" },
            new() { MenuKey = "rpt.db", Key = "rpt.db.export", Desc = "Export Dashboard Data" },

            // HR Reports
            new() { MenuKey = "rpt.hr", Key = "rpt.hr.view", Desc = "View HR Reports" },
            new() { MenuKey = "rpt.hr", Key = "rpt.hr.export", Desc = "Export HR Reports" },
            new() { MenuKey = "rpt.hr", Key = "rpt.hr.schedule", Desc = "Schedule HR Reports" },
            new() { MenuKey = "rpt.hr", Key = "rpt.hr.custom", Desc = "Custom HR Reports" },

            // Finance Reports
            new() { MenuKey = "rpt.finance", Key = "rpt.finance.view", Desc = "View Finance Reports" },
            new() { MenuKey = "rpt.finance", Key = "rpt.finance.export", Desc = "Export Finance Reports" },
            new() { MenuKey = "rpt.finance", Key = "rpt.finance.schedule", Desc = "Schedule Finance Reports" },
            new() { MenuKey = "rpt.finance", Key = "rpt.finance.custom", Desc = "Custom Finance Reports" },

            // Inventory Reports
            new() { MenuKey = "rpt.inventory", Key = "rpt.inventory.view", Desc = "View Inventory Reports" },
            new() { MenuKey = "rpt.inventory", Key = "rpt.inventory.export", Desc = "Export Inventory Reports" },
            new() { MenuKey = "rpt.inventory", Key = "rpt.inventory.schedule", Desc = "Schedule Inventory Reports" },
            new() { MenuKey = "rpt.inventory", Key = "rpt.inventory.custom", Desc = "Custom Inventory Reports" },

            // CRM Reports
            new() { MenuKey = "rpt.crm", Key = "rpt.crm.view", Desc = "View CRM Reports" },
            new() { MenuKey = "rpt.crm", Key = "rpt.crm.export", Desc = "Export CRM Reports" },
            new() { MenuKey = "rpt.crm", Key = "rpt.crm.schedule", Desc = "Schedule CRM Reports" },
            new() { MenuKey = "rpt.crm", Key = "rpt.crm.custom", Desc = "Custom CRM Reports" },

            // Procurement Reports
            new() { MenuKey = "rpt.procurement", Key = "rpt.procurement.view", Desc = "View Procurement Reports" },
            new() { MenuKey = "rpt.procurement", Key = "rpt.procurement.export", Desc = "Export Procurement Reports" },
            new() { MenuKey = "rpt.procurement", Key = "rpt.procurement.schedule", Desc = "Schedule Procurement Reports" },
            new() { MenuKey = "rpt.procurement", Key = "rpt.procurement.custom", Desc = "Custom Procurement Reports" },

            // Project Reports
            new() { MenuKey = "rpt.project", Key = "rpt.project.view", Desc = "View Project Reports" },
            new() { MenuKey = "rpt.project", Key = "rpt.project.export", Desc = "Export Project Reports" },
            new() { MenuKey = "rpt.project", Key = "rpt.project.schedule", Desc = "Schedule Project Reports" },
            new() { MenuKey = "rpt.project", Key = "rpt.project.custom", Desc = "Custom Project Reports" },

            // Custom Reports
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.view", Desc = "View Custom Reports" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.create", Desc = "Create Custom Report" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.mod", Desc = "Edit Custom Report" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.del", Desc = "Delete Custom Report" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.schedule", Desc = "Schedule Report" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.builder", Desc = "Report Builder" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.template", Desc = "Report Templates" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.export", Desc = "Export Custom Reports" },
            new() { MenuKey = "rpt.custom", Key = "rpt.custom.share", Desc = "Share Reports" },

            // Report Scheduler
            new() { MenuKey = "rpt.scheduler", Key = "rpt.scheduler.view", Desc = "View Scheduler" },
            new() { MenuKey = "rpt.scheduler", Key = "rpt.scheduler.add", Desc = "Add Schedule" },
            new() { MenuKey = "rpt.scheduler", Key = "rpt.scheduler.mod", Desc = "Edit Schedule" },
            new() { MenuKey = "rpt.scheduler", Key = "rpt.scheduler.del", Desc = "Delete Schedule" },
            new() { MenuKey = "rpt.scheduler", Key = "rpt.scheduler.enable", Desc = "Enable/Disable Schedule" },
            new() { MenuKey = "rpt.scheduler", Key = "rpt.scheduler.log", Desc = "View Schedule Logs" },
            new() { MenuKey = "rpt.scheduler", Key = "rpt.scheduler.recipient", Desc = "Manage Recipients" },
        };
    }
}