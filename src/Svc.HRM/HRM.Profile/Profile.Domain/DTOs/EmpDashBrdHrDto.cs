using System;
using System.Collections.Generic;

namespace Profile.Domain.DTOs;

// ============================================================
// EMPLOYEE DATABASE REPORT DTOs
// ============================================================

public class EmpDbReport
{
    public int EmpTot { get; set; }
    public int EmpAct { get; set; }
    public int EmpPen { get; set; }
    public int EmpSus { get; set; }
    public int EmpRet { get; set; }
    public int EmpStd { get; set; }
    public int EmpTer { get; set; }
    public int EmpLeave { get; set; }
    public int EmpRej { get; set; }
}

public class EmpStateList
{
    public string? EmpState { get; set; }
}

// ============================================================
// PENDING EMPLOYEE LIST DTOs
// ============================================================

public class EmpDbPendList
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? EmpFullName { get; set; }
    public string? EmpFullNameAm { get; set; }
    public string? Gender { get; set; }
    public string? Branch { get; set; }
    public string? Department { get; set; }
    public string? DepartmentAm { get; set; }  // ✅ Added
    public string? Position { get; set; }
    public string? JobGrade { get; set; }
}

public class EmpDbPendEduExpList
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? EmpFullName { get; set; }
    public string? EmpFullNameAm { get; set; }
    public string? Gender { get; set; }
    public string? Branch { get; set; }
    public string? Department { get; set; }
    public string? DepartmentAm { get; set; }  // ✅ Added
    public string? Position { get; set; }
    public string? JobGrade { get; set; }
}

// ============================================================
// JOINT DTO FOR EMPLOYEE + PERSON DATA
// ============================================================

public class EmpDbPendJoin
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameAm { get; set; }
    public string? MiddleNameAm { get; set; }
    public string? LastNameAm { get; set; }
    public string? Gender { get; set; }
}

// ============================================================
// EMPLOYEE DATA DTOs
// ============================================================

public class EmployeeDataDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? EmpState { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public DateTime DateAdd { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameAm { get; set; }
    public string? MiddleNameAm { get; set; }
    public string? LastNameAm { get; set; }
    public string? Gender { get; set; }
}

public class EmployeeBaseDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? FullName { get; set; }
    public string? FullNameAm { get; set; }
    public string? Gender { get; set; }
    public string? Department { get; set; }
    public string? DepartmentAm { get; set; }
    public string? Position { get; set; }
    public string? JobGrade { get; set; }
    public string? Status { get; set; }
}

// ============================================================
// COMPLETE HR DASHBOARD DTO
// ============================================================

public class HrDashboardDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int PendingEmployeesCount { get; set; }
    public int SuspendedEmployees { get; set; }
    public int RetiredEmployees { get; set; }
    public int StandByEmployees { get; set; }
    public int TerminatedEmployees { get; set; }
    public int LeaveEmployees { get; set; }
    public int RejectedEmployees { get; set; }

    public List<EmpDbPendList> PendingEmployeesList { get; set; } = new();
    public List<EmpDbPendEduExpList> PendingEducationExperienceList { get; set; } = new();

    public int TotalDepartments { get; set; }
    public int TotalPositions { get; set; }
    public int TotalJobGrades { get; set; }
    public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
    public Dictionary<string, int> EmployeesByPosition { get; set; } = new();
    public Dictionary<string, int> EmployeesByStatus { get; set; } = new();

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int CacheDurationSeconds { get; set; } = 600;
}