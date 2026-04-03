using System;
using System.Collections.Generic;
using System.Text;

namespace Recruit.Domain.DTOs;

public class VacancyListDto : BaseDto
{
    public int NumOpen { get; set; } = 0;
    public string PostNumber { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string WorkArr { get; set; } = default!;
    public string DatePosted { get; set; } = default!;
    public string Deadline { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
}

public class VacancyDetailDto : BaseDto
{
    public int NumOpen { get; set; } = 0;
    public string Position { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string WorkArr { get; set; } = default!;
    public string DatePosted { get; set; } = default!;
    public string Deadline { get; set; } = default!;
    public string JobGrade { get; set; } = default!;

    public int NumApp { get; set; } = 0;
    public string JgStep { get; set; } = default!;
    public string JobDesc { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGenderStr { get; set; } = default!; // enum.Gender
    public string ContractTypeStr { get; set; } = default!; // enum.EmpNature
}


