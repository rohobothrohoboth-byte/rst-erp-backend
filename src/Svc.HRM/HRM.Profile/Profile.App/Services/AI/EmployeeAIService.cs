// Services/AI/EmployeeAIService.cs
using Dapper;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Common;
using System.Text;
using Profile.App.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace Profile.App.Services.AI;


    public class EmployeeAIService
    {
        private readonly IDapperHelper _dapper;
        private readonly ILogger<EmployeeAIService> _logger;
        private readonly IMemoryCache _cache;

        public EmployeeAIService(
            IDapperHelper dapper,
            ILogger<EmployeeAIService> logger,
            IMemoryCache cache)
        {
            _dapper = dapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<AIResponse> ProcessQueryAsync(string userQuery)
        {
            try
            {
                var parsed = ParseEmployeeQuery(userQuery);
                var result = await ExecuteQueryAsync(parsed);
                var response = FormatResponse(parsed, result);

                return new AIResponse
                {
                    Success = true,
                    Message = response,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI query: {Query}", userQuery);
                return new AIResponse
                {
                    Success = false,
                    Message = "Sorry, I couldn't process your request. Please try again.",
                    Error = ex.Message
                };
            }
        }

        private EmployeeQuery ParseEmployeeQuery(string query)
        {
            var lower = query.ToLower();
            var parsed = new EmployeeQuery
            {
                OriginalQuery = query
            };

            if (ContainsAny(lower, "count", "how many", "number of", "total"))
                parsed.QueryType = "count";
            else if (ContainsAny(lower, "list", "show", "display", "get", "who"))
                parsed.QueryType = "list";
            else if (ContainsAny(lower, "statistics", "stats", "summary"))
                parsed.QueryType = "stats";
            else if (ContainsAny(lower, "retire", "retirement", "retiring"))
                parsed.QueryType = "retirement";

            if (ContainsAny(lower, "male", "men", "man"))
                parsed.Gender = "Male";
            else if (ContainsAny(lower, "female", "women", "woman"))
                parsed.Gender = "Female";

            var yearMatch = Regex.Match(query, @"\b(20\d{2})\b");
            parsed.Year = yearMatch.Success ? int.Parse(yearMatch.Value) : DateTime.Now.Year;

            var deptPatterns = new[] { "hr", "human resources", "finance", "it", "operations", "sales", "marketing", "procurement", "inventory", "crm" };
            foreach (var dept in deptPatterns)
            {
                if (lower.Contains(dept))
                {
                    parsed.Department = dept;
                    break;
                }
            }

            if (ContainsAny(lower, "details", "info", "information", "data"))
                parsed.IncludeDetails = true;

            return parsed;
        }

        private async Task<object> ExecuteQueryAsync(EmployeeQuery parsed)
        {
            return parsed.QueryType switch
            {
                "count" => await GetEmployeeCountAsync(parsed),
                "list" => await GetEmployeeListAsync(parsed),
                "stats" => await GetGeneralDataAsync(parsed),
                "retirement" => await GetRetirementDataAsync(parsed),
                _ => await GetGeneralDataAsync(parsed)
            };
        }

        private async Task<object> GetEmployeeCountAsync(EmployeeQuery parsed)
        {
            // ✅ Joined with EmpBio for BirthDate
            var sql = new StringBuilder(@"
                SELECT COUNT(*)
                FROM ""Employee"" e
                INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
                LEFT JOIN ""EmpBio"" eb ON e.""Id"" = eb.""EmployeeId""
                WHERE 1=1");

            if (!string.IsNullOrEmpty(parsed.Gender))
                sql.Append($" AND p.\"Gender\" = '{parsed.Gender}'");

            if (!string.IsNullOrEmpty(parsed.Department))
            {
                sql.Append(@"
                    AND e.""DepartmentId"" IN (
                        SELECT ""Id"" FROM ""Departments""
                        WHERE LOWER(""Name"") LIKE @Department
                    )");
            }

            if (parsed.QueryType == "retirement")
            {
                sql.Append(@"
                    AND EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) >= 60");
            }

            var parameters = new { Department = $"%{parsed.Department}%" };
            var count = await _dapper.ExecuteScalarAsync<int>(sql.ToString(), parameters);

            // ✅ Breakdown by gender
            var breakdownSql = @"
                SELECT
                    p.""Gender"",
                    COUNT(*) as Count
                FROM ""Employee"" e
                INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
                LEFT JOIN ""EmpBio"" eb ON e.""Id"" = eb.""EmployeeId""
                WHERE 1=1";

            if (!string.IsNullOrEmpty(parsed.Department))
            {
                breakdownSql += @"
                    AND e.""DepartmentId"" IN (
                        SELECT ""Id"" FROM ""Departments""
                        WHERE LOWER(""Name"") LIKE @Department
                    )";
            }

            if (parsed.QueryType == "retirement")
            {
                breakdownSql += @"
                    AND EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) >= 60";
            }

            breakdownSql += " GROUP BY p.\"Gender\"";
            var breakdown = await _dapper.QueryAsync<GenderBreakdown>(breakdownSql, parameters);

            return new
            {
                Total = count,
                Breakdown = breakdown,
                Filters = new { parsed.Gender, parsed.Department }
            };
        }

        private async Task<object> GetEmployeeListAsync(EmployeeQuery parsed)
        {
            // ✅ Joined with EmpBio for BirthDate
            var sql = new StringBuilder(@"
                SELECT
                    e.""Id"",
                    e.""Code"",
                    p.""FirstName"",
                    p.""MiddleName"",
                    p.""LastName"",
                    p.""Gender"",
                    eb.""BirthDate"",
                    e.""EmploymentDate"" as ""DateAdd"",
                    d.""Name"" as ""Department"",
                    pos.""Title"" as ""Position"",
                    b.""Name"" as ""Branch"",
                    EXTRACT(YEAR FROM AGE(CURRENT_DATE, e.""EmploymentDate"")) as ""YearsEmployed"",
                    EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) as ""Age""
                FROM ""Employee"" e
                INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
                LEFT JOIN ""EmpBio"" eb ON e.""Id"" = eb.""EmployeeId""
                LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id""
                LEFT JOIN ""Positions"" pos ON e.""PositionId"" = pos.""Id""
                LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id""
                WHERE 1=1");

            if (!string.IsNullOrEmpty(parsed.Gender))
                sql.Append($" AND p.\"Gender\" = '{parsed.Gender}'");

            if (!string.IsNullOrEmpty(parsed.Department))
                sql.Append(@" AND LOWER(d.""Name"") LIKE @Department");

            if (parsed.QueryType == "retirement")
            {
                sql.Append(@"
                    AND EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) >= 55");
            }

            sql.Append(" ORDER BY e.\"EmploymentDate\" DESC LIMIT 20");

            var parameters = new { Department = $"%{parsed.Department}%" };
            var employees = await _dapper.QueryAsync<EmployeeListDto>(sql.ToString(), parameters);

            return employees;
        }

        private async Task<object> GetGeneralDataAsync(EmployeeQuery parsed)
        {
            // ✅ Joined with EmpBio for BirthDate
            var sql = @"
                SELECT
                    COUNT(*) as TotalEmployees,
                    COUNT(CASE WHEN p.""Gender"" = 'Male' THEN 1 END) as MaleCount,
                    COUNT(CASE WHEN p.""Gender"" = 'Female' THEN 1 END) as FemaleCount,
                    COUNT(CASE WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) >= 60 THEN 1 END) as RetirementEligible
                FROM ""Employee"" e
                INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
                LEFT JOIN ""EmpBio"" eb ON e.""Id"" = eb.""EmployeeId""";

            var stats = await _dapper.QueryFirstOrDefaultAsync<GeneralStatsDto>(sql);
            return stats;
        }

        private async Task<object> GetRetirementDataAsync(EmployeeQuery parsed)
        {
            // ✅ Using EmpBio.BirthDate for retirement eligibility (age >= 60)
            var sql = @"
                SELECT
                    e.""Id"",
                    e.""Code"",
                    p.""FirstName"",
                    p.""MiddleName"",
                    p.""LastName"",
                    p.""Gender"",
                    eb.""BirthDate"",
                    e.""EmploymentDate"",
                    d.""Name"" as ""Department"",
                    pos.""Title"" as ""Position"",
                    EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) as ""Age"",
                    EXTRACT(YEAR FROM AGE(CURRENT_DATE, e.""EmploymentDate"")) as ""YearsEmployed"",
                    CASE
                        WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) >= 60
                        THEN 'Eligible for Retirement'
                        WHEN EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) >= 55
                        THEN 'Eligible in 5 years'
                        ELSE 'Not Eligible'
                    END as ""RetirementStatus""
                FROM ""Employee"" e
                INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
                LEFT JOIN ""EmpBio"" eb ON e.""Id"" = eb.""EmployeeId""
                LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id""
                LEFT JOIN ""Positions"" pos ON e.""PositionId"" = pos.""Id""
                WHERE EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) >= 55
                ORDER BY EXTRACT(YEAR FROM AGE(CURRENT_DATE, eb.""BirthDate"")) DESC";

            var employees = await _dapper.QueryAsync<RetirementEmployeeDto>(sql);

            var totalEligible = employees.Count(e => e.Age >= 60);
            var eligibleIn5Years = employees.Count(e => e.Age >= 55 && e.Age < 60);

            return new
            {
                TotalEligible = totalEligible,
                EligibleIn5Years = eligibleIn5Years,
                Employees = employees.Take(10),
                Summary = new
                {
                    ByGender = employees.GroupBy(e => e.Gender).Select(g => new { Gender = g.Key, Count = g.Count() }),
                    ByDepartment = employees.GroupBy(e => e.Department).Select(g => new { Department = g.Key, Count = g.Count() })
                }
            };
        }

        private string FormatResponse(EmployeeQuery parsed, object result)
        {
            var sb = new StringBuilder();

            switch (parsed.QueryType)
            {
                case "count":
                    var countData = result as dynamic;
                    if (countData != null)
                    {
                        var total = countData.Total;
                        var breakdown = countData.Breakdown as IEnumerable<GenderBreakdown>;

                        sb.AppendLine($"📊 **Employee Count Summary**");
                        sb.AppendLine();
                        sb.AppendLine($"**Total Employees:** {total}");

                        if (breakdown != null && breakdown.Any())
                        {
                            sb.AppendLine();
                            sb.AppendLine("**Breakdown by Gender:**");
                            foreach (var item in breakdown)
                            {
                                var percentage = total > 0 ? ((double)item.Count / total * 100) : 0;
                                if (item.Gender == "Male" || item.Gender == "Female")
                                {
                                    sb.AppendLine($"• {item.Gender}: **{item.Count}** ({percentage:F1}%)");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(parsed.Department))
                            sb.AppendLine($"• Department: **{parsed.Department.ToUpper()}**");
                        if (!string.IsNullOrEmpty(parsed.Gender))
                            sb.AppendLine($"• Gender: **{parsed.Gender}**");
                    }
                    break;

                case "list":
                    var employees = result as IEnumerable<EmployeeListDto>;
                    if (employees != null && employees.Any())
                    {
                        sb.AppendLine($"📋 **Employee List**");
                        sb.AppendLine();
                        sb.AppendLine($"Found **{employees.Count()}** employees");
                        sb.AppendLine();
                        sb.AppendLine("| # | Code | Name | Gender | Department | Position | Age |");
                        sb.AppendLine("|---|------|------|--------|------------|----------|-----|");

                        var index = 1;
                        foreach (var emp in employees.Take(10))
                        {
                            var fullName = $"{emp.FirstName} {emp.MiddleName} {emp.LastName}".Trim();
                            sb.AppendLine($"| {index} | {emp.Code} | {fullName} | {emp.Gender} | {emp.Department} | {emp.Position} | {emp.Age} |");
                            index++;
                        }

                        if (employees.Count() > 10)
                            sb.AppendLine($"*Showing 10 of {employees.Count()} employees*");
                    }
                    else
                    {
                        sb.AppendLine("❌ No employees found matching your criteria.");
                    }
                    break;

                case "retirement":
                    var retirementData = result as dynamic;
                    if (retirementData != null)
                    {
                        sb.AppendLine($"👴 **Retirement Eligibility**");
                        sb.AppendLine();
                        sb.AppendLine($"**Eligible Now (Age 60+):** {retirementData.TotalEligible}");
                        sb.AppendLine($"**Eligible in 5 Years (Age 55+):** {retirementData.EligibleIn5Years}");

                        var employees2 = retirementData.Employees as IEnumerable<RetirementEmployeeDto>;
                        if (employees2 != null && employees2.Any())
                        {
                            sb.AppendLine();
                            sb.AppendLine("**Eligible Employees:**");
                            foreach (var emp in employees2)
                            {
                                var fullName = $"{emp.FirstName} {emp.MiddleName} {emp.LastName}".Trim();
                                sb.AppendLine($"• {fullName} ({emp.Code}) - {emp.Gender} - Age: {emp.Age} - {emp.Department} - {emp.RetirementStatus}");
                            }
                        }
                    }
                    break;

                default:
                    var stats = result as GeneralStatsDto;
                    if (stats != null)
                    {
                        sb.AppendLine($"📊 **General Employee Statistics**");
                        sb.AppendLine();
                        sb.AppendLine($"• Total Employees: **{stats.TotalEmployees}**");
                        sb.AppendLine($"• Male: **{stats.MaleCount}**");
                        sb.AppendLine($"• Female: **{stats.FemaleCount}**");
                        sb.AppendLine($"• Retirement Eligible (60+): **{stats.RetirementEligible}**");
                    }
                    break;
            }

            return sb.ToString();
        }

        private bool ContainsAny(string text, params string[] keywords)
        {
            return keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
        }
    }

    // ============================================================
    // DTOs
    // ============================================================

    public class AIResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; } = new();
        public string Error { get; set; } = string.Empty;
    }

    public class EmployeeQuery
    {
        public string OriginalQuery { get; set; } = string.Empty;
        public string QueryType { get; set; } = "general";
        public string Gender { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Department { get; set; } = string.Empty;
        public bool IncludeDetails { get; set; }
    }

    public class EmployeeListDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime DateAdd { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public int YearsEmployed { get; set; }
        public int Age { get; set; }
    }

    public class RetirementEmployeeDto : EmployeeListDto
    {
        public string RetirementStatus { get; set; } = string.Empty;
    }

    public class GenderBreakdown
    {
        public string Gender { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class GeneralStatsDto
    {
        public int TotalEmployees { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int RetirementEligible { get; set; }
    }
