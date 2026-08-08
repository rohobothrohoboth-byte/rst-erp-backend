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
    public static readonly Dictionary<string, int> IndexMap = Permissions.All.Select((p, i) => new { p, i }).ToDictionary(x => x.p, x => x.i);
}