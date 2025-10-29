namespace Profile.Domain.Entities;

public class Address : BaseEntity
{
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
    public string Telephone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Website { get; set; } = default!;
}