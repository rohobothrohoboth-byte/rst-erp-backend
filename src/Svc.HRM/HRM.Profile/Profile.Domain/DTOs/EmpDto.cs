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
