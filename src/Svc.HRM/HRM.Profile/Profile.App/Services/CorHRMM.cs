using System.Net.Http.Json;
using System.Text.Json;

namespace Profile.App.Services;

public class CorHRMM(HttpClient http) : ICorHRMM
{
    public async Task<NameList?> JobGrade(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Names/GetJobGradeName/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<NameList>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<NameList>?> JobGradeList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Names/AllJobGradeName", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<NameList>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }

    public async Task<NameAmList?> Position(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Names/GetPositionName/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<NameAmList>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<NameAmList>?> PositionList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Names/AllPositionName", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<NameAmList>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }


}