using Leave.Domain.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace Leave.App.Services;

public class HrmProfile(HttpClient http) : IHrmProfile
{
    public async Task<NameList?> Emp(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Names/GetEmpName/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<NameList>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<NameList>?> EmpList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Names/AllEmpName", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<NameList>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }


}
