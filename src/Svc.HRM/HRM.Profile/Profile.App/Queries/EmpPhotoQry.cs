using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpPhotoQry : IRequest<EmpPhotoRes?> { public Guid Id { get; set; } }
public class EmpPhotoThumbnailQry : IRequest<EmpPhotoRes?> { public Guid Id { get; set; } }



public class EmpPhotoHandler : IRequestHandler<EmpPhotoQry, EmpPhotoRes?>
{
    private readonly IDapperHelper _dapper;
    public EmpPhotoHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EmpPhotoRes?> Handle(EmpPhotoQry request, CancellationToken ct)
    {
        const string ph = "ph";
        const string pb = "pb";
        const string th = "th";
        var qb = new QueryBuilder()
            .Select<EmpPhoto>(ph, x => x.Id)
            .Select<FileMetaData>(th, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .SelectAs<EmpPhotoBlob, EmpPhotoRes>(pb, x => x.Data, x => x.PhotoBinary!)
            .From<EmpPhoto>(ph)
            .Join<EmpPhoto, EmpPhotoBlob>(ph, pb, x => x.FileMetaDataId, x => x.FileMetaDataId)
            .LeftJoin<EmpPhoto, FileMetaData>(ph, th, x => x.FileMetaDataId, x => x.Id)
            .Where<EmpPhoto>(ph, x => x.EmployeeId == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpPhotoRes>(sql, parameters, ct);
        if (data == null) return null;

        data.Photo = data.PhotoBinary != null ? Convert.ToBase64String(data.PhotoBinary) : "";
        data.PhotoSize = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "";
        return data;
    }
}

public class EmpPhotoThumbnailHandler : IRequestHandler<EmpPhotoThumbnailQry, EmpPhotoRes?>
{
    private readonly IDapperHelper _dapper;
    public EmpPhotoThumbnailHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EmpPhotoRes?> Handle(EmpPhotoThumbnailQry request, CancellationToken ct)
    {
        const string ph = "ph";
        const string pt = "pt";
        const string th = "th";
        var qb = new QueryBuilder()
            .Select<EmpPhoto>(ph, x => x.Id)
            .Select<FileMetaData>(th, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .SelectAs<EmpPhotoThumbnail, EmpPhotoRes>(pt, x => x.Data, x => x.PhotoBinary!)
            .From<EmpPhoto>(ph)
            .Join<EmpPhoto, EmpPhotoThumbnail>(ph, pt, x => x.ThumbnailId, x => x.FileMetaDataId)
            .LeftJoin<EmpPhoto, FileMetaData>(ph, th, x => x.FileMetaDataId, x => x.Id)
            .Where<EmpPhoto>(ph, x => x.EmployeeId == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpPhotoRes>(sql, parameters, ct);
        if (data == null) return null;

        data.Photo = data.PhotoBinary != null ? Convert.ToBase64String(data.PhotoBinary) : "";
        data.PhotoSize = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "";
        return data;
    }
}