using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class FileModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            new() { ModKey = "mod.flm", Key = "flm.db", Label = "Dashboard", Path = "/file", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },
            new() { ModKey = "mod.flm", Key = "flm.company", Label = "Company Documents", Path = "", Icon = "Building", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.flm", Key = "flm.company.folders", Label = "Company Folders", Path = "/file/folders/company", Icon = "FolderOpen", ParKey = "flm.company", IsChild = true, Order = 1 },
            new() { ModKey = "mod.flm", Key = "flm.company.shared", Label = "Shared Documents", Path = "/file/documents/shared", Icon = "Share2", ParKey = "flm.company", IsChild = true, Order = 2 },

            new() { ModKey = "mod.flm", Key = "flm.personal", Label = "Personal Documents", Path = "", Icon = "User", ParKey = "", IsChild = false, Order = 3 },
            new() { ModKey = "mod.flm", Key = "flm.personal.folders", Label = "My Folders", Path = "/file/folders/personal", Icon = "Folder", ParKey = "flm.personal", IsChild = true, Order = 1 },
            new() { ModKey = "mod.flm", Key = "flm.personal.recent", Label = "Recent Files", Path = "file/documents/recent", Icon = "History", ParKey = "flm.personal", IsChild = true, Order = 2 },

            new() { ModKey = "mod.flm", Key = "flm.archive", Label = "Archive", Path = "/file/documents/archive", Icon = "Archive", ParKey = "", IsChild = false, Order = 4 },
            new() { ModKey = "mod.flm", Key = "flm.settings", Label = "Settings", Path = "/file/settings", Icon = "Settings", ParKey = "", IsChild = false, Order = 5 },
        };
    }
}