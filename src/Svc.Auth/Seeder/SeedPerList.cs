using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Seeder;

public static class SeedPerList
{
    public static IEnumerable<PerModule> GetPerModule()
    {
        return
        [
            new() { Key = "mod.hrm", Desc = "HR Management" },
            new() { Key = "mod.fnm", Desc = "Finance" },
            new() { Key = "mod.inv", Desc = "Inventory" },
            new() { Key = "mod.crm", Desc = "CRM" },
            new() { Key = "mod.pro", Desc = "Procurement" },
            new() { Key = "mod.pld", Desc = "Plan & Development" },
            new() { Key = "mod.prm", Desc = "Project Management" },
            new() { Key = "mod.flm", Desc = "File Management" }
        ];
    }

    public static IEnumerable<PerMenuSeedDto> GetPerMenu()
    {
        return
        [
            new() {ModKey = "mod.hrm", Key = "hr.db", Label = "Dashboard", Path = "/hr", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },
            new() {ModKey = "mod.hrm", Key = "hr.emp", Label = "Employees", Path = "/hr/employees/record", Icon = "Users", ParKey = "", IsChild = false, Order = 2 },
            new() {ModKey = "mod.hrm", Key = "an.leave", Label = "Annual Leave", Path = "", Icon = "CalendarDays", ParKey = "", IsChild = false, Order = 4 },
            new() {ModKey = "mod.hrm", Key = "my.leave", Label = "My Leave", Path = "/hr/leave/list", Icon = "CalendarDays", ParKey = "an.leave", IsChild = true, Order = 1 }
        ];

        //,
        //    new() {ModKey = "", Key = "", Label = "", Path = "", Icon = "", ParKey = "", IsChild = false, Order = 2 }
    }

    public static IEnumerable<PerAccessSeedDto> GetPerAccess()
    {
        return
        [
            new() {MenuKey = "hr.emp", Key = "hr.emp.view", Desc = "View List" },
            new() {MenuKey = "hr.emp", Key = "hr.emp.add", Desc = "Add New" },
            new() {MenuKey = "hr.emp", Key = "hr.emp.dtl", Desc = "View Details" },
            new() {MenuKey = "hr.emp", Key = "hr.emp.mod", Desc = "Update" },
            new() {MenuKey = "hr.emp", Key = "hr.emp.del", Desc = "Delete" },
            new() {MenuKey = "hr.emp", Key = "hr.emp.rev", Desc = "Review" },

            new() {MenuKey = "my.leave", Key = "my.leave.view", Desc = "View" },
            new() {MenuKey = "my.leave", Key = "my.leave.add", Desc = "Request" },
            new() {MenuKey = "my.leave", Key = "my.leave.mod", Desc = "Update" },
            new() {MenuKey = "my.leave", Key = "my.leave.del", Desc = "Delete" },
            new() {MenuKey = "my.leave", Key = "my.leave.rev", Desc = "Review" }
        ];

        //,
        //    new() {MenuKey = "", Key = "", Desc = "" }
    }
}