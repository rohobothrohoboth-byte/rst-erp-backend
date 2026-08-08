// Models/DTOs/ComplianceDto.cs
namespace Cor.Finance.Models.DTOs;

public class InternalControlDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Frequency { get; set; }
    public string? Owner { get; set; }
    public string? Department { get; set; }
    public string? Status { get; set; }
    public string? Effectiveness { get; set; }
    public DateTime? LastTestedDate { get; set; }
    public DateTime? NextTestDate { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddInternalControlDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Frequency { get; set; }
    public string? Owner { get; set; }
    public string? Department { get; set; }
    public string? Status { get; set; }
    public string? Effectiveness { get; set; }
    public DateTime? LastTestedDate { get; set; }
    public DateTime? NextTestDate { get; set; }
}

public class EditInternalControlDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Frequency { get; set; }
    public string? Owner { get; set; }
    public string? Department { get; set; }
    public string? Status { get; set; }
    public string? Effectiveness { get; set; }
    public DateTime? LastTestedDate { get; set; }
    public DateTime? NextTestDate { get; set; }
}


public class AddComplianceRequirementDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Regulation { get; set; }
    public string? Section { get; set; }
    public string? ComplianceStatus { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Owner { get; set; }
    public string? RiskLevel { get; set; }
    public string? Notes { get; set; }
}

public class EditComplianceRequirementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Regulation { get; set; }
    public string? Section { get; set; }
    public string? ComplianceStatus { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Owner { get; set; }
    public string? RiskLevel { get; set; }
    public string? Notes { get; set; }
}
public class ComplianceReportDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public string? Format { get; set; }
    public DateTime? GeneratedDate { get; set; }
    public string? GeneratedBy { get; set; }
    public string? FilePath { get; set; }
    public string? FileSize { get; set; }
    public string? Summary { get; set; }
    public int Findings { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public int PartiallyPassed { get; set; }
    public decimal ComplianceScore { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class GenerateComplianceReportDto
{
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Format { get; set; }
    public bool IncludeDetails { get; set; }
    public bool IncludeRecommendations { get; set; }
}








public class ComplianceRequirementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Regulation { get; set; }
    public string? Section { get; set; }
    public string? ComplianceStatus { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Owner { get; set; }
    public string? RiskLevel { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public List<ComplianceRequirementControlDto> Controls { get; set; } = new(); // ✅ ADD THIS
    public List<ComplianceRequirementEvidenceDto> Evidence { get; set; } = new(); // ✅ ADD THIS
}

// ✅ ADD THESE - Missing DTOs
public class ComplianceRequirementControlDto
{
    public Guid Id { get; set; }
    public Guid ComplianceRequirementId { get; set; }
    public string ControlName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ComplianceRequirementEvidenceDto
{
    public Guid Id { get; set; }
    public Guid ComplianceRequirementId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public DateTime UploadedAt { get; set; }
}