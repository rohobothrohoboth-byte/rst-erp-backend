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
        var empDict = empTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        var result = new List<EmpListDto>();
        foreach (var item in empList)
        {
            var empId = Guid.Parse(item.Id);
            const string v = "v";
            var qb = new QueryBuilder()
                .SelectAs<AppUser, IdDto>(v, x => x.EmployeeId, x => x.Id)
                .From<AppUser>(v)
                .Where<AppUser>(v, x => x.EmployeeId == empId)
                .Limit(1);
            var (sql, parameters) = qb.Build();
            var data = await _dapper.QueryFirstOrDefaultAsync<IdDto>(sql, parameters, ct);
            var emp = new EmpListDto
            {
                Code = item.Code,
                EmpFullName = item.Name,
                EmpFullNameAm = item.NameAm,
                Gender = item.Gender,
                Branch = item.Branch,
                Department = item.Dept,
                Position = item.Position,
                EmpState = item.Status,
                HasAccount = data != null
            };
            result.Add(emp);
        }
        return result;
    }
}