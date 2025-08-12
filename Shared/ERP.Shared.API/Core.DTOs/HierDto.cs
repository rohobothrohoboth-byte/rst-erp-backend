namespace ERP.Shared.API.Core.DTOs
{
    public class HierListDto
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid ChildId { get; set; }
        public string Parent { get; set; } = default!;
        public string Child { get; set; } = default!;
        public string CreatedAt { get; set; } = default!;
        public string CreatedAtAm { get; set; } = default!;
        public string ModifiedAt { get; set; } = default!;
        public string ModifiedAtAm { get; set; } = default!;
        public string RowVersion { get; set; } = default!;
    }
    
    public class AddHierDto
    {
        public Guid ParentId { get; set; }
        public Guid ChildId { get; set; }
    }

    public class EditHierDto
    {
        public Guid ParentId { get; set; }
        public Guid ChildId { get; set; }
        public string RowVersion { get; set; } = default!;
    }

}
