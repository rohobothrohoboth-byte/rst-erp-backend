using System.Text.Json;

namespace Cor.HRMM.Services;

public class LupClient(HttpClient http) : ILupClient
{
    public async Task<LupListDto?> EducationLevel(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"EducationLevel/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> EducationLevelList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("EducationLevel", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }



}
