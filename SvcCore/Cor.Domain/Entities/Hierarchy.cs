namespace Cor.Domain.Entities
{
    public class Hierarchy : BaseEntity
    {
        public Guid ParentId { get; set; }
        public Guid ChildId { get; set; }

        //******************************************//

        public Company Parent { get; set; } = default!;
        public Company Child { get; set; } = default!;
    }
}
