using Profile.Domain.DTOs;

namespace Profile.App.Services;

public class AddressListDto : BaseDto
{
    public Guid RegionId { get; set; }
    public Guid AddressTypeId { get; set; }
    public string Region { get; set; } = default!;
    public string AddressType { get; set; } = default!;
    public string Country { get; set; } = default!;
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