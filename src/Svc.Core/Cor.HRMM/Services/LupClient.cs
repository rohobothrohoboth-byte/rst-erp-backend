namespace Cor.HRMM.Services;

public class LupClient(HttpClient http) : ILupClient
{
    public async Task<LupListDto?> GetAddressType(Guid id)
    {
        var res = await http.GetAsync($"AddressType/{id}");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<LupListDto>();
    }

    public async Task<LupListDto?> GetRegion(Guid id)
    {
        var res = await http.GetAsync($"Region/{id}");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<LupListDto>();
    }
}
