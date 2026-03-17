using Common;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class EmpAllAdminQry : IRequest<List<EmpListDto>> { }



public class EmpAllAdminHandler : IRequestHandler<EmpAllAdminQry, List<EmpListDto>>
{
    private readonly IHrmProfileClient _hrmProfile;
    private readonly IDapperHelper _dapper;
    public EmpAllAdminHandler(IHrmProfileClient hrmProfile, IDapperHelper dapper)
    {
        _hrmProfile = hrmProfile;
        _dapper = dapper;
    }

    public async Task<List<EmpListDto>> Handle(EmpAllAdminQry request, CancellationToken ct)
    {
        var empTask = await _hrmProfile.GetAdminEmpList(ct);
        var empList = empTask.Res.ToList();
        var empMap = empList.Select(x => new
        {
            Raw = x,
            Id = Guid.TryParse(x.Id, out var g) ? g : Guid.Empty
        }).Where(x => x.Id != Guid.Empty).ToList();
        var empIds = empMap.Select(x => x.Id).ToArray();

        const string v = "v";
        var qb = new QueryBuilder()
            .SelectAs<AppUser, IdDto>(v, x => x.EmployeeId, x => x.Id)
            .From<AppUser>(v)
            .WhereIn<AppUser>(v, x => x.EmployeeId, empIds);
        var (sql, parameters) = qb.Build();
        var users = await _dapper.QueryAsync<IdDto>(sql, parameters, ct);
        var userSet = users.Select(x => x.Id).ToHashSet();
        var result = new List<EmpListDto>();

        foreach (var x in empMap)
        {
            result.Add(new EmpListDto
            {
                Id = x.Id,
                Code = x.Raw.Code,
                EmpFullName = x.Raw.Name,
                EmpFullNameAm = x.Raw.NameAm,
                Gender = x.Raw.Gender,
                Branch = x.Raw.Branch,
                Department = x.Raw.Dept,
                Position = x.Raw.Position,
                EmpState = x.Raw.Status,
                HasAccount = userSet.Contains(x.Id)
            });
        }
        return result;
    }
}