using Svc.Shared.DTOs;

namespace Svc.Shared.Services;

public class RegionClient(HttpClient http) : IRegionClient
{
    public async Task<LupListDto?> GetRegion(Guid id)
    {
        var res = await http.GetAsync($"api/lup/Region/{id}");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<LupListDto>();
    }
}
