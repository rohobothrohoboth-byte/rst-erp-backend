using System.Text.Json;

namespace Cor.HRMM.Services;

public class CoreModuleClient(HttpClient http) : ICoreModuleClient
{
    public async Task<LupListDto?> Department(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Names/GetDeptName/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }

    public async Task<List<LupListDto>?> DepartmentList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Names/AllDeptName", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    

}