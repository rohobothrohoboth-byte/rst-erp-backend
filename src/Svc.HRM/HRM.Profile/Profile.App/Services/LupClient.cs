using System.Net.Http.Json;
using System.Text.Json;

namespace Profile.App.Services;

public class LupClient(HttpClient http) : ILupClient
{
    public async Task<LupListDto?> Quarter(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Quarter/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> QuarterList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Quarter", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }
}
