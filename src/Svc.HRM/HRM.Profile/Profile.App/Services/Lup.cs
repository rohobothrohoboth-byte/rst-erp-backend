using System.Net.Http.Json;
using System.Text.Json;

namespace Profile.App.Services;

public class Lup(HttpClient http) : ILup
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
    
    public async Task<LupListDto?> Relation(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Relation/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> RelationList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Relation", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }

    public async Task<LupListDto?> MaritalStatus(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"MaritalStatus/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> MaritalStatusList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("MaritalStatus", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }

    public async Task<LupListDto?> EmploymentType(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"EmploymentType/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> EmploymentTypeList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("EmploymentType", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }

    public async Task<LupListDto?> EmploymentNature(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"EmploymentNature/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<LupListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<LupListDto>?> EmploymentNatureList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("EmploymentNature", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<LupListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }




}
