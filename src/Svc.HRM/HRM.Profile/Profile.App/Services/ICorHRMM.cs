namespace Profile.App.Services;

public interface ICorHRMM
{
    Task<AddressListDto?> Address(Guid id, CancellationToken ct = default);
    Task<List<AddressListDto>?> AddressList(CancellationToken ct = default);

}
