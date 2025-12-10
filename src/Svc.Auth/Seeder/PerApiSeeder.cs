using Svc.Auth.Models.Entities;

namespace Svc.Auth.Seeder;

public static class PerApiSeeder
{
    public static IEnumerable<PerApi> GetPerApi()
    {
        return new List<PerApi>
        {
            new() { Key = "hr.employee.view.list", Desc = "View employees list" },
            new() { Key = "hr.employee.view.profile", Desc = "View employee profile" },
            new() { Key = "hr.employee.create", Desc = "Create new employee" },
            new() { Key = "hr.employee.update", Desc = "Update employee information" },
            new() { Key = "hr.employee.delete", Desc = "Delete employee" },
        };
    }
}

