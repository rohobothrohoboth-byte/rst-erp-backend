using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpCertAllQry : IRequest<List<EmpFileList>> { public Guid Id { get; set; } }
public class EmpCertByIdQry : IRequest<EmpFileRes?> { public Guid Id { get; set; } }



public class EmpCertAll(IDapperHelper _dapper) : IRequestHandler<EmpCertAllQry, List<EmpFileList>>
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

public class EmpCertById(IDapperHelper _dapper) : IRequestHandler<EmpCertByIdQry, EmpFileRes?>
{
    private async Task<EmpFileDta?> GetFile(EmpFileList dto, CancellationToken ct)
    {
        var bcType = BoolToStr.EnumToString(CertType.Birth);
        if (dto.CertType == bcType)
        {
            const string v = "v";
            var qb = new QueryBuilder()
                .Select<EmpCertBirth>(v, x => x.Data)
                .From<EmpCertBirth>(v)
                .Where<EmpCertBirth>(v, x => x.EmpCertId == dto.Id)
                .Limit(1);
            var (sql, parameters) = qb.Build();
            var data = await _dapper.QueryFirstOrDefaultAsync<EmpFileDta>(sql, parameters, ct);
            return data ?? null;
        }
        else
        {
            const string v = "v";
            var qb = new QueryBuilder()
                .Select<EmpCertMarriage>(v, x => x.Data)
                .From<EmpCertMarriage>(v)
                .Where<EmpCertMarriage>(v, x => x.EmpCertId == dto.Id)
                .Limit(1);
            var (sql, parameters) = qb.Build();
            var data = await _dapper.QueryFirstOrDefaultAsync<EmpFileDta>(sql, parameters, ct);
            return data ?? null;
        }
    }

    public async Task<EmpFileRes?> Handle(EmpCertByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EmpCert>(v, x => x.Id, x => x.FileName, x => x.ContentType, x => x.CertType)
            .From<EmpCert>(v)
            .Where<EmpCert>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var cert = await _dapper.QueryFirstOrDefaultAsync<EmpFileList>(sql, parameters, ct);
        if (cert == null) return null;

        var data = await GetFile(cert, ct);
        if (data == null) { return null; }

        return new EmpFileRes
        {
            FileName = cert.FileName,
            ContentType = cert.ContentType,
            Data = data.Data
        };
    }
}