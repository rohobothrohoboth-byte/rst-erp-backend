// Common/OrgPermissions.cs (NEW)

namespace Common;

public static class OrgPermissions
{
    // Branch-level permissions
    public const string BranchView = "branch.view";
    public const string BranchCreate = "branch.create";
    public const string BranchEdit = "branch.edit";
    public const string BranchDelete = "branch.delete";

    // Department-level permissions
    public const string DepartmentView = "department.view";
    public const string DepartmentCreate = "department.create";
    public const string DepartmentEdit = "department.edit";
    public const string DepartmentDelete = "department.delete";

    // Position-level permissions
    public const string PositionView = "position.view";
    public const string PositionCreate = "position.create";
    public const string PositionEdit = "position.edit";
    public const string PositionDelete = "position.delete";

    // User management with org scope
    public const string UserViewByBranch = "user.view.branch";
    public const string UserViewByDept = "user.view.dept";
    public const string UserManageOrg = "user.manage.org";
}