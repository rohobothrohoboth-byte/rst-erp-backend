using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class FilePermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // Dashboard
            new() { MenuKey = "flm.db", Key = "flm.db.view", Desc = "View File Dashboard" },
            new() { MenuKey = "flm.db", Key = "flm.db.export", Desc = "Export Dashboard Data" },

            // Company Documents
            new() { MenuKey = "flm.company", Key = "flm.company.view", Desc = "Access Company Documents" },
            new() { MenuKey = "flm.company", Key = "flm.company.manage", Desc = "Manage Company Documents" },

            new() { MenuKey = "flm.company.folders", Key = "flm.company.folders.view", Desc = "View Folders" },
            new() { MenuKey = "flm.company.folders", Key = "flm.company.folders.create", Desc = "Create Folder" },
            new() { MenuKey = "flm.company.folders", Key = "flm.company.folders.mod", Desc = "Edit Folder" },
            new() { MenuKey = "flm.company.folders", Key = "flm.company.folders.del", Desc = "Delete Folder" },
            new() { MenuKey = "flm.company.folders", Key = "flm.company.folders.move", Desc = "Move Folder" },
            new() { MenuKey = "flm.company.folders", Key = "flm.company.folders.copy", Desc = "Copy Folder" },

            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.view", Desc = "View Shared Documents" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.upload", Desc = "Upload Document" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.download", Desc = "Download Document" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.del", Desc = "Delete Document" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.share", Desc = "Share Document" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.version", Desc = "Version Control" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.restore", Desc = "Restore Document" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.preview", Desc = "Preview Document" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.rename", Desc = "Rename Document" },
            new() { MenuKey = "flm.company.shared", Key = "flm.company.shared.comment", Desc = "Add Comments" },

            // Personal Documents
            new() { MenuKey = "flm.personal", Key = "flm.personal.view", Desc = "Access Personal Documents" },
            new() { MenuKey = "flm.personal", Key = "flm.personal.manage", Desc = "Manage Personal Documents" },

            new() { MenuKey = "flm.personal.folders", Key = "flm.personal.folders.view", Desc = "View My Folders" },
            new() { MenuKey = "flm.personal.folders", Key = "flm.personal.folders.create", Desc = "Create Folder" },
            new() { MenuKey = "flm.personal.folders", Key = "flm.personal.folders.mod", Desc = "Edit Folder" },
            new() { MenuKey = "flm.personal.folders", Key = "flm.personal.folders.del", Desc = "Delete Folder" },

            new() { MenuKey = "flm.personal.recent", Key = "flm.personal.recent.view", Desc = "View Recent Files" },
            new() { MenuKey = "flm.personal.recent", Key = "flm.personal.recent.upload", Desc = "Upload File" },
            new() { MenuKey = "flm.personal.recent", Key = "flm.personal.recent.download", Desc = "Download File" },
            new() { MenuKey = "flm.personal.recent", Key = "flm.personal.recent.del", Desc = "Delete File" },
            new() { MenuKey = "flm.personal.recent", Key = "flm.personal.recent.share", Desc = "Share File" },
            new() { MenuKey = "flm.personal.recent", Key = "flm.personal.recent.version", Desc = "Version Control" },
            new() { MenuKey = "flm.personal.recent", Key = "flm.personal.recent.favorite", Desc = "Mark as Favorite" },

            // Archive
            new() { MenuKey = "flm.archive", Key = "flm.archive.view", Desc = "View Archive" },
            new() { MenuKey = "flm.archive", Key = "flm.archive.restore", Desc = "Restore from Archive" },
            new() { MenuKey = "flm.archive", Key = "flm.archive.del", Desc = "Delete Permanently" },
            new() { MenuKey = "flm.archive", Key = "flm.archive.search", Desc = "Search Archive" },
            new() { MenuKey = "flm.archive", Key = "flm.archive.export", Desc = "Export Archive" },

            // Settings
            new() { MenuKey = "flm.settings", Key = "flm.settings.view", Desc = "View File Settings" },
            new() { MenuKey = "flm.settings", Key = "flm.settings.mod", Desc = "Edit File Settings" },
            new() { MenuKey = "flm.settings", Key = "flm.settings.storage", Desc = "Storage Settings" },
            new() { MenuKey = "flm.settings", Key = "flm.settings.security", Desc = "Security Settings" },
            new() { MenuKey = "flm.settings", Key = "flm.settings.integration", Desc = "Integration Settings" },
        };
    }
}