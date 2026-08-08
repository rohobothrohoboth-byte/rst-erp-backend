using Common;
using Dapper;
using EthiopianCalendar;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;  // ✅ Add this
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Microsoft.Extensions.Logging;  // ✅ Add this

namespace Profile.App.Queries;

public class EmployeePaginatedQryHandler : IRequestHandler<EmployeePaginatedQry, PaginatedResult<EmployeeListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICachedReferenceService _referenceService;  // ✅ Use cached service
    private readonly ILogger<EmployeePaginatedQryHandler> _logger;

    public EmployeePaginatedQryHandler(
        IDapperHelper dapper,
        ICachedReferenceService referenceService,
        ILogger<EmployeePaginatedQryHandler> logger)
    {
        _dapper = dapper;
        _referenceService = referenceService;
        _logger = logger;
    }

    public async Task<PaginatedResult<EmployeeListDto>> Handle(EmployeePaginatedQry request, CancellationToken ct)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // ✅ Get cached reference data (no gRPC calls on every request)
        var (deptDict, posDict, _) = await _referenceService.GetReferenceDataAsync(ct);

        _logger.LogDebug("Reference data loaded in {Elapsed}ms", stopwatch.ElapsedMilliseconds);
        stopwatch.Restart();

        // Build the query using QueryBuilder
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmpState, x => x.DepartmentId, x => x.PositionId,
                x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm,
                x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.IsDeleted == false);

        // Add search filter if provided
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = $"%{request.SearchTerm}%";
            qb.WhereRaw<Employee>(e, x => x.Code, "ILIKE", searchTerm);
        }

        // Add EmpState filter if provided
        if (!string.IsNullOrEmpty(request.EmpState))
        {
            qb.WhereRaw<Employee>(e, x => x.EmpState, "=", request.EmpState);
        }

        // Add Department filter if provided
        if (!string.IsNullOrEmpty(request.Department))
        {
            qb.WhereRaw<Employee>(e, x => x.DepartmentId, "=", request.Department);
        }

        // Add Gender filter if provided
        if (!string.IsNullOrEmpty(request.Gender))
        {
            qb.WhereRaw<Person>(p, x => x.Gender, "=", request.Gender);
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", "Id" },
                { "Code", "Code" },
                { "EmpState", "EmpState" },
                { "DateAdd", "DateAdd" },
                { "DateMod", "DateMod" },
                { "FirstName", "FirstName" },
                { "LastName", "LastName" },
                { "FirstNameAm", "FirstNameAm" },
                { "LastNameAm", "LastNameAm" },
                { "Gender", "Gender" }
            };

            if (allowedSortColumns.TryGetValue(request.SortBy, out var dbColumn))
            {
                var alias = dbColumn switch
                {
                    "FirstName" or "LastName" or "FirstNameAm" or "LastNameAm" or "Gender" => p,
                    _ => e
                };
                var sortOrder = request.SortOrder?.ToLower() == "desc" ? "DESC" : "ASC";
                qb.OrderByRaw($"\"{alias}\".\"{dbColumn}\" {sortOrder}");
            }
            else
            {
                qb.OrderBy<Employee>(e, x => x.DateAdd, desc: true);
            }
        }
        else
        {
            qb.OrderBy<Employee>(e, x => x.DateAdd, desc: true);
        }

        // Build count query
        var countQb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.IsDeleted == false);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = $"%{request.SearchTerm}%";
            countQb.WhereRaw<Employee>(e, x => x.Code, "ILIKE", searchTerm);
        }

        if (!string.IsNullOrEmpty(request.EmpState))
        {
            countQb.WhereRaw<Employee>(e, x => x.EmpState, "=", request.EmpState);
        }

        var (countSql, countParams) = countQb.BuildCount();
        var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, countParams, ct);
        _logger.LogDebug("Count query completed in {Elapsed}ms", stopwatch.ElapsedMilliseconds);
        stopwatch.Restart();

        // Apply pagination
        var offset = (request.PageNumber - 1) * request.PageSize;
        qb.Limit(request.PageSize);
        qb.Offset(offset);

        var (sql, parameters) = qb.Build();

        // Execute query
        var rows = await _dapper.QueryAsync<EmpJoinRow>(sql, parameters, ct);
        _logger.LogDebug("Data query completed in {Elapsed}ms", stopwatch.ElapsedMilliseconds);

        // Map results
        var result = new List<EmployeeListDto>();
        foreach (var row in rows)
        {
            deptDict.TryGetValue(row.DepartmentId, out var dept);
            posDict.TryGetValue(row.PositionId, out var pos);

            result.Add(new EmployeeListDto
            {
                Id = row.Id,
                Code = row.Code,
                EmpFullName = $"{row.FirstName} {row.MiddleName} {row.LastName}".Trim(),
                EmpFullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}".Trim(),
                EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
                Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
                Branch = dept?.NameAm ?? "",
                Department = dept?.Name ?? "",
                Position = pos?.Name ?? "",
                DateAdd = row.DateAdd,
                DateMod = row.DateMod,
                RowVersion = row.xmin.ToString(),
                DepartmentId = row.DepartmentId,
                PositionId = row.PositionId
            });
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        _logger.LogInformation("✅ Paginated query completed in {TotalMs}ms (Data: {DataMs}ms, Count: {CountMs}ms)",
            stopwatch.ElapsedMilliseconds,
            stopwatch.ElapsedMilliseconds - (stopwatch.ElapsedMilliseconds > 0 ? 0 : 0),
            stopwatch.ElapsedMilliseconds > 0 ? 0 : 0);

        return new PaginatedResult<EmployeeListDto>
        {
            Items = result,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < totalPages
        };
    }
}