// Models/DTOs/EliminationEntryDto.cs
namespace Cor.Finance.Models.DTOs;

public class EliminationEntryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }

    // From Entity
    public Guid? FromEntityId { get; set; }
    public string? FromEntityName { get; set; }
    public string? FromEntityCode { get; set; }

    // To Entity
    public Guid? ToEntityId { get; set; }
    public string? ToEntityName { get; set; }
    public string? ToEntityCode { get; set; }

    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal AmountInReportingCurrency { get; set; }

    public string? AccountCode { get; set; }
    public string? AccountName { get; set; }

    public string? Status { get; set; }
    public Guid? ConsolidationGroupId { get; set; }
    public string? ConsolidationGroupName { get; set; }
    public string? Period { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedBy { get; set; }

    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class AddEliminationEntryDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public Guid? FromEntityId { get; set; }
    public Guid? ToEntityId { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public decimal ExchangeRate { get; set; } = 1;
    public decimal AmountInReportingCurrency { get; set; }
    public string? AccountCode { get; set; }
    public string? AccountName { get; set; }
    public string? Status { get; set; }
    public Guid? ConsolidationGroupId { get; set; }
    public string? Period { get; set; }
}

public class EditEliminationEntryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public Guid? FromEntityId { get; set; }
    public Guid? ToEntityId { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal AmountInReportingCurrency { get; set; }
    public string? AccountCode { get; set; }
    public string? AccountName { get; set; }
    public string? Status { get; set; }
    public Guid? ConsolidationGroupId { get; set; }
    public string? Period { get; set; }
    public string? RowVersion { get; set; }
}

public class PostEliminationEntryDto
{
    public Guid Id { get; set; }
    public string? PostedBy { get; set; }
}

public class RejectEliminationEntryDto
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
}