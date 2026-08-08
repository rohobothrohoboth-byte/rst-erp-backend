using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Models.Entities;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Helpers.Services;
using Helpers;


namespace Svc.Auth.Queries;

// Add this new query class
public class GetAllAppUsersQry : IRequest<List<AppUserDto>> { }

// Add this handler class
public class GetAllAppUsersHandler : IRequestHandler<GetAllAppUsersQry, List<AppUserDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetAllAppUsersHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<AppUserDto>> Handle(GetAllAppUsersQry request, CancellationToken ct)
    {
        var users = await _userManager.Users.ToListAsync(ct);

        return users.Select(u => new AppUserDto
        {
            Id = u.Id.ToString(),
            EmployeeId = u.EmployeeId?.ToString() ?? string.Empty,
            IsActive = u.IsActive
        }).ToList();
    }
}

public class GetUsersByBranchQry : IRequest<List<AppUserWithOrgDto>>
{
    public Guid BranchId { get; set; }
}


public class GetUsersByBranchHandler : IRequestHandler<GetUsersByBranchQry, List<AppUserWithOrgDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICacheService _cache;
    private readonly ILogger<GetUsersByBranchHandler> _logger;

    public GetUsersByBranchHandler(IDapperHelper dapper, ICacheService cache, ILogger<GetUsersByBranchHandler> logger)
    {
        _dapper = dapper;
        _cache = cache;
        _logger = logger;
    }

  public async Task<List<AppUserWithOrgDto>> Handle(GetUsersByBranchQry request, CancellationToken ct)
  {
      var cacheKey = $"users_by_branch_{request.BranchId}";

      // ✅ Store result in a variable
      var result = await _cache.GetOrCreateAsync(cacheKey, async (cancellationToken) =>
      {
          _logger.LogInformation("Cache miss for {CacheKey}, fetching from database", cacheKey);

          const string sql = @"
              SELECT
                  u.""Id""::text as Id,
                  u.""EmployeeId""::text as EmployeeId,
                  u.""UserName"",
                  u.""Email"",
                  u.""IsActive"",
                  u.""BranchId"",
                  u.""DepartmentId"",
                  u.""PositionId"",
                  b.""Name"" as BranchName,
                  d.""Name"" as DepartmentName,
                  p.""Name"" as PositionName
              FROM ""AppUser"" u
              LEFT JOIN ""Branches"" b ON u.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
              LEFT JOIN ""Departments"" d ON u.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
              LEFT JOIN ""Positions"" p ON u.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
              WHERE u.""BranchId"" = @BranchId";

          var users = await _dapper.QueryAsync<AppUserWithOrgDto>(sql, new { request.BranchId }, cancellationToken);
          return users.AsList();
      }, TimeSpan.FromMinutes(15), ct);


      return result ?? new List<AppUserWithOrgDto>();
  }
   }
public class GetUsersByDepartmentQry : IRequest<List<AppUserWithOrgDto>>
{
    public Guid DepartmentId { get; set; }
}

public class GetUsersByDepartmentHandler : IRequestHandler<GetUsersByDepartmentQry, List<AppUserWithOrgDto>>
{
    private readonly IDapperHelper _dapper;

    public GetUsersByDepartmentHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<AppUserWithOrgDto>> Handle(GetUsersByDepartmentQry request, CancellationToken ct)
    {
        // ✅ FIXED: Cast Id and EmployeeId to text to avoid deserialization issues
        const string sql = @"
            SELECT
                u.""Id""::text as Id,
                u.""EmployeeId""::text as EmployeeId,
                u.""UserName"",
                u.""Email"",
                u.""IsActive"",
                u.""BranchId"",
                u.""DepartmentId"",
                u.""PositionId"",
                b.""Name"" as BranchName,
                d.""Name"" as DepartmentName,
                p.""Name"" as PositionName
            FROM ""AppUser"" u
            LEFT JOIN ""Branches"" b ON u.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON u.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Positions"" p ON u.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            WHERE u.""DepartmentId"" = @DepartmentId";

        var users = await _dapper.QueryAsync<AppUserWithOrgDto>(sql, new { request.DepartmentId }, ct);
        return users.AsList();
    }
}

public class GetUserWithOrgQry : IRequest<UserWithOrgDto>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetUserWithOrgHandler : IRequestHandler<GetUserWithOrgQry, UserWithOrgDto>
{
    private readonly IDapperHelper _dapper;

    public GetUserWithOrgHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<UserWithOrgDto> Handle(GetUserWithOrgQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                u.""Id""::text as Id,
                u.""UserName"",
                u.""Email"",
                u.""IsActive"",
                u.""BranchId"",
                u.""DepartmentId"",
                u.""PositionId"",
                b.""Name"" as BranchName,
                d.""Name"" as DepartmentName,
                p.""Name"" as PositionName
            FROM ""AppUser"" u
            LEFT JOIN ""Branches"" b ON u.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON u.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Positions"" p ON u.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            WHERE u.""Id""::text = @UserId";

        // ✅ Option 1: Throw if not found (Recommended)
        var result = await _dapper.QueryFirstOrDefaultAsync<UserWithOrgDto>(sql, new { UserId = request.UserId }, ct);
        if (result == null)
            throw new DomainException($"User with ID '{request.UserId}' not found");
        return result;


    }
}