using System.Text.Json;

namespace Cor.HRMM.Services;

public class LupClient(HttpClient http) : ILupClient
{
    public async Task<LupListDto?> AddressType(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"AddressType/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }

    public async Task<List<LupListDto>?> AddressTypeList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("AddressType", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<LupListDto?> Region(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Region/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> RegionList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Region", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

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

    public async Task<LupListDto?> ProfessionType(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"ProfessionType/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> ProfessionTypeList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("ProfessionType", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }



}
