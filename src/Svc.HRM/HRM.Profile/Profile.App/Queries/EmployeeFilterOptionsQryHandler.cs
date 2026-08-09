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



 public class EmployeeFilterOptionsQryHandler : IRequestHandler<EmployeeFilterOptionsQry, EmployeeFilterOptionsDto>
 {
     private readonly IDapperHelper _dapper;
     private readonly ICachedReferenceService _referenceService;
     private readonly ILogger<EmployeeFilterOptionsQryHandler> _logger;

     public EmployeeFilterOptionsQryHandler(
         IDapperHelper dapper,
         ICachedReferenceService referenceService,
         ILogger<EmployeeFilterOptionsQryHandler> logger)
     {
         _dapper = dapper;
         _referenceService = referenceService;
         _logger = logger;
     }

     public async Task<EmployeeFilterOptionsDto> Handle(EmployeeFilterOptionsQry request, CancellationToken ct)
     {
         var options = new EmployeeFilterOptionsDto();

         // Get reference data (departments, positions, etc.)
         var (deptDict, posDict, _) = await _referenceService.GetReferenceDataAsync(ct);

         // Get all employees with their person data using Dapper
         const string e = "e";
         const string p = "p";

         var qb = new QueryBuilder()
             .Select<Employee>(e, x => x.Id, x => x.DepartmentId, x => x.PositionId, x => x.EmpState, x => x.EmploymentNature)
             .Select<Person>(p, x => x.Gender)
             .From<Employee>(e)
             .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
             .Where<Employee>(e, x => x.IsDeleted == false);

         var (sql, parameters) = qb.Build();

         var rows = await _dapper.QueryAsync<EmpFilterRow>(sql, parameters, ct);

         // Get distinct values
         var departmentIds = rows.Select(r => r.DepartmentId).Distinct().ToList();
         var positionIds = rows.Select(r => r.PositionId).Distinct().ToList();

         // Build Departments from reference data
         options.Departments = departmentIds
             .Where(id => deptDict.ContainsKey(id))
             .Select(id => new IdNameDto
             {
                 Id = id,
                 Name = deptDict[id]?.Name ?? id.ToString()
             })
             .ToList();

         // Build Branches/Positions from reference data
         options.Branches = positionIds
             .Where(id => posDict.ContainsKey(id))
             .Select(id => new IdNameDto
             {
                 Id = id,
                 Name = posDict[id]?.Name ?? id.ToString()
             })
             .ToList();

         // Get distinct EmpStates
         options.EmpStates = rows
             .Select(r => r.EmpState)
             .Where(s => s != null)
             .Select(s => s!)
             .Distinct()
             .OrderBy(s => s)
             .ToList();

         // Get distinct EmploymentNature
         options.EmpNatures = rows
             .Select(r => r.EmploymentNature)
             .Where(en => en != null)
             .Select(en => en!)
             .Distinct()
             .OrderBy(en => en)
             .ToList();

         // Get distinct Genders
         options.Genders = rows
             .Select(r => r.Gender)
             .Where(g => g != null)
             .Select(g => g!)
             .Distinct()
             .OrderBy(g => g)
             .ToList();

         return options;
     }
 }

 // DTO for Dapper query result
 internal class EmpFilterRow
 {
     public Guid Id { get; set; }
     public Guid DepartmentId { get; set; }
     public Guid PositionId { get; set; }
     public string? EmpState { get; set; }
     public string? EmploymentNature { get; set; }
     public string? Gender { get; set; }
 }