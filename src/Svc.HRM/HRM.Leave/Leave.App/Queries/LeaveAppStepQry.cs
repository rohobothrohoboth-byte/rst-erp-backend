using Common;
using Dapper;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class AppStepByChainIdQry : IRequest<List<LeaveAppStepListDto>> { public Guid Id { get; set; } }
public class LeaveAppStepByIdQry : IRequest<LeaveAppStepListDto?> { public Guid Id { get; set; } }



public class AppStepByChainIdHandler : IRequestHandler<AppStepByChainIdQry, List<LeaveAppStepListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;

    public AppStepByChainIdHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
    }

    public async Task<List<LeaveAppStepListDto>> Handle(AppStepByChainIdQry request, CancellationToken ct)
    {
        var empTask = await _hrmProfile.GetListEmp(ct);
        var empDict = empTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveAppStep>(v, x => x.Id, x => x.StepName, x => x.StepOrder, x => x.Role, x => x.IsFinal, x => x.EmployeeId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<LeaveAppChain>(c, x => x.EffectiveFrom)
            .From<LeaveAppStep>(v)
            .Join<LeaveAppStep, LeaveAppChain>(v, c, x => x.LeaveAppChainId, x => x.Id)
            .Where<LeaveAppStep>(v, x => x.LeaveAppChainId == request.Id);

        var (sql, parameters) = qb.Build();
        var result = new List<LeaveAppStepListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<LeaveAppStepListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            var emp = "Not Assigned";
            if (data.EmployeeId != null)
            {
                empDict.TryGetValue((Guid)data.EmployeeId, out var empN);
                emp = empN?.Name ?? "";
            }

            result.Add(new LeaveAppStepListDto
            {
                Id = data.Id,
                StepName = data.StepName,
                StepOrder = data.StepOrder,
                Role = data.Role,
                IsFinal = data.IsFinal,
                RoleStr = MyEnumHelper.FormatEnum<ApprovalRole>(data.Role),
                IsFinalStr = BoolToStr.FormatBool(data.IsFinal),
                Employee = emp,
                LeaveAppChain = $"From : {data.EffectiveFrom:MMMM dd, yyyy}",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }

        return result;
    }
}

public class LeaveAppStepByIdHandler : IRequestHandler<LeaveAppStepByIdQry, LeaveAppStepListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IHrmProfileClient _hrmProfile;

    public LeaveAppStepByIdHandler(IDapperHelper dapper, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _hrmProfile = hrmProfile;
    }

    public async Task<LeaveAppStepListDto?> Handle(LeaveAppStepByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveAppStep>(v, x => x.Id, x => x.StepName, x => x.StepOrder, x => x.Role, x => x.IsFinal, x => x.EmployeeId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<LeaveAppChain>(c, x => x.EffectiveFrom)
            .From<LeaveAppStep>(v)
            .Join<LeaveAppStep, LeaveAppChain>(v, c, x => x.LeaveAppChainId, x => x.Id)
            .Where<LeaveAppStep>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<LeaveAppStepListDto>(sql, parameters, ct);
        if (data == null) return null;

        var emp = "NOT ASSIGNED";
        if (data.EmployeeId != null)
        {
            var empR = await _hrmProfile.GetEmp(((Guid)data.EmployeeId).ToString(), ct);
            emp = empR.Res.Name;
        }


        data.RoleStr = MyEnumHelper.FormatEnum<ApprovalRole>(data.Role);
        data.IsFinalStr = BoolToStr.FormatBool(data.IsFinal);
        data.Employee = emp;
        data.LeaveAppChain = $"From : {data.EffectiveFrom:MMMM dd, yyyy}";
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}