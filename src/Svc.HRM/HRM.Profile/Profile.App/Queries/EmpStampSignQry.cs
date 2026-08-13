using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

// Retrieve an employee's official stamp / signature image (by employee id).
public class EmpStampQry : IRequest<EmpImageRes?> { public Guid Id { get; set; } }
public class EmpSignQry : IRequest<EmpImageRes?> { public Guid Id { get; set; } }

public class EmpStampHandler : IRequestHandler<EmpStampQry, EmpImageRes?>
{
    private readonly IDapperHelper _dapper;
    public EmpStampHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EmpImageRes?> Handle(EmpStampQry request, CancellationToken ct)
    {
        const string es = "es";
        const string sb = "sb";
        const string fm = "fm";
        var qb = new QueryBuilder()
            .Select<EmpStamp>(es, x => x.Id)
            .Select<FileMetaData>(fm, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .SelectAs<EmpStampBlob, EmpImageRes>(sb, x => x.Data, x => x.ImageBinary!)
            .From<EmpStamp>(es)
            .Join<EmpStamp, EmpStampBlob>(es, sb, x => x.FileMetaDataId, x => x.FileMetaDataId)
            .LeftJoin<EmpStamp, FileMetaData>(es, fm, x => x.FileMetaDataId, x => x.Id)
            .Where<EmpStamp>(es, x => x.EmployeeId == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpImageRes>(sql, parameters, ct);
        if (data == null) return null;

        data.Image = data.ImageBinary != null ? Convert.ToBase64String(data.ImageBinary) : "";
        data.Size = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "";
        return data;
    }
}

public class EmpSignHandler : IRequestHandler<EmpSignQry, EmpImageRes?>
{
    private readonly IDapperHelper _dapper;
    public EmpSignHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EmpImageRes?> Handle(EmpSignQry request, CancellationToken ct)
    {
        const string es = "es";
        const string sb = "sb";
        const string fm = "fm";
        var qb = new QueryBuilder()
            .Select<EmpSign>(es, x => x.Id)
            .Select<FileMetaData>(fm, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .SelectAs<EmpSignBlob, EmpImageRes>(sb, x => x.Data, x => x.ImageBinary!)
            .From<EmpSign>(es)
            .Join<EmpSign, EmpSignBlob>(es, sb, x => x.FileMetaDataId, x => x.FileMetaDataId)
            .LeftJoin<EmpSign, FileMetaData>(es, fm, x => x.FileMetaDataId, x => x.Id)
            .Where<EmpSign>(es, x => x.EmployeeId == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpImageRes>(sql, parameters, ct);
        if (data == null) return null;

        data.Image = data.ImageBinary != null ? Convert.ToBase64String(data.ImageBinary) : "";
        data.Size = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "";
        return data;
    }
}
