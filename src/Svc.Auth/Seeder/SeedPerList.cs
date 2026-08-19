// Svc.Auth/Seeder/SeedPerList.cs
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Seeder.Modules;
using Svc.Auth.Seeder.Permissions;

namespace Svc.Auth.Seeder;

public static class SeedPerList
{
    // ==================== MODULES ====================
    public static IEnumerable<PerModuleSeedDto> GetPerModuleDtos()
    {
        Console.WriteLine("?? GetPerModuleDtos called");
        return new List<PerModuleSeedDto>
        {
            new() { Key = "mod.core", Desc = "Core System Management", Icon = "Settings", Order = 1 },
            new() { Key = "mod.hrm", Desc = "Human Resource Management", Icon = "Users", Order = 2 },
            new() { Key = "mod.fnm", Desc = "Financial Management", Icon = "DollarSign", Order = 3 },
            new() { Key = "mod.inv", Desc = "Inventory Management", Icon = "Package", Order = 4 },
            new() { Key = "mod.crm", Desc = "Customer Relationship Management", Icon = "Heart", Order = 5 },
            new() { Key = "mod.pro", Desc = "Procurement Management", Icon = "ShoppingCart", Order = 6 },
            new() { Key = "mod.pld", Desc = "Plan & Development", Icon = "Target", Order = 7 },
            new() { Key = "mod.prm", Desc = "Project Management", Icon = "Briefcase", Order = 8 },
            new() { Key = "mod.flm", Desc = "File Management", Icon = "Folder", Order = 9 },
            new() { Key = "mod.rpt", Desc = "Reports & Analytics", Icon = "BarChart", Order = 10 }
        };
    }

    public static IEnumerable<PerModule> GetPerModule()
    {
        // Keep Icon/Order in sync with GetPerModuleDtos so seeded modules
        // have proper sidebar icons and ordering.
        return new List<PerModule>
        {
            new() { Key = "mod.core", Desc = "Core System Management", Icon = "Settings", Order = 1 },
            new() { Key = "mod.hrm", Desc = "Human Resource Management", Icon = "Users", Order = 2 },
            new() { Key = "mod.fnm", Desc = "Financial Management", Icon = "DollarSign", Order = 3 },
            new() { Key = "mod.inv", Desc = "Inventory Management", Icon = "Package", Order = 4 },
            new() { Key = "mod.crm", Desc = "Customer Relationship Management", Icon = "Heart", Order = 5 },
            new() { Key = "mod.pro", Desc = "Procurement Management", Icon = "ShoppingCart", Order = 6 },
            new() { Key = "mod.pld", Desc = "Plan & Development", Icon = "Target", Order = 7 },
            new() { Key = "mod.prm", Desc = "Project Management", Icon = "Briefcase", Order = 8 },
            new() { Key = "mod.flm", Desc = "File Management", Icon = "Folder", Order = 9 },
            new() { Key = "mod.rpt", Desc = "Reports & Analytics", Icon = "BarChart", Order = 10 }
        };
    }

    // ==================== MENUS ====================
    public static IEnumerable<PerMenuSeedDto> GetPerMenu()
    {
        Console.WriteLine("?? GetPerMenu called - Building menu list from all seeders...");

        var menus = new List<PerMenuSeedDto>();

        // Add all module menus
        Console.WriteLine("   Adding CoreModuleSeeder...");
        menus.AddRange(CoreModuleSeeder.GetMenus());
        Console.WriteLine($"   CoreModuleSeeder added {CoreModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding HrModuleSeeder...");
        menus.AddRange(HrModuleSeeder.GetMenus());
        Console.WriteLine($"   HrModuleSeeder added {HrModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding FinanceModuleSeeder...");
        var financeMenus = FinanceModuleSeeder.GetMenus().ToList();
        menus.AddRange(financeMenus);
        Console.WriteLine($"   FinanceModuleSeeder added {financeMenus.Count} menus");

        Console.WriteLine("   Adding InventoryModuleSeeder...");
        menus.AddRange(InventoryModuleSeeder.GetMenus());
        Console.WriteLine($"   InventoryModuleSeeder added {InventoryModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding CrmModuleSeeder...");
        menus.AddRange(CrmModuleSeeder.GetMenus());
        Console.WriteLine($"   CrmModuleSeeder added {CrmModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding ProcurementModuleSeeder...");
        menus.AddRange(ProcurementModuleSeeder.GetMenus());
        Console.WriteLine($"   ProcurementModuleSeeder added {ProcurementModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding PlanDevModuleSeeder...");
        menus.AddRange(PlanDevModuleSeeder.GetMenus());
        Console.WriteLine($"   PlanDevModuleSeeder added {PlanDevModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding ProjectModuleSeeder...");
        menus.AddRange(ProjectModuleSeeder.GetMenus());
        Console.WriteLine($"   ProjectModuleSeeder added {ProjectModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding FileModuleSeeder...");
        menus.AddRange(FileModuleSeeder.GetMenus());
        Console.WriteLine($"   FileModuleSeeder added {FileModuleSeeder.GetMenus().Count()} menus");

        Console.WriteLine("   Adding ReportsModuleSeeder...");
        menus.AddRange(ReportsModuleSeeder.GetMenus());
        Console.WriteLine($"   ReportsModuleSeeder added {ReportsModuleSeeder.GetMenus().Count()} menus");

        // Debug: Log finance menus
        var financeMenusList = menus.Where(m => m.Key.StartsWith("fnm.") || m.Key == "fnm.db").ToList();
        Console.WriteLine($"?? Total menus from all seeders: {menus.Count}");
        Console.WriteLine($"?? Finance menus found in SeedPerList: {financeMenusList.Count}");

        if (financeMenusList.Any())
        {
            Console.WriteLine("   Finance menus:");
            foreach (var menu in financeMenusList)
            {
                Console.WriteLine($"      - {menu.Key} (Module: {menu.ModKey}, IsChild: {menu.IsChild})");
            }
        }
        else
        {
            Console.WriteLine("   ?? NO FINANCE MENUS FOUND IN SEED LIST!");
        }

        return menus;
    }

    // ==================== PERMISSIONS ====================
    public static IEnumerable<PerAccessSeedDto> GetPerAccess()
    {
        Console.WriteLine("?? GetPerAccess called - Building permissions list from all seeders...");

        var accesses = new List<PerAccessSeedDto>();

        // Add all module permissions
        accesses.AddRange(CorePermissionsSeeder.GetPermissions());
        accesses.AddRange(HrPermissionsSeeder.GetPermissions());
        accesses.AddRange(RecruitmentPermissionsSeeder.GetPermissions());
        accesses.AddRange(FinancePermissionsSeeder.GetPermissions());
        accesses.AddRange(InventoryPermissionsSeeder.GetPermissions());
        accesses.AddRange(CrmPermissionsSeeder.GetPermissions());
        accesses.AddRange(ProcurementPermissionsSeeder.GetPermissions());
        accesses.AddRange(PlanDevPermissionsSeeder.GetPermissions());
        accesses.AddRange(ProjectPermissionsSeeder.GetPermissions());
        accesses.AddRange(FilePermissionsSeeder.GetPermissions());
        accesses.AddRange(ReportsPermissionsSeeder.GetPermissions());

        // Debug: Log finance permissions
        var financePerms = accesses.Where(p => p.Key.StartsWith("fnm.")).ToList();
        Console.WriteLine($"?? Total permissions from all seeders: {accesses.Count}");
        Console.WriteLine($"?? Finance permissions found: {financePerms.Count}");

        return accesses;
    }
}