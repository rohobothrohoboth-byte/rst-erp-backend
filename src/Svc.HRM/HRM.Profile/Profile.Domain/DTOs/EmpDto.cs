using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Profile.Domain.DTOs;

public class EmpPolicyCtx
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = default!;
    public double SerYear { get; set; } = default!;
    public string EmpType { get; set; } = default!;
    public string WorkAr { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Jg { get; set; } = default!;
}

public class EmpPosResDto
{
    public Guid Id { get; set; }
    public Guid PositionId { get; set; }
    public string SaturdayWorkOption { get; set; } = default!;
    public string SundayWorkOption { get; set; } = default!;
}

public class HrmEmpId
{
    public Guid Id { get; set; }
    public Guid PositionId { get; set; }
    public Guid DeptId { get; set; }
    public Guid BranchId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid JgStepId { get; set; }
    public Guid JgId { get; set; }
}

public class EmpPhotoRes
{
    [JsonIgnore]
    public byte[]? PhotoBinary { get; init; }
    [JsonIgnore]
    public long FileSize { get; set; } = default!;

    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string PhotoSize { get; set; } = default!;
    public string Photo { get; set; } = default!;
}

public class ProfileInfoJoin
{
    public string FirstName { get; init; } = default!;
    public string MiddleName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string FirstNameAm { get; init; } = default!;
    public string MiddleNameAm { get; init; } = default!;
    public string LastNameAm { get; init; } = default!;
    public string Code { get; set; } = default!;
    public Guid DepartmentId { get; init; }
    public Guid PositionId { get; init; }
}

public class ProfileInfo
{
    public string Code { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string FullNameAm { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
}

public class ProfileCardJoin
{
    public Guid Id { get; init; }
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
}

public class ProfileCard
{
    public string Tenure { get; set; } = default!;
    public string Performance { get; set; } = default!;
    public string Training { get; set; } = default!;
    public string Attendance { get; set; } = default!;
    public string RepToName { get; set; } = default!;
    public string RepToPos { get; set; } = default!;
}
