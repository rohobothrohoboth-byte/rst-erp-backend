using System.Net.Http.Json;

namespace Module.App.Services;

public class LupClient(HttpClient http) : ILupClient
{
    public async Task<LupListDto?> GetQuarter(Guid id)
    {
        var res = await http.GetAsync($"Quarter/{id}");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<LupListDto>();
    }

}
