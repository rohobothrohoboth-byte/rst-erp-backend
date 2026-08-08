// Profile.App/Services/UserScopeService.cs
using Common;
using Contracts;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Profile.App.Interfaces;

namespace Profile.App.Services;

public interface IUserScopeService
{
    Task<UserScopeDto> GetUserScopeAsync(string employeeId);
    Task<Guid?> GetUserBranchIdAsync(string employeeId);
    Task<Guid?> GetUserDepartmentIdAsync(string employeeId);
    Task<Guid?> GetUserCompanyIdAsync(string employeeId);
}

public class UserScopeService : IUserScopeService
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    private readonly ICorHrmmClient _corHrmm;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserScopeService> _logger;

    public UserScopeService(
        IDapperHelper dapper,
        ICorModClient corMod,
        ICorHrmmClient corHrmm,
        IConfiguration configuration,
        ILogger<UserScopeService> logger)
    {
        _dapper = dapper;
        _corMod = corMod;
        _corHrmm = corHrmm;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UserScopeDto> GetUserScopeAsync(string employeeId)
    {
        try
        {
            _logger.LogInformation("========== START ==========");
            _logger.LogInformation("Getting user scope for employeeId: {EmployeeId}", employeeId ?? "null");

            string? actualEmployeeId = employeeId;

            var directCheck = await CheckEmployeeExistsAsync(employeeId);

            if (!directCheck)
            {
                _logger.LogInformation("Employee not found directly, checking AppUser table...");

                const string getEmployeeIdSql = @"
                    SELECT ""EmployeeId""
                    FROM public.""AppUser""
                    WHERE ""Id"" = @AppUserId::uuid
                      AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

                var hrmEmployeeId = await _dapper.QueryFirstOrDefaultAsync<string>(getEmployeeIdSql,
                    new { AppUserId = employeeId });

                if (!string.IsNullOrEmpty(hrmEmployeeId))
                {
                    _logger.LogInformation("Found EmployeeId from AppUser: {HrmEmployeeId}", hrmEmployeeId);
                    actualEmployeeId = hrmEmployeeId;
                }
                else
                {
                    _logger.LogWarning("No EmployeeId found in AppUser for: {EmployeeId}", employeeId);
                    return new UserScopeDto();
                }
            }

            _logger.LogInformation("Using EmployeeId: {ActualEmployeeId}", actualEmployeeId);

            var employee = await GetEmployeeWithPersonAsync(actualEmployeeId!);

            if (employee == null)
            {
                _logger.LogWarning("Employee not found for ID: {ActualEmployeeId}", actualEmployeeId);
                return new UserScopeDto();
            }

            _logger.LogInformation("✅ Employee found: {EmployeeId} - {FirstName} {LastName} (Code: {EmployeeCode})",
                employee.EmployeeId, employee.FirstName, employee.LastName, employee.EmployeeCode);

          DepartmentDto? department = null;
          BranchDto? branch = null;
          CompanyDto? company = null;
            if (employee.DepartmentId.HasValue && employee.DepartmentId.Value != Guid.Empty)
            {
                _logger.LogInformation("Fetching department: {DepartmentId}", employee.DepartmentId.Value);
                department = await GetDepartmentFromCoreDbAsync(employee.DepartmentId.Value);

                if (department != null)
                {
                    _logger.LogInformation("✅ Department found: {DepartmentName}", department.Name);

                    if (department.BranchId.HasValue && department.BranchId.Value != Guid.Empty)
                    {
                        _logger.LogInformation("Fetching branch: {BranchId}", department.BranchId.Value);
                        branch = await GetBranchFromCoreDbAsync(department.BranchId.Value);

                        if (branch != null)
                        {
                            _logger.LogInformation("✅ Branch found: {BranchName}", branch.Name);

                            if (branch.CompanyId.HasValue && branch.CompanyId.Value != Guid.Empty)
                            {
                                _logger.LogInformation("Fetching company: {CompanyId}", branch.CompanyId.Value);
                                company = await GetCompanyFromCoreDbAsync(branch.CompanyId.Value);

                                if (company != null)
                                {
                                    _logger.LogInformation("✅ Company found: {CompanyName}", company.Name);
                                }
                            }
                        }
                    }
                }
            }

            PositionDto? position = null;
            if (employee.PositionId.HasValue && employee.PositionId.Value != Guid.Empty)
            {
                _logger.LogInformation("Fetching position: {PositionId}", employee.PositionId.Value);
                position = await GetPositionFromGrpcAsync(employee.PositionId.Value);
                if (position != null)
                {
                    _logger.LogInformation("✅ Position found: {PositionName}", position.Name);
                }
            }

            JobGradeDto? jobGrade = null;
            if (employee.JobGradeId.HasValue && employee.JobGradeId.Value != Guid.Empty)
            {
                _logger.LogInformation("Fetching job grade: {JobGradeId}", employee.JobGradeId.Value);
                jobGrade = await GetJobGradeFromGrpcAsync(employee.JobGradeId.Value);
                if (jobGrade != null)
                {
                    _logger.LogInformation("✅ Job grade found: {JobGradeName}", jobGrade.Name);
                }
            }

            var result = new UserScopeDto
            {
                Employee = employee,
                Department = department,
                Branch = branch,
                Company = company,
                Position = position,
                JobGrade = jobGrade
            };

            _logger.LogInformation("========== COMPLETE ==========");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user scope for employee: {EmployeeId}", employeeId ?? "null");
            throw;
        }
    }

    public async Task<Guid?> GetUserBranchIdAsync(string employeeId)
    {
        var scope = await GetUserScopeAsync(employeeId);
        return scope?.Branch?.Id;
    }

    public async Task<Guid?> GetUserDepartmentIdAsync(string employeeId)
    {
        var scope = await GetUserScopeAsync(employeeId);
        return scope?.Department?.Id;
    }

    public async Task<Guid?> GetUserCompanyIdAsync(string employeeId)
    {
        var scope = await GetUserScopeAsync(employeeId);
        return scope?.Company?.Id;
    }

    private async Task<bool> CheckEmployeeExistsAsync(string employeeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM public.""Employee""
                WHERE ""Id"" = @EmployeeId::uuid
                  AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

            var count = await _dapper.QueryFirstOrDefaultAsync<int>(sql, new { EmployeeId = employeeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking employee existence: {EmployeeId}", employeeId);
            return false;
        }
    }
private async Task<EmployeeDto?> GetEmployeeWithPersonAsync(string employeeId)
{
    try
    {
        _logger.LogInformation("Querying Employee table for: {EmployeeId}", employeeId);

        const string employeeSql = @"
            SELECT
                e.""Id"" as EmployeeId,
                e.""Code"" as EmployeeCode,
                e.""EmploymentType"" as EmpType,
                e.""EmploymentNature"" as EmpNature,
                e.""WorkArrangement"" as WorkArrangement,
                e.""EmpState"" as EmpState,
                e.""EmploymentDate"" as EmploymentDate,
                e.""JobGradeId"" as JobGradeId,
                e.""PositionId"" as PositionId,
                e.""DepartmentId"" as DepartmentId,
                e.""PersonId"" as PersonId
            FROM public.""Employee"" e
            WHERE e.""Id"" = @EmployeeId::uuid
              AND (e.""IsDeleted"" IS NULL OR e.""IsDeleted"" = FALSE)";

        var employeeResult = await _dapper.QueryFirstOrDefaultAsync<dynamic>(employeeSql, new { EmployeeId = employeeId });

        if (employeeResult == null)
        {
            _logger.LogWarning("❌ Employee NOT found in Employee table: {EmployeeId}", employeeId);
            return null;
        }

        _logger.LogInformation("✅ Employee found in Employee table");

        var dict = employeeResult as IDictionary<string, object>;
        if (dict != null)
        {
            _logger.LogInformation("📋 Employee result - ALL PROPERTIES:");
            foreach (var kvp in dict)
            {
                var valueStr = kvp.Value?.ToString() ?? "NULL";
                var typeName = kvp.Value?.GetType().Name ?? "null";
                _logger.LogInformation("  🔹 {Key} = {Value} ({Type})", kvp.Key, valueStr, typeName);
            }
        }

        // Helper function to get value from dictionary - FIXED for Guid
        T? GetValueFromDict<T>(IDictionary<string, object>? dictionary, string key)
        {
            if (dictionary == null) return default;

            // Try case-insensitive lookup
            string? foundKey = null;
            foreach (var k in dictionary.Keys)
            {
                if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    foundKey = k;
                    break;
                }
            }

            if (foundKey == null || !dictionary.TryGetValue(foundKey, out var value))
                return default;

            if (value == null || value == DBNull.Value)
                return default;

            try
            {
                Type targetType = typeof(T);

                // Handle Nullable types
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = Nullable.GetUnderlyingType(targetType)!;
                }

                // Special handling for Guid
                if (targetType == typeof(Guid))
                {
                    if (value is Guid guidValue)
                        return (T)(object)guidValue;

                    if (value is string strValue && Guid.TryParse(strValue, out Guid parsedGuid))
                        return (T)(object)parsedGuid;

                    if (value is byte[] bytes && bytes.Length == 16)
                        return (T)(object)new Guid(bytes);

                    return default;
                }

                // Special handling for DateTime
                if (targetType == typeof(DateTime))
                {
                    if (value is DateTime dateTime)
                        return (T)(object)dateTime;

                    if (value is string strDate && DateTime.TryParse(strDate, out DateTime parsedDate))
                        return (T)(object)parsedDate;

                    return default;
                }

                // For other types, use Convert.ChangeType
                return (T)Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error converting value for key {Key} to type {Type}", key, typeof(T).Name);
                return default;
            }
        }

        // Extract EmployeeId and EmployeeCode using the helper
        var employeeIdGuid = GetValueFromDict<Guid>(dict, "EmployeeId");
        var employeeCode = GetValueFromDict<string>(dict, "EmployeeCode") ?? string.Empty;

        _logger.LogInformation("✅ Extracted EmployeeId: {EmployeeId}, EmployeeCode: {EmployeeCode}", employeeIdGuid, employeeCode);

        // Get PersonId with case-insensitive lookup - FIXED
        Guid? personId = null;
        try
        {
            if (dict != null)
            {
                string? personIdKey = null;
                foreach (var key in dict.Keys)
                {
                    if (key.Equals("PersonId", StringComparison.OrdinalIgnoreCase))
                    {
                        personIdKey = key;
                        break;
                    }
                }

                if (personIdKey != null && dict.TryGetValue(personIdKey, out var personIdObj))
                {
                    if (personIdObj != null && personIdObj != DBNull.Value)
                    {
                        if (personIdObj is Guid guidValue)
                        {
                            personId = guidValue;
                        }
                        else if (personIdObj is string strValue && Guid.TryParse(strValue, out Guid parsedGuid))
                        {
                            personId = parsedGuid;
                        }
                        else
                        {
                            var strVal = personIdObj.ToString();
                            if (!string.IsNullOrEmpty(strVal) && Guid.TryParse(strVal, out Guid parsed))
                            {
                                personId = parsed;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting PersonId");
        }

        _logger.LogInformation("PersonId: {PersonId}", personId?.ToString() ?? "null");

        // Get Person data using the SAME connection
        PersonDto? person = null;
        if (personId.HasValue && personId.Value != Guid.Empty)
        {
            try
            {
                _logger.LogInformation("Querying Person table for PersonId: {PersonId}", personId.Value);

                const string personSql = @"
                    SELECT
                        ""Id"" as PersonId,
                        ""FirstName"" as FirstName,
                        ""FirstNameAm"" as FirstNameAm,
                        ""MiddleName"" as MiddleName,
                        ""MiddleNameAm"" as MiddleNameAm,
                        ""LastName"" as LastName,
                        ""LastNameAm"" as LastNameAm,
                        ""Gender"" as Gender,
                        ""Nationality"" as Nationality
                    FROM public.""Person""
                    WHERE ""Id"" = @PersonId::uuid
                      AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

                var personResult = await _dapper.QueryFirstOrDefaultAsync<dynamic>(personSql,
                    new { PersonId = personId.Value });

                if (personResult != null)
                {
                    var personDict = personResult as IDictionary<string, object>;

                    // Extract values using the helper
                    var firstName = GetValueFromDict<string>(personDict, "FirstName") ?? string.Empty;
                    var lastName = GetValueFromDict<string>(personDict, "LastName") ?? string.Empty;
                    var firstNameAm = GetValueFromDict<string>(personDict, "FirstNameAm") ?? string.Empty;
                    var lastNameAm = GetValueFromDict<string>(personDict, "LastNameAm") ?? string.Empty;
                    var middleName = GetValueFromDict<string>(personDict, "MiddleName") ?? string.Empty;
                    var middleNameAm = GetValueFromDict<string>(personDict, "MiddleNameAm") ?? string.Empty;
                    var gender = GetValueFromDict<string>(personDict, "Gender") ?? string.Empty;
                    var nationality = GetValueFromDict<string>(personDict, "Nationality") ?? string.Empty;
                    var personIdGuid = GetValueFromDict<Guid>(personDict, "PersonId");

                    person = new PersonDto
                    {
                        PersonId = personIdGuid,
                        FirstName = firstName,
                        FirstNameAm = firstNameAm,
                        MiddleName = middleName,
                        MiddleNameAm = middleNameAm,
                        LastName = lastName,
                        LastNameAm = lastNameAm,
                        Gender = gender,
                        Nationality = nationality
                    };

                    _logger.LogInformation("✅ Person found: FirstName='{FirstName}', LastName='{LastName}', FirstNameAm='{FirstNameAm}', LastNameAm='{LastNameAm}'",
                        person.FirstName, person.LastName, person.FirstNameAm, person.LastNameAm);
                }
                else
                {
                    _logger.LogWarning("❌ Person NOT found for PersonId: {PersonId}", personId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error fetching Person for PersonId: {PersonId}", personId.Value);
            }
        }
        else
        {
            _logger.LogWarning("No valid PersonId in Employee record");
        }

        // Build the employee DTO with proper extraction
        return new EmployeeDto
        {
            EmployeeId = employeeIdGuid,
            EmployeeCode = employeeCode,
            PersonId = personId,
            FirstName = person?.FirstName ?? string.Empty,
            MiddleName = person?.MiddleName ?? string.Empty,
            LastName = person?.LastName ?? string.Empty,
            FirstNameAm = person?.FirstNameAm ?? string.Empty,
            MiddleNameAm = person?.MiddleNameAm ?? string.Empty,
            LastNameAm = person?.LastNameAm ?? string.Empty,
            Gender = person?.Gender ?? string.Empty,
            Nationality = person?.Nationality ?? string.Empty,
            DepartmentId = GetValueFromDict<Guid?>(dict, "DepartmentId"),
            PositionId = GetValueFromDict<Guid?>(dict, "PositionId"),
            JobGradeId = GetValueFromDict<Guid?>(dict, "JobGradeId"),
            EmpState = GetValueFromDict<string>(dict, "EmpState") ?? string.Empty,
            EmpType = GetValueFromDict<string>(dict, "EmpType") ?? string.Empty,
            EmpNature = GetValueFromDict<string>(dict, "EmpNature") ?? string.Empty,
            EmploymentDate = GetValueFromDict<DateTime?>(dict, "EmploymentDate"),
            WorkArrangement = GetValueFromDict<string>(dict, "WorkArrangement") ?? string.Empty
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting employee with person for ID: {EmployeeId}", employeeId);
        return null;
    }
}
private async Task<DepartmentDto?> GetDepartmentFromCoreDbAsync(Guid departmentId)
{
    try
    {
        var connectionString = _configuration.GetConnectionString("CorModuleDbCon");
        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                ""Id"" as Id,
                ""Name"" as Name,
                ""NameAm"" as NameAm,
                ""BranchId"" as BranchId
            FROM public.""Department""
            WHERE ""Id"" = @DepartmentId::uuid
              AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { DepartmentId = departmentId });

        if (result == null)
            return null;

        var dict = result as IDictionary<string, object>;

        // Log the department result for debugging
        if (dict != null)
        {
            _logger.LogInformation("📋 Department result - ALL PROPERTIES:");
            foreach (var kvp in dict)
            {
                var valueStr = kvp.Value?.ToString() ?? "NULL";
                var typeName = kvp.Value?.GetType().Name ?? "null";
                _logger.LogInformation("  🔹 {Key} = {Value} ({Type})", kvp.Key, valueStr, typeName);
            }
        }

        var department = new DepartmentDto
        {
            Id = GetValueFromDict<Guid>(dict, "Id"),
            Name = GetValueFromDict<string>(dict, "Name") ?? "Unknown",
            NameAm = GetValueFromDict<string>(dict, "NameAm") ?? "Unknown",
            BranchId = GetValueFromDict<Guid?>(dict, "BranchId")
        };

        _logger.LogInformation("✅ Department created: Id={Id}, Name={Name}, BranchId={BranchId}",
            department.Id, department.Name, department.BranchId?.ToString() ?? "null");

        return department;
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Failed to get department from Core Module DB: {DepartmentId}", departmentId);
        return null;
    }
}
private async Task<BranchDto?> GetBranchFromCoreDbAsync(Guid branchId)
{
    try
    {
        _logger.LogInformation("Fetching branch from Core DB: {BranchId}", branchId);

        var connectionString = _configuration.GetConnectionString("CorModuleDbCon");
        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                ""Id"" as Id,
                ""Name"" as Name,
                ""NameAm"" as NameAm,
                ""CompId"" as CompanyId
            FROM public.""Branch""
            WHERE ""Id"" = @BranchId::uuid
              AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { BranchId = branchId });

        if (result == null)
        {
            _logger.LogWarning("❌ Branch NOT found for BranchId: {BranchId}", branchId);
            return null;
        }

        var dict = result as IDictionary<string, object>;

        // Log the branch result for debugging
        if (dict != null)
        {
            _logger.LogInformation("📋 Branch result - ALL PROPERTIES:");
            foreach (var kvp in dict)
            {
                var valueStr = kvp.Value?.ToString() ?? "NULL";
                var typeName = kvp.Value?.GetType().Name ?? "null";
                _logger.LogInformation("  🔹 {Key} = {Value} ({Type})", kvp.Key, valueStr, typeName);
            }
        }

        var branch = new BranchDto
        {
            Id = GetValueFromDict<Guid>(dict, "Id"),
            Name = GetValueFromDict<string>(dict, "Name") ?? "Unknown",
            NameAm = GetValueFromDict<string>(dict, "NameAm") ?? "Unknown",
            CompanyId = GetValueFromDict<Guid?>(dict, "CompanyId")
        };

        _logger.LogInformation("✅ Branch created: Id={Id}, Name={Name}, CompanyId={CompanyId}",
            branch.Id, branch.Name, branch.CompanyId?.ToString() ?? "null");

        return branch;
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Failed to get branch from Core Module DB: {BranchId}", branchId);
        return null;
    }
}

private async Task<CompanyDto?> GetCompanyFromCoreDbAsync(Guid companyId)
{
    try
    {
        _logger.LogInformation("Fetching company from Core DB: {CompanyId}", companyId);

        var connectionString = _configuration.GetConnectionString("CorModuleDbCon");
        await using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                ""Id"" as Id,
                ""Name"" as Name,
                ""NameAm"" as NameAm
            FROM public.""Company""
            WHERE ""Id"" = @CompanyId::uuid
              AND (""IsDeleted"" IS NULL OR ""IsDeleted"" = FALSE)";

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { CompanyId = companyId });

        if (result == null)
        {
            _logger.LogWarning("❌ Company NOT found for CompanyId: {CompanyId}", companyId);
            return null;
        }

        var dict = result as IDictionary<string, object>;

        // Log the company result for debugging
        if (dict != null)
        {
            _logger.LogInformation("📋 Company result - ALL PROPERTIES:");
            foreach (var kvp in dict)
            {
                var valueStr = kvp.Value?.ToString() ?? "NULL";
                var typeName = kvp.Value?.GetType().Name ?? "null";
                _logger.LogInformation("  🔹 {Key} = {Value} ({Type})", kvp.Key, valueStr, typeName);
            }
        }

        var company = new CompanyDto
        {
            Id = GetValueFromDict<Guid>(dict, "Id"),
            Name = GetValueFromDict<string>(dict, "Name") ?? "Unknown",
            NameAm = GetValueFromDict<string>(dict, "NameAm") ?? "Unknown"
        };

        _logger.LogInformation("✅ Company created: Id={Id}, Name={Name}", company.Id, company.Name);

        return company;
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Failed to get company from Core Module DB: {CompanyId}", companyId);
        return null;
    }
}
// Helper method - Make this a class-level method so it can be reused
private T? GetValueFromDict<T>(IDictionary<string, object>? dictionary, string key)
{
    if (dictionary == null) return default;

    // Try case-insensitive lookup
    string? foundKey = null;
    foreach (var k in dictionary.Keys)
    {
        if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
        {
            foundKey = k;
            break;
        }
    }

    if (foundKey == null || !dictionary.TryGetValue(foundKey, out var value))
        return default;

    if (value == null || value == DBNull.Value)
        return default;

    try
    {
        Type targetType = typeof(T);

        // Handle Nullable types
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = Nullable.GetUnderlyingType(targetType)!;
        }

        // Special handling for Guid
        if (targetType == typeof(Guid))
        {
            if (value is Guid guidValue)
                return (T)(object)guidValue;

            if (value is string strValue && Guid.TryParse(strValue, out Guid parsedGuid))
                return (T)(object)parsedGuid;

            if (value is byte[] bytes && bytes.Length == 16)
                return (T)(object)new Guid(bytes);

            return default;
        }

        // Special handling for DateTime
        if (targetType == typeof(DateTime))
        {
            if (value is DateTime dateTime)
                return (T)(object)dateTime;

            if (value is string strDate && DateTime.TryParse(strDate, out DateTime parsedDate))
                return (T)(object)parsedDate;

            return default;
        }

        // For other types, use Convert.ChangeType
        return (T)Convert.ChangeType(value, targetType);
    }
    catch (Exception ex)
    {
        _logger?.LogWarning(ex, "Error converting value for key {Key} to type {Type}", key, typeof(T).Name);
        return default;
    }
}




    private async Task<PositionDto?> GetPositionFromGrpcAsync(Guid positionId)
    {
        try
        {
            var response = await _corHrmm.GetPosition(positionId.ToString());
            if (response?.Res != null)
            {
                return new PositionDto
                {
                    Id = Guid.Parse(response.Res.Id),
                    Name = response.Res.Name ?? "Unknown"
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get position from gRPC: {PositionId}", positionId);
            return null;
        }
    }

    private async Task<JobGradeDto?> GetJobGradeFromGrpcAsync(Guid jobGradeId)
    {
        try
        {
            var response = await _corHrmm.GetJobGrade(jobGradeId.ToString());
            if (response?.Res != null)
            {
                return new JobGradeDto
                {
                    Id = Guid.Parse(response.Res.Id),
                    Name = response.Res.Name ?? "Unknown"
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get job grade from gRPC: {JobGradeId}", jobGradeId);
            return null;
        }
    }

    // ============================================
    // HELPER METHODS
    // ============================================

    private static Guid ConvertToGuid(object? value)
    {
        if (value == null || value == DBNull.Value)
            return Guid.Empty;

        if (value is Guid guid)
            return guid;

        if (value is string str && Guid.TryParse(str, out var parsedGuid))
            return parsedGuid;

        if (value is byte[] bytes && bytes.Length == 16)
            return new Guid(bytes);

        return Guid.Empty;
    }

    private static Guid? ConvertToNullableGuid(object? value)
    {
        if (value == null || value == DBNull.Value)
            return null;

        if (value is Guid guid)
            return guid;

        if (value is string str && Guid.TryParse(str, out var parsedGuid))
            return parsedGuid;

        if (value is byte[] bytes && bytes.Length == 16)
            return new Guid(bytes);

        return null;
    }
}

// ============================================
// DTOS (Data Transfer Objects)
// ============================================

public class UserScopeDto
{
    public EmployeeDto Employee { get; set; } = new();
    public DepartmentDto? Department { get; set; }
    public BranchDto? Branch { get; set; }
    public CompanyDto? Company { get; set; }
    public PositionDto? Position { get; set; }
    public JobGradeDto? JobGrade { get; set; }
}

public class PersonDto
{
    public Guid PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
}

public class EmployeeDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public Guid? PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? JobGradeId { get; set; }
    public string EmpState { get; set; } = string.Empty;
    public string EmpType { get; set; } = string.Empty;
    public string EmpNature { get; set; } = string.Empty;
    public DateTime? EmploymentDate { get; set; }
    public string WorkArrangement { get; set; } = string.Empty;
}

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public Guid? BranchId { get; set; }
}

public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
}

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
}

public class PositionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class JobGradeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
public class PersonResult
{
    public Guid PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
}