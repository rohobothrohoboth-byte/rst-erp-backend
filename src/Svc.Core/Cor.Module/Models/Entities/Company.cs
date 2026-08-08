namespace Cor.Module.Models.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;

        public string? TaxId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? LogoUrl { get; set; }

       public ICollection<Branch> Branches { get; set; } = new List<Branch>();
}