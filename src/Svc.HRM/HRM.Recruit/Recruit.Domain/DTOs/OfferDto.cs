namespace Recruit.Domain.DTOs;

public class OfferListDto
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public Guid JobPostingId { get; set; }
    public string JobPostingNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime OfferDate { get; set; }
    public string OfferDateStr { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string Currency { get; set; } = "ETB";
    public string Benefits { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string StartDateStr { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string ExpiryDateStr { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}

public class OfferAddDto
{
    public Guid ApplicantId { get; set; }
    public Guid JobPostingId { get; set; }
    public decimal Salary { get; set; }
    public string Currency { get; set; } = "ETB";
    public string? Benefits { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Notes { get; set; }
}

public class OfferModDto
{
    public Guid Id { get; set; }
    public decimal Salary { get; set; }
    public string? Benefits { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Notes { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}

public class OfferResponseDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!; // Accepted / Rejected
    public string? Comment { get; set; }
}
