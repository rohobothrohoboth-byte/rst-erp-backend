namespace ERP.Shared.API.Core.DTOs
{
    public class FiscYearListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string IsActive { get; set; } = default!;
        public string StartDate { get; set; } = default!;
        public string EndDate { get; set; } = default!;
        public string StartDateAm { get; set; } = default!;
        public string EndDateAm { get; set; } = default!;
        public string CreatedAt { get; set; } = default!;
        public string CreatedAtAm { get; set; } = default!;
        public string ModifiedAt { get; set; } = default!;
        public string ModifiedAtAm { get; set; } = default!;
        public string RowVersion { get; set; } = default!;
    }
    
    public class AddFiscYearDto
    {
        public required string Name { get; set; }
        public DateTime DateStart { get; set; } = DateTime.UtcNow;
        public DateTime DateEnd { get; set; } = DateTime.UtcNow.AddDays(365);
        public required bool IsActive { get; set; }
    }

    public class EditFiscYearDto
    {
        public required string Name { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public required bool IsActive { get; set; }
        public string RowVersion { get; set; } = default!;
    }
}
