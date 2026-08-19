// Models/DTOs/AuditLogSummaryDto.cs
namespace Cor.ProjectManagement.Models.DTOs
{
    public class AuditLogSummaryDto
    {
        public Guid ProjectId { get; set; }
        public int TotalLogs { get; set; }
        public Dictionary<string, int> LogsByAction { get; set; } = new();
        public Dictionary<string, int> LogsByUser { get; set; } = new();
        public Dictionary<string, int> LogsByEntity { get; set; } = new();
        public DateTime? LastActivity { get; set; }
        public string? MostActiveUser { get; set; }
    }
}