// Recruit.App/Queries/OnboardingTaskQueries.cs
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class OnboardingTaskAllQry : IRequest<List<OnboardingTaskListDto>> { }

public class OnboardingTaskByIdQry : IRequest<OnboardingTaskListDto?>
{
    public Guid Id { get; set; }
}

// ----- Handlers -----

public class OnboardingTaskAllHandler : IRequestHandler<OnboardingTaskAllQry, List<OnboardingTaskListDto>>
{
    private readonly IDapperHelper _dapper;

    public OnboardingTaskAllHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<OnboardingTaskListDto>> Handle(OnboardingTaskAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<OnboardingTask>(v,
                x => x.Id,
                x => x.TaskName,
                x => x.Description,
                x => x.SequenceOrder,
                x => x.IsDeleted,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .From<OnboardingTask>(v)
            .Where<OnboardingTask>(v, x => x.IsDeleted == false)
            .OrderBy<OnboardingTask>(v, x => x.SequenceOrder);

        var (sql, parameters) = qb.Build();

        // ✅ Use a separate class for raw query results
        var rawResults = (await _dapper.QueryAsync<OnboardingTaskRawDto>(sql, parameters, ct)).ToList();

        // ✅ Map raw results to ListDto with proper RowVersion
        var list = rawResults.Select(raw => new OnboardingTaskListDto
        {
            Id = raw.Id,
            TaskName = raw.TaskName,
            Description = raw.Description,
            SequenceOrder = raw.SequenceOrder,
            IsDeleted = raw.IsDeleted,
            DateAdd = raw.DateAdd,
            DateAddAm = raw.DateAdd.ToString("dd/MM/yyyy HH:mm"),
            DateMod = raw.DateMod,
            DateModAm = raw.DateMod?.ToString("dd/MM/yyyy HH:mm") ?? "",
            RowVersion = raw.xmin.ToString() // ✅ Convert uint to string
        }).ToList();

        return list;
    }
}
public class OnboardingTaskByIdHandler : IRequestHandler<OnboardingTaskByIdQry, OnboardingTaskListDto?>
{
    private readonly IDapperHelper _dapper;

    public OnboardingTaskByIdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<OnboardingTaskListDto?> Handle(OnboardingTaskByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<OnboardingTask>(v,
                x => x.Id,
                x => x.TaskName,
                x => x.Description,
                x => x.SequenceOrder,
                x => x.IsDeleted,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .From<OnboardingTask>(v)
            .Where<OnboardingTask>(v, x => x.Id == request.Id && x.IsDeleted == false)
            .Limit(1);

        var (sql, parameters) = qb.Build();

        // ✅ Use raw DTO for mapping
        var raw = await _dapper.QueryFirstOrDefaultAsync<OnboardingTaskRawDto>(sql, parameters, ct);

        if (raw == null) return null;

        // ✅ Map to ListDto with RowVersion from xmin
        return new OnboardingTaskListDto
        {
            Id = raw.Id,
            TaskName = raw.TaskName,
            Description = raw.Description,
            SequenceOrder = raw.SequenceOrder,
            IsDeleted = raw.IsDeleted,
            DateAdd = raw.DateAdd,
            DateAddAm = raw.DateAdd.ToString("dd/MM/yyyy HH:mm"),
            DateMod = raw.DateMod,
            DateModAm = raw.DateMod?.ToString("dd/MM/yyyy HH:mm") ?? "",
            RowVersion = raw.xmin.ToString() // ✅ Convert uint to string
        };
    }
}