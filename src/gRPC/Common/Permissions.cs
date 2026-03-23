namespace Common;

public static class Permissions
{
    public const string EmpAdd = "emp.add";
    public const string EmpList = "emp.list";
    public const string EmpTest = "emp.test";

    public static readonly string[] All =
    {
        EmpAdd,
        EmpList,
        EmpTest
    };
}

public static class PermissionMap
{
    public static readonly Dictionary<string, int> IndexMap = Permissions.All.Select((p, i) => new { p, i }).ToDictionary(x => x.p, x => x.i);
}
