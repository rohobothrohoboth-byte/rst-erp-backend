// Models/DTOs/ConsolidationDto.cs
namespace Cor.Finance.Models.DTOs;






public class AddConsolidationGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentEntityId { get; set; }
    public List<Guid> EntityIds { get; set; } = new();
    public Guid? PeriodId { get; set; }
    public DateTime? ConsolidationDate { get; set; }
}

public class EditConsolidationGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentEntityId { get; set; }
    public List<Guid> EntityIds { get; set; } = new();
    public Guid? PeriodId { get; set; }
    public DateTime? ConsolidationDate { get; set; }
    public string? Status { get; set; }
}



public class ConsolidationGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentEntityId { get; set; }
    public string? ParentEntityName { get; set; }
    public string? Status { get; set; }
    public DateTime? ConsolidationDate { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
   public List<EntityDto> Entities { get; set; } = new(); // ✅ This will now include all EntityDto properties
    public List<EliminationEntryDto> EliminationEntries { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}


public class ConsolidationResultDto
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public DateTime ConsolidationDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public int EntitiesCount { get; set; }
    public int EliminationEntriesCount { get; set; }
    public string? Status { get; set; }
    public List<EntityConsolidationResultDto> EntityResults { get; set; } = new();
}

public class EntityConsolidationResultDto
{
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
    public decimal Assets { get; set; }
    public decimal Liabilities { get; set; }
    public decimal Equity { get; set; }
    public decimal OwnershipPercentage { get; set; }
}




