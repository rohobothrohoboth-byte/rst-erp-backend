namespace Cor.Domain.Entities
{
    public class Department : BaseEntity
    {
        public required string Name { get; set; }
        public required string NameAm { get; set; }
        public Guid BranchId { get; set; }

        //******************************************//

        public Branch Branch { get; set; } = default!;
    }
}
