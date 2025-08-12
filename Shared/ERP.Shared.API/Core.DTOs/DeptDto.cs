namespace ERP.Shared.API.Core.DTOs
{
    public class DeptListDto
    {
        public Guid Id { get; set; }
        public Guid BranchId { get; set; }
        public string Name { get; set; } = default!;
        public string NameAm { get; set; } = default!;
        public string Branch { get; set; } = default!;
        public string CreatedAt { get; set; } = default!;
        public string CreatedAtAm { get; set; } = default!;
        public string ModifiedAt { get; set; } = default!;
        public string ModifiedAtAm { get; set; } = default!;
        public string RowVersion { get; set; } = default!;
    }

    public class AddDeptDto
    {
        public required string Name { get; set; }
        public required string NameAm { get; set; }
        public Guid BranchId { get; set; }
    }
    
    public class EdtDeptDto
    {
        public required string Name { get; set; }
        public required string NameAm { get; set; }
        public Guid BranchId { get; set; }
        public string RowVersion { get; set; } = default!;
    }
}
