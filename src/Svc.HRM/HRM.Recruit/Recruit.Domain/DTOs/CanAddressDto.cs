namespace Recruit.Domain.DTOs;

public class CanAddressListDto : BaseDto
{
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
}

public class CanAddressAddDto
{
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
}

public class CanAddressModDto
{
    public Guid Id { get; set; }
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}