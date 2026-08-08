using System.Text.Json;
using Helpers;
using Microsoft.Extensions.Logging;

namespace Recruit.App.Services;

public class HireEmployeeRequest
{
    public string FirstName { get; set; } = default!;
    public string FirstNameAm { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string MiddleNameAm { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LastNameAm { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public DateTime EmploymentDate { get; set; }
    public Guid JobGradeId { get; set; }
    public Guid JgStepId { get; set; }
    public Guid PositionId { get; set; }
    public Guid DepartmentId { get; set; }
    public string EmploymentType { get; set; } = default!;
    public string EmploymentNature { get; set; } = default!;
    public string WorkArrangement { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public string MaritalStatus { get; set; } = default!;
    public string AddressType { get; set; } = default!;
    public string? Country { get; set; }
    public string Region { get; set; } = default!;
    public string? Subcity { get; set; }
    public string? Zone { get; set; }
    public string? Woreda { get; set; }
    public string? Kebele { get; set; }
    public string? HouseNo { get; set; }
    public string Telephone { get; set; } = default!;
    public string? PoBox { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
}

public class HireEmployeeResult
{
    public Guid EmployeeId { get; set; }
}

public interface IHrmProfileHireClient
{
    Task<HireEmployeeResult> CreateEmployeeAsync(HireEmployeeRequest request, CancellationToken ct = default);
}

public class HrmProfileHireClient : IHrmProfileHireClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HrmProfileHireClient> _logger;

    public HrmProfileHireClient(HttpClient httpClient, ILogger<HrmProfileHireClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HireEmployeeResult> CreateEmployeeAsync(HireEmployeeRequest request, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();

        void Add(string name, string? value)
        {
            if (value != null)
                form.Add(new StringContent(value), name);
        }

        Add("FirstName", request.FirstName);
        Add("FirstNameAm", request.FirstNameAm);
        Add("MiddleName", request.MiddleName);
        Add("MiddleNameAm", request.MiddleNameAm);
        Add("LastName", request.LastName);
        Add("LastNameAm", request.LastNameAm);
        Add("Gender", request.Gender);
        Add("Nationality", request.Nationality);
        Add("EmploymentDate", request.EmploymentDate.ToString("o"));
        Add("JobGradeId", request.JobGradeId.ToString());
        Add("JgStepId", request.JgStepId.ToString());
        Add("PositionId", request.PositionId.ToString());
        Add("DepartmentId", request.DepartmentId.ToString());
        Add("EmploymentType", request.EmploymentType);
        Add("EmploymentNature", request.EmploymentNature);
        Add("WorkArrangement", request.WorkArrangement);
        Add("BirthDate", request.BirthDate.ToString("o"));
        Add("MaritalStatus", request.MaritalStatus);
        Add("AddressType", request.AddressType);
        Add("Country", request.Country);
        Add("Region", request.Region);
        Add("Subcity", request.Subcity);
        Add("Zone", request.Zone);
        Add("Woreda", request.Woreda);
        Add("Kebele", request.Kebele);
        Add("HouseNo", request.HouseNo);
        Add("Telephone", request.Telephone);
        Add("PoBox", request.PoBox);
        Add("Fax", request.Fax);
        Add("Email", request.Email);

        var response = await _httpClient.PostAsync("/api/hrm/profile/v1/AddEmp/Step1", form, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Profile hire failed ({Status}): {Body}", response.StatusCode, body);
            throw new DomainException($"Failed to create employee in Profile: {response.StatusCode}. {body}");
        }

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        Guid employeeId = default;
        if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
        {
            if (data.TryGetProperty("id", out var idEl) || data.TryGetProperty("Id", out idEl))
                employeeId = idEl.GetGuid();
        }
        else if (root.TryGetProperty("id", out var topId) || root.TryGetProperty("Id", out topId))
        {
            employeeId = topId.GetGuid();
        }

        if (employeeId == Guid.Empty)
            throw new DomainException("Profile hire succeeded but employee Id was not returned.");

        return new HireEmployeeResult { EmployeeId = employeeId };
    }
}
