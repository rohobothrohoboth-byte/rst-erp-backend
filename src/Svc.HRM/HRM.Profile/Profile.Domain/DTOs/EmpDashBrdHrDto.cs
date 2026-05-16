namespace Profile.Domain.DTOs;

public class EmpDbReport
{
    public int EmpTot { get; set; } = 0;
    public int EmpAct { get; set; } = 0;
    public int EmpPen { get; set; } = 0;
    public int EmpSus { get; set; } = 0;
    public int EmpRet { get; set; } = 0;
    public int EmpStd { get; set; } = 0;
    public int EmpTer { get; set; } = 0;
    public int EmpLeave { get; set; } = 0;
}

public class EmpDbPendList
{
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
}