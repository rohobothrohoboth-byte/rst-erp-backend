namespace ERP.Shared.API.Core.DTOs
{
    public class CompListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string NameAm { get; set; } = default!;
        public string CreatedAt { get; set; } = default!;
        public string CreatedAtAm { get; set; } = default!;
        public string ModifiedAt { get; set; } = default!;
        public string ModifiedAtAm { get; set; } = default!;
        public string RowVersion { get; set; } = default!;
    }

    public class AddCompDto
    {
        public required string Name { get; set; }
        public required string NameAm { get; set; }
    }

    public class EditCompDto
    {
        public required string Name { get; set; }
        public required string NameAm { get; set; }
        public string RowVersion { get; set; } = default!;
    }
}
