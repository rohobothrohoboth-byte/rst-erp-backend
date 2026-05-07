namespace Profile.Domain.Entities;

public class Address : BaseEntity
{
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string? Country { get; set; }
    public string Region { get; set; } = default!;
    public string? Subcity { get; set; }
    public string? Zone { get; set; }
    public string? Woreda { get; set; }
    public string? Kebele { get; set; }
    public string? HouseNo { get; set; }
    public string Telephone { get; set; } = default!;
    public string? PoBox { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}