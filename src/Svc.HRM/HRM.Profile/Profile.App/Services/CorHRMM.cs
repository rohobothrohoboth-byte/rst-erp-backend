using System.Net.Http.Json;
using System.Text.Json;

namespace Profile.App.Services;

public class CorHRMM(HttpClient http) : ICorHRMM
{
    public async Task<AddressListDto?> Address(Guid id, CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync($"Address/{id}", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<AddressListDto>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }
        catch (NotSupportedException) { return null; }
        catch (JsonException) { return null; }
    }

    public async Task<List<AddressListDto>?> AddressList(CancellationToken ct = default)
    {
        try
        {
            using var res = await http.GetAsync("Address", ct);
            if (!res.IsSuccessStatusCode) { return null; }
            return await res.Content.ReadFromJsonAsync<List<AddressListDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException) { return null; }   // network errors
        catch (NotSupportedException) { return null; } // invalid content type
        catch (JsonException) { return null; }        // bad JSON
    }



}