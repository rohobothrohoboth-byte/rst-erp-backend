using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Helpers;
using MediatR;

namespace Cor.HRMM.Queries;

public class JgStepAllQry : IRequest<List<JgStepListDto>>
{
    // No parameters - get all
}

// ? Keep existing for getting by JobGradeId
public class JgStepByJobGradeIdQry : IRequest<List<JgStepListDto>>
{
    public Guid JobGradeId { get; set; }
}

public class JgStepByIdQry : IRequest<JgStepListDto?>
{
    public Guid Id { get; set; }
}



// JgStepAllHandler.cs - For ALL JgSteps (no filter)
public class JgStepAllHandler(IDapperHelper dapper) : IRequestHandler<JgStepAllQry, List<JgStepListDto>>
{
    public async Task<List<JgStepListDto>> Handle(JgStepAllQry request, CancellationToken ct)
    {
        const string jgs = "jgs";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<JgStep>(jgs, x => x.Id, x => x.Name, x => x.Salary, x => x.Currency, x => x.SalaryPayFreq, x => x.JobGradeId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<JobGrade, JgStepListDto>(jg, x => x.Name, d => d.JobGrade)
            .From<JgStep>(jgs)
            .Join<JgStep, JobGrade>(jgs, jg, x => x.JobGradeId, x => x.Id)
            .OrderBy<JgStep>(jgs, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<JgStepListDto>(ct);

        foreach (var data in list)
        {
            // Format display strings
            if (!string.IsNullOrEmpty(data.Currency))
            {
                var cur = MyEnumHelper.FormatEnum<Currency>(data.Currency);
                data.CurrencyStr = cur;
                data.SalaryStr = $"{data.Salary:#,##0.##} {cur}";
            }
            else
            {
                data.CurrencyStr = "ETB";
                data.SalaryStr = $"{data.Salary:#,##0.##} ETB";
            }

            if (!string.IsNullOrEmpty(data.SalaryPayFreq))
            {
                data.SalaryPayFreqStr = MyEnumHelper.FormatEnum<SalaryPayFreq>(data.SalaryPayFreq);
            }
            else
            {
                data.SalaryPayFreqStr = "Monthly";
            }

            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}
// JgStepByJobGradeIdHandler.cs
public class JgStepByJobGradeIdHandler(IDapperHelper dapper) : IRequestHandler<JgStepByJobGradeIdQry, List<JgStepListDto>>
{
    public async Task<List<JgStepListDto>> Handle(JgStepByJobGradeIdQry request, CancellationToken ct)
    {
        const string jgs = "jgs";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<JgStep>(jgs, x => x.Id, x => x.Name, x => x.Salary, x => x.Currency, x => x.SalaryPayFreq, x => x.JobGradeId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<JobGrade, JgStepListDto>(jg, x => x.Name, d => d.JobGrade)
            .From<JgStep>(jgs)
            .Join<JgStep, JobGrade>(jgs, jg, x => x.JobGradeId, x => x.Id)
            .Where<JgStep>(jgs, x => x.JobGradeId == request.JobGradeId)
            .OrderBy<JgStep>(jgs, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<JgStepListDto>(ct);

        foreach (var data in list)
        {
            if (!string.IsNullOrEmpty(data.Currency))
            {
                var cur = MyEnumHelper.FormatEnum<Currency>(data.Currency);
                data.CurrencyStr = cur;
                data.SalaryStr = $"{data.Salary:#,##0.##} {cur}";
            }
            else
            {
                data.CurrencyStr = "ETB";
                data.SalaryStr = $"{data.Salary:#,##0.##} ETB";
            }

            if (!string.IsNullOrEmpty(data.SalaryPayFreq))
            {
                data.SalaryPayFreqStr = MyEnumHelper.FormatEnum<SalaryPayFreq>(data.SalaryPayFreq);
            }
            else
            {
                data.SalaryPayFreqStr = "Monthly";
            }

            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JgStepByIdQryHandler(IDapperHelper dapper) : IRequestHandler<JgStepByIdQry, JgStepListDto?>
{
    public async Task<JgStepListDto?> Handle(JgStepByIdQry request, CancellationToken ct)
    {
        const string jgs = "jgs";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<JgStep>(jgs, x => x.Id, x => x.Name, x => x.Salary, x => x.Currency, x => x.SalaryPayFreq, x => x.JobGradeId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<JobGrade, JgStepListDto>(jg, x => x.Name, d => d.JobGrade)
            .From<JgStep>(jgs)
            .Join<JgStep, JobGrade>(jgs, jg, x => x.JobGradeId, x => x.Id)
            .Where<JgStep>(jgs, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<JgStepListDto>(sql, parameters, ct);
        if (data == null) return null;

        var cur = MyEnumHelper.FormatEnum<Currency>(data.Currency);
        data.SalaryStr = $"{data.Salary:#,##0.##} {cur}";
        data.CurrencyStr = cur;
        data.SalaryPayFreqStr = MyEnumHelper.FormatEnum<SalaryPayFreq>(data.SalaryPayFreq);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}
