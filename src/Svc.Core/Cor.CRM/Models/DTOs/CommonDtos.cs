using System;

namespace Cor.CRM.Models.DTOs
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }

    public class DateRangeDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class IdNameDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class IdNameCodeDto : IdNameDto
    {
        public string Code { get; set; } = string.Empty;
    }

    public class SelectOptionDto
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? Group { get; set; }
        public bool IsDisabled { get; set; }
    }
}
