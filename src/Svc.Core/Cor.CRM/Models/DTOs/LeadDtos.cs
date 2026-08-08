using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs
{
    public class LeadDto : BaseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Budget { get; set; }
        public decimal? EstimatedValue { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public bool IsConverted { get; set; }
        public DateTime? ConvertedDate { get; set; }
        public Guid? ConvertedCustomerId { get; set; }
        public int Score { get; set; }
        public int EngagementScore { get; set; }
        public string? Tags { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastContactDate { get; set; }
        public int ContactCount { get; set; }

        // Industry Specific
        public string? PropertyType { get; set; }
        public decimal? PropertyPrice { get; set; }
        public string? PropertyLocation { get; set; }
        public int? PropertySize { get; set; }
        public string? ProductCategory { get; set; }
        public int? OrderQuantity { get; set; }
        public DateTime? RequiredDeliveryDate { get; set; }
        public string? TenderNumber { get; set; }
        public DateTime? TenderDeadline { get; set; }
        public string? Department { get; set; }
    }

    public class CreateLeadDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Status { get; set; }
        public string? Source { get; set; }
        public string? Priority { get; set; }
        public string? Industry { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Budget { get; set; }
        public decimal? EstimatedValue { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? Tags { get; set; }
        public string? PropertyType { get; set; }
        public decimal? PropertyPrice { get; set; }
        public string? PropertyLocation { get; set; }
        public int? PropertySize { get; set; }
        public string? ProductCategory { get; set; }
        public int? OrderQuantity { get; set; }
        public DateTime? RequiredDeliveryDate { get; set; }
        public string? TenderNumber { get; set; }
        public DateTime? TenderDeadline { get; set; }
        public string? Department { get; set; }
    }

  public class UpdateLeadDto
  {
      public Guid Id { get; set; }  // ? MUST HAVE THIS!
      public string? FirstName { get; set; }
      public string? LastName { get; set; }
      public string? CompanyName { get; set; }
      public string? Email { get; set; }
      public string? Phone { get; set; }
      public string? Mobile { get; set; }
      public string? Address { get; set; }
      public string? City { get; set; }
      public string? State { get; set; }
      public string? Country { get; set; }
      public string? Status { get; set; }
      public string? Source { get; set; }
      public string? Priority { get; set; }
      public string? Industry { get; set; }
      public string? Title { get; set; }
      public string? Description { get; set; }
      public decimal? Budget { get; set; }
      public decimal? EstimatedValue { get; set; }
      public DateTime? ExpectedCloseDate { get; set; }
      public Guid? AssignedToUserId { get; set; }
      public string? Tags { get; set; }
      public string? PropertyType { get; set; }
      public decimal? PropertyPrice { get; set; }
      public string? PropertyLocation { get; set; }
      public int? PropertySize { get; set; }
      public string? ProductCategory { get; set; }
      public int? OrderQuantity { get; set; }
      public DateTime? RequiredDeliveryDate { get; set; }
      public string? TenderNumber { get; set; }
      public DateTime? TenderDeadline { get; set; }
      public string? Department { get; set; }
  }

    public class LeadFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? Source { get; set; }
        public string? Priority { get; set; }
        public string? Industry { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public bool? IsConverted { get; set; }
        public string? Tags { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }

    public class LeadStatsDto
    {
        public int TotalLeads { get; set; }
        public int NewLeads { get; set; }
        public int ContactedLeads { get; set; }
        public int QualifiedLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public int LostLeads { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal AverageLeadScore { get; set; }
        public Dictionary<string, int> LeadsBySource { get; set; } = new();
        public Dictionary<string, int> LeadsByIndustry { get; set; } = new();
        public Dictionary<string, int> LeadsByPriority { get; set; } = new();
    }

    public class LeadBulkActionDto
    {
        public List<Guid> LeadIds { get; set; } = new();
        public string Action { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
