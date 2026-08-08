using System.Text.Json.Serialization;

namespace Cor.Procurement.Models.DTOs;

public class VendorEvaluationDto
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string? VendorCode { get; set; }
    public int OverallScore { get; set; }
    public string? Category { get; set; }
    public DateTime EvaluationDate { get; set; }
    public string? Evaluator { get; set; }
    public string Status { get; set; } = "Good";
    public List<EvaluationCriteriaDto> Criteria { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class EvaluationCriteriaDto
{
    public string Name { get; set; } = string.Empty;
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public int Weight { get; set; }
}

public class CreateVendorEvaluationDto
{
    public Guid VendorId { get; set; }
    public string? Category { get; set; }
    public DateTime EvaluationDate { get; set; }
    public string? Evaluator { get; set; }
    public List<EvaluationCriteriaDto> Criteria { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public string? Notes { get; set; }
}

public class UpdateVendorEvaluationDto
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string? Category { get; set; }
    public DateTime EvaluationDate { get; set; }
    public string? Evaluator { get; set; }
    public List<EvaluationCriteriaDto> Criteria { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public string? Notes { get; set; }
    public string? RowVersion { get; set; }
}