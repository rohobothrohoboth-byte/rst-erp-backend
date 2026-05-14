using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpCertAllQry : IRequest<List<EmpFileList>> { public Guid Id { get; set; } }
public class EmpCertByIdQry : IRequest<EmpFileList?> { public Guid Id { get; set; } }



public class EmpCertAllHandler(IDapperHelper _dapper) : IRequestHandler<EmpCertAllQry, List<EmpFileList>>
{
    public async Task<List<EmpFileList>> Handle(EmpCertAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EmpCert>(v, x => x.Id, x => x.FileName, x => x.ContentType, x => x.FileSize, x => x.CertType)
            .From<EmpCert>(v)
            .Where<EmpCert>(v, x => x.EmployeeId == request.Id)
            .OrderBy<EmpCert>(v, x => x.DateAdd, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpFileList>(ct);

        foreach (var data in list)
        {
            data.CertType = MyEnumHelper.FormatEnum<CertType>(data.CertType);
            data.Size = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "0";
        }

        return list;
    }
}

public class EmpCertByIdHandler(IDapperHelper _dapper) : IRequestHandler<EmpCertByIdQry, EmpFileList?>
{
    public async Task<EmpFileList?> Handle(EmpCertByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EmpCert>(v, x => x.Id, x => x.FileName, x => x.ContentType, x => x.FileSize, x => x.CertType)
            .From<EmpCert>(v)
            .Where<EmpCert>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpFileList>(sql, parameters, ct);
        if (data == null) return null;

        data.CertType = MyEnumHelper.FormatEnum<CertType>(data.CertType);
        data.Size = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "0";
        return data;
    }
}