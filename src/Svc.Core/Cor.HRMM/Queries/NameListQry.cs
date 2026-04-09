using Common;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using MediatR;

namespace Cor.HRMM.Queries;

public class BenSetNameAllQry : IRequest<List<NameList>> { }
public class BenSetNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class EduQualNameAllQry : IRequest<List<NameList>> { }
public class EduQualNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class JgStepNameAllQry : IRequest<List<NameList>> { }
public class JgStepNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class JobGradeNameAllQry : IRequest<List<NameList>> { }
public class JobGradeNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class PosByDeptQry : IRequest<List<NameList>> { public Guid Id { get; set; } }
public class PosNameAllQry : IRequest<List<NameList>> { }
public class PosNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class SalaryQry : IRequest<string?> { public Guid Id { get; set; } }




public class BenSetNameAllHandler : IRequestHandler<BenSetNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public BenSetNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(BenSetNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<BenefitSetting>(v, x => x.Id, x => x.Name).From<BenefitSetting>(v);
        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<NameList>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = data.Name
            });
        }
        return dataL;
    }
}

public class BenSetNameByIdHandler : IRequestHandler<BenSetNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public BenSetNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(BenSetNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<BenefitSetting>(v, x => x.Id, x => x.Name).From<BenefitSetting>(v).Where<BenefitSetting>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;

        return new NameList
        {
            Id = data.Id,
            Name = data.Name
        };
    }
}

public class EduQualNameAllHandler : IRequestHandler<EduQualNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public EduQualNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(EduQualNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<EducationQual>(v, x => x.Id, x => x.Name).From<EducationQual>(v);

        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<NameList>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = data.Name
            });
        }
        return dataL;
    }
}

public class EduQualNameByIdHandler : IRequestHandler<EduQualNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public EduQualNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(EduQualNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<EducationQual>(v, x => x.Id, x => x.Name).From<EducationQual>(v).Where<EducationQual>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;

        return new NameList
        {
            Id = data.Id,
            Name = data.Name
        };
    }
}

public class JgStepNameAllHandler : IRequestHandler<JgStepNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public JgStepNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(JgStepNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<JgStep>(v, x => x.Id, x => x.Name)
            .SelectAs<JobGrade, NameAmList>(jg, x => x.Name, x => x.NameAm)
            .From<JgStep>(v)
            .Join<JgStep, JobGrade>(v, jg, x => x.JobGradeId, x => x.Id);

        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<NameAmList>();
        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = $"{data.Name} => {data.NameAm}"
            });
        }
        return dataL;
    }
}

public class JgStepNameByIdHandler : IRequestHandler<JgStepNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public JgStepNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(JgStepNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<JgStep>(v, x => x.Id, x => x.Name)
            .SelectAs<JobGrade, NameAmList>(jg, x => x.Name, x => x.NameAm)
            .From<JgStep>(v)
            .Join<JgStep, JobGrade>(v, jg, x => x.JobGradeId, x => x.Id)
            .Where<JgStep>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameAmList>(sql, parameters, ct);
        if (data == null) return null;

        return new NameList
        {
            Id = data.Id,
            Name = $"{data.Name} => {data.NameAm}"
        };
    }
}

public class JobGradeNameAllHandler : IRequestHandler<JobGradeNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public JobGradeNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(JobGradeNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<JobGrade>(v, x => x.Id, x => x.Name)
            .From<JobGrade>(v)
            .OrderBy<JobGrade>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<NameList>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = data.Name
            });
        }
        return dataL;
    }
}

public class JobGradeNameByIdHandler : IRequestHandler<JobGradeNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public JobGradeNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(JobGradeNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<JobGrade>(v, x => x.Id, x => x.Name).From<JobGrade>(v).Where<JobGrade>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;

        return new NameList
        {
            Id = data.Id,
            Name = data.Name
        };
    }
}

public class PosByDeptHandler : IRequestHandler<PosByDeptQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public PosByDeptHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(PosByDeptQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<Position>(v, x => x.Id, x => x.Name)
            .From<Position>(v)
            .Where<Position>(v, x => x.DepartmentId == request.Id);

        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<NameList>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = data.Name
            });
        }
        return dataL;
    }
}

public class PosNameAllHandler : IRequestHandler<PosNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public PosNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(PosNameAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<Position>(v, x => x.Id, x => x.Name).From<Position>(v);
        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<NameList>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = data.Name
            });
        }
        return dataL;
    }
}

public class PosNameByIdHandler : IRequestHandler<PosNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public PosNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(PosNameByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder().Select<Position>(v, x => x.Id, x => x.Name).From<Position>(v).Where<Position>(v, x => x.Id == request.Id).Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<NameList>(sql, parameters, ct);
        if (data == null) return null;

        return new NameList
        {
            Id = data.Id,
            Name = data.Name
        };
    }
}

public class SalaryHandler : IRequestHandler<SalaryQry, string?>
{
    private readonly IDapperHelper _dapper;
    public SalaryHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<string?> Handle(SalaryQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .SelectAs<JgStep, DoubleList>(v, x => x.Salary, x => x.Name)
            .From<JgStep>(v)
            .Where<JgStep>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<DoubleList>(sql, parameters, ct);
        if (data == null) return null;

        return $"{data.Name:#,##0.##} ETB";
    }
}



public class ValUserQry : IRequest<string?> { public string Token { get; set; } = default!; }

public class ValUserQryHandler : IRequestHandler<ValUserQry, string?>
{
    private readonly IAuthClient _authClient;
    public ValUserQryHandler(IAuthClient authClient) { _authClient = authClient; }

    public async Task<string?> Handle(ValUserQry request, CancellationToken cancellationToken)
    {
        var c = await _authClient.GetUser(request.Token, cancellationToken);
        return c.Username;
    }
}