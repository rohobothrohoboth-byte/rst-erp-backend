using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Module.Commands;

public class AddCompCmd : IRequest<CompListDto> { public AddCompDto AddDto { get; set; } = default!; }
public class ModCompCmd : IRequest<CompListDto> { public EditCompDto ModDto { get; set; } = default!; }
public class DelCompCmd : IRequest { public Guid Id { get; set; } }



public class AddCompCmdHandler : IRequestHandler<AddCompCmd, CompListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public AddCompCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<CompListDto> Handle(AddCompCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var com = new Company
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm
            };
            await _uow.Add(com, ct);
            await _uow.Commit(ct);

            var res = new CompListDto();
            var response = await _med.Send(new CompByIdQry { Id = com.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class ModCompCmdHandler : IRequestHandler<ModCompCmd, CompListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public ModCompCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<CompListDto> Handle(ModCompCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Company>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"COMPANY with id [{request.ModDto.Id}] NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.NameAm = request.ModDto.NameAm;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new CompListDto();
            var response = await _med.Send(new CompByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class DelCompCmdHandler : IRequestHandler<DelCompCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly IDapperHelper _dapper;
    public DelCompCmdHandler(IUnitOfWork unitOfWork, IDapperHelper dapper)
    {
        _uow = unitOfWork;
        _dapper = dapper;
    }

    public async Task Handle(DelCompCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            const string v = "v";
            var qb = new QueryBuilder().SelectDto<Branch, NameList>(v).From<Branch>(v).Where<Branch>(v, x => x.CompId == request.Id);
            var (sql, parameters) = qb.Build();
            await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
            var bra = await reader.ToListAsync<NameList>(ct);
            if (bra.Count != 0) { throw new DomainException($"COMPANY with id [{request.Id}] Has branches, can not be deleted."); }

            var data = await _uow.Set<Company>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"COMPANY with id [{request.Id}] NOT FOUND."); }
            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}