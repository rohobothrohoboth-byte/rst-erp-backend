using System.Net.Http.Json;
using System.Text.Json;

namespace Profile.App.Services;

public class CorMod(HttpClient http) : ICorMod
{
    public async Task<NameAmList?> Dept(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"GetDeptName/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<NameAmList>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<NameAmList>?> DeptList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("AllDeptName", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<NameAmList>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }
}
