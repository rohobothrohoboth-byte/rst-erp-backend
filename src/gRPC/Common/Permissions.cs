// Common/Permissions.cs (UPDATED)

namespace Common;

public static class Permissions
{
    // Employee Permissions
    public const string EmpAdd = "emp.add";
    public const string EmpList = "emp.list";
    public const string EmpEdit = "emp.edit";
    public const string EmpDelete = "emp.delete";
    public const string EmpView = "emp.view";
    public const string EmpTest = "emp.test";
    
    // User Management
    public const string UserView = "user.view";
    public const string UserViewBranch = "user.view.branch";
    public const string UserViewDept = "user.view.dept";
    public const string UserManage = "user.manage";
    
    // Organization Permissions
    public const string BranchView = "branch.view";
    public const string BranchManage = "branch.manage";
    public const string DepartmentView = "department.view";
    public const string DepartmentManage = "department.manage";
    public const string PositionView = "position.view";
    public const string PositionManage = "position.manage";

    public static readonly string[] All =
    {
        EmpAdd,
        EmpList,
        EmpEdit,
        EmpDelete,
        EmpView,
        EmpTest,
        UserView,
        UserViewBranch,
        UserViewDept,
        UserManage,
        BranchView,
        BranchManage,
        DepartmentView,
        DepartmentManage,
        PositionView,
        PositionManage
    };
}
public static class PermissionMap
{
    private static Dictionary<string, int> _indexMap = BuildMap(Permissions.All);
    private static string[] _orderedKeys = Permissions.All;

    // The authoritative permission -> bit-index map used by the JWT `ph` bitmask
    // and the [PerAuth] policy provider.
    public static IReadOnlyDictionary<string, int> IndexMap => _indexMap;

    // Deterministically ordered permission keys backing the index map.
    public static IReadOnlyList<string> OrderedKeys => _orderedKeys;

    private static Dictionary<string, int> BuildMap(IEnumerable<string> keys) =>
        keys.Select((p, i) => new { p, i }).ToDictionary(x => x.p, x => x.i, StringComparer.Ordinal);

    // Initialize the registry from the authoritative permission set (the seeded
    // permission keys), unioned with the legacy static keys and deterministically
    // ordered (ordinal) so bit indices are identical across restarts and services
    // that initialize from the same key set.
    public static void Initialize(IEnumerable<string> permissionKeys)
    {
        var ordered = Permissions.All
            .Concat(permissionKeys ?? Enumerable.Empty<string>())
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToArray();
        _orderedKeys = ordered;
        _indexMap = BuildMap(ordered);
    }
}