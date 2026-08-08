// Constants/CacheKeys.cs
namespace Cor.HRMM.Constants;

public static class CacheKeys
{
    // Master data keys
    public const string AllPositions = "AllPositions";
    public const string AllBranches = "AllBranches";
    public const string AllJobGrades = "AllJobGrades";
    public const string AllJgSteps = "AllJgSteps";
    public const string AllDepartments = "AllDepartments";
    public const string AllCompanies = "AllCompanies";
    public const string AllEmployees = "AllEmployees";

    // Individual item keys
    public static string Position(Guid id) => $"Position:{id}";
    public static string JobGrade(Guid id) => $"JobGrade:{id}";
    public static string JgStep(Guid id) => $"JgStep:{id}";
    public static string Branch(Guid id) => $"Branch:{id}";
    public static string Department(Guid id) => $"Department:{id}";
    public static string Company(Guid id) => $"Company:{id}";
    public static string Employee(Guid id) => $"Employee:{id}";

    // Filtered lists
    public static string JgStepsByJobGrade(Guid jobGradeId) => $"JgSteps:JobGrade:{jobGradeId}";
    public static string PositionsByDepartment(Guid departmentId) => $"Positions:Department:{departmentId}";
    public static string EmployeesByBranch(Guid branchId) => $"Employees:Branch:{branchId}";
}