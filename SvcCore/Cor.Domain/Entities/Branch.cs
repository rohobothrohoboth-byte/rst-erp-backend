namespace Cor.Domain.Entities
{
    public class Branch : BaseEntity
    {
        public required string Name { get; set; }
        public required string NameAm { get; set; }
        public Guid CompId { get; set; }

        //******************************************//

        public Company Comp { get; set; } = default!;
    }
}
