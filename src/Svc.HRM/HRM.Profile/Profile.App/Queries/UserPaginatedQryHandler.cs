using Common;
using Dapper;
using EthiopianCalendar;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Microsoft.Extensions.Logging;

using System.Text;

namespace Profile.App.Queries;

public class UserPaginatedQryHandler : IRequestHandler<UserPaginatedQry, PaginatedResult<UserListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ILogger<UserPaginatedQryHandler> _logger;

    public UserPaginatedQryHandler(
        IDapperHelper dapper,
        ICorModClient corMod,
        ICorHrmmClient corHRMM,
        ILogger<UserPaginatedQryHandler> logger)
    {
        _dapper = dapper;
        _corMod = corMod;
        _corHRMM = corHRMM;
        _logger = logger;
    }

    public async Task<PaginatedResult<UserListDto>> Handle(UserPaginatedQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("=== UserPaginatedQryHandler Start ===");
            _logger.LogInformation("Request - PageNumber: {PageNumber}, PageSize: {PageSize}, SearchTerm: {SearchTerm}",
                request.PageNumber, request.PageSize, request.SearchTerm);

            // Get departments and positions from gRPC services
            var deptTask = _corMod.GetListDept(ct);
            var posTask = _corHRMM.GetListPosition(ct);
            await Task.WhenAll(deptTask, posTask);

            var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
            var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));

            // Build the base query without search filters
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

            // Add filters (not search yet)
            if (!string.IsNullOrEmpty(request.EmpState))
            {
                qb.WhereRaw<Employee>(e, x => x.EmpState, "=", request.EmpState);
                _logger.LogInformation("Filtering by EmpState: {EmpState}", request.EmpState);
            }

            if (!string.IsNullOrEmpty(request.Gender))
            {
                qb.WhereRaw<Person>(p, x => x.Gender, "=", request.Gender);
                _logger.LogInformation("Filtering by Gender: {Gender}", request.Gender);
            }

            // For search, we'll handle it in memory after getting all data
            // First, get all matching records without search filter
            var allQb = new QueryBuilder()
                .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmpState, x => x.DepartmentId, x => x.PositionId,
                    x => x.DateAdd, x => x.DateMod!, x => x.xmin)
                .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm,
                    x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
                .From<Employee>(e)
                .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
                .Where<Employee>(e, x => x.IsDeleted == false);

            if (!string.IsNullOrEmpty(request.EmpState))
            {
                allQb.WhereRaw<Employee>(e, x => x.EmpState, "=", request.EmpState);
            }

            if (!string.IsNullOrEmpty(request.Gender))
            {
                allQb.WhereRaw<Person>(p, x => x.Gender, "=", request.Gender);
            }

            // Count query for total (without search)
            var countQb = new QueryBuilder()
                .Select<Employee>(e, x => x.Id)
                .From<Employee>(e)
                .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
                .Where<Employee>(e, x => x.IsDeleted == false);

            if (!string.IsNullOrEmpty(request.EmpState))
            {
                countQb.WhereRaw<Employee>(e, x => x.EmpState, "=", request.EmpState);
            }

            if (!string.IsNullOrEmpty(request.Gender))
            {
                countQb.WhereRaw<Person>(p, x => x.Gender, "=", request.Gender);
            }

            var (countSql, countParams) = countQb.BuildCount();
            var totalCountBeforeSearch = await _dapper.ExecuteScalarAsync<int>(countSql, countParams, ct);
            _logger.LogInformation("Total count before search: {TotalCount}", totalCountBeforeSearch);

            if (totalCountBeforeSearch == 0)
            {
                return EmptyResult(request);
            }

            // Get ALL data (without pagination first) to apply search in memory
            allQb.Limit(10000); // Get up to 10000 records for search
            var (allSql, allParams) = allQb.Build();
            var allRows = await _dapper.QueryAsync<EmpJoinRow>(allSql, allParams, ct);
            _logger.LogInformation("Retrieved {RowCount} rows for search", allRows.Count());

            // Build the result list with department/position names
            var allResults = new List<UserListDto>();
            foreach (var row in allRows)
            {
                deptDict.TryGetValue(row.DepartmentId, out var dept);
                posDict.TryGetValue(row.PositionId, out var pos);

                allResults.Add(new UserListDto
                {
                    Id = row.Id,
                    Code = row.Code,
                    EmpFullName = $"{row.FirstName} {row.MiddleName} {row.LastName}",
                    EmpFullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}",
                    Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
                    Department = dept?.Name ?? "",
                    Position = pos?.Name ?? "",
                    Branch = dept?.NameAm ?? "",
                    EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
                    HasAccount = false,
                    IsAccountActive = false,
                    UserId = null
                });
            }

            // Apply search filter in memory (OR condition)
            var filteredResults = allResults;
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                filteredResults = allResults.Where(r =>
                    r.Code?.ToLower().Contains(searchLower) == true ||
                    r.EmpFullName?.ToLower().Contains(searchLower) == true ||
                    r.EmpFullNameAm?.ToLower().Contains(searchLower) == true ||
                    r.Department?.ToLower().Contains(searchLower) == true ||
                    r.Position?.ToLower().Contains(searchLower) == true
                ).ToList();
                _logger.LogInformation("After search filter: {ResultCount} results", filteredResults.Count);
            }

            // Apply department filter
            if (!string.IsNullOrEmpty(request.Department))
            {
                filteredResults = filteredResults.Where(r => r.Department == request.Department).ToList();
            }

            // Apply branch filter
            if (!string.IsNullOrEmpty(request.Branch))
            {
                filteredResults = filteredResults.Where(r => r.Branch == request.Branch).ToList();
            }

            var totalCount = filteredResults.Count;
            _logger.LogInformation("Total count after all filters: {TotalCount}", totalCount);

            if (totalCount == 0)
            {
                return EmptyResult(request);
            }

            // Apply pagination
            var offset = (request.PageNumber - 1) * request.PageSize;
            var paginatedResults = filteredResults.Skip(offset).Take(request.PageSize).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedResult<UserListDto>
            {
                Items = paginatedResults,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated users: {Error}", ex.Message);
            return EmptyResult(request);
        }
    }

    private PaginatedResult<UserListDto> EmptyResult(UserPaginatedQry request)
    {
        return new PaginatedResult<UserListDto>
        {
            Items = new List<UserListDto>(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 0,
            TotalPages = 0,
            HasPreviousPage = false,
            HasNextPage = false
        };
    }
}