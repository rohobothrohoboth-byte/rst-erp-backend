using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class CorePermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // Dashboard
            new() { MenuKey = "core.db", Key = "core.db.view", Desc = "View Dashboard" },

            // Company Management
            new() { MenuKey = "core.company", Key = "core.company.view", Desc = "View Companies" },
            new() { MenuKey = "core.company", Key = "core.company.add", Desc = "Create Company" },
            new() { MenuKey = "core.company", Key = "core.company.mod", Desc = "Edit Company" },
            new() { MenuKey = "core.company", Key = "core.company.del", Desc = "Delete Company" },

            // Branch Management
            new() { MenuKey = "core.branch", Key = "core.branch.view", Desc = "View Branches" },
            new() { MenuKey = "core.branch", Key = "core.branch.add", Desc = "Create Branch" },
            new() { MenuKey = "core.branch", Key = "core.branch.mod", Desc = "Edit Branch" },
            new() { MenuKey = "core.branch", Key = "core.branch.del", Desc = "Delete Branch" },

            // Department Management
            new() { MenuKey = "core.dept", Key = "core.dept.view", Desc = "View Departments" },
            new() { MenuKey = "core.dept", Key = "core.dept.add", Desc = "Create Department" },
            new() { MenuKey = "core.dept", Key = "core.dept.mod", Desc = "Edit Department" },
            new() { MenuKey = "core.dept", Key = "core.dept.del", Desc = "Delete Department" },

            // Fiscal Year
            new() { MenuKey = "core.fiscal", Key = "core.fiscal.view", Desc = "View Fiscal Years" },
            new() { MenuKey = "core.fiscal", Key = "core.fiscal.add", Desc = "Create Fiscal Year" },
            new() { MenuKey = "core.fiscal", Key = "core.fiscal.mod", Desc = "Edit Fiscal Year" },
            new() { MenuKey = "core.fiscal", Key = "core.fiscal.del", Desc = "Delete Fiscal Year" },

            // Roles & Permissions
            new() { MenuKey = "core.roles", Key = "core.roles.view", Desc = "View Roles" },
            new() { MenuKey = "core.roles", Key = "core.roles.add", Desc = "Create Role" },
            new() { MenuKey = "core.roles", Key = "core.roles.mod", Desc = "Edit Role" },
            new() { MenuKey = "core.roles", Key = "core.roles.del", Desc = "Delete Role" },
            new() { MenuKey = "core.roles", Key = "core.roles.assign", Desc = "Assign Permissions" },

            // User Management
            new() { MenuKey = "core.users", Key = "core.users.view", Desc = "View Users" },
            new() { MenuKey = "core.users", Key = "core.users.add", Desc = "Create User" },
            new() { MenuKey = "core.users", Key = "core.users.mod", Desc = "Edit User" },
            new() { MenuKey = "core.users", Key = "core.users.del", Desc = "Delete User" },
            new() { MenuKey = "core.users", Key = "core.users.perm", Desc = "Manage User Permissions" },
            new() { MenuKey = "core.users", Key = "core.users.reset", Desc = "Reset Password" },

            // Audit Trail
            new() { MenuKey = "core.audit", Key = "core.audit.view", Desc = "View Audit Logs" },

            // System Settings
            new() { MenuKey = "core.settings", Key = "core.settings.view", Desc = "View Settings" },
            new() { MenuKey = "core.settings", Key = "core.settings.mod", Desc = "Update Settings" },

            // Backup & Restore
            new() { MenuKey = "core.backup", Key = "core.backup.view", Desc = "View Backups" },
            new() { MenuKey = "core.backup", Key = "core.backup.create", Desc = "Create Backup" },
            new() { MenuKey = "core.backup", Key = "core.backup.restore", Desc = "Restore Backup" },
        };
    }
}