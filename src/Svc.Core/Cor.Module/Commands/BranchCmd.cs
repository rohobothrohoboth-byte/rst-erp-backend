using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Module.Commands;

public class AddBranchCmd : IRequest<BranchListDto> { public AddBranchDto AddDto { get; set; } = default!; }
public class ModBranchCmd : IRequest<BranchListDto> { public EditBranchDto ModDto { get; set; } = default!; }
public class DelBranchCmd : IRequest { public Guid Id { get; set; } }



public class AddBranchCmdHandler : IRequestHandler<AddBranchCmd, BranchListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public AddBranchCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<BranchListDto> Handle(AddBranchCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var bra = new Branch
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                Location = request.AddDto.Location,
                BranchType = request.AddDto.BranchType,
                BranchStat = BoolToStr.EnumToString(BranchStat.Active),
                OpenDate = request.AddDto.OpenDate,
                CompId = request.AddDto.CompId
            };
            await _uow.Add(bra, ct);
            await _uow.Commit(ct);

            var res = new BranchListDto();
            var response = await _med.Send(new BranchByIdQry { Id = bra.Id }, ct);
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

public class ModBranchCmdHandler : IRequestHandler<ModBranchCmd, BranchListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public ModBranchCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<BranchListDto> Handle(ModBranchCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Branch>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"BRANCH with id [{request.ModDto.Id}] NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.NameAm = request.ModDto.NameAm;
            oldData.Location = request.ModDto.Location;
            oldData.BranchType = request.ModDto.BranchType;
            oldData.BranchStat = request.ModDto.BranchStat;
            oldData.OpenDate = request.ModDto.OpenDate;
            oldData.CompId = request.ModDto.CompId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new BranchListDto();
            var response = await _med.Send(new BranchByIdQry { Id = request.ModDto.Id }, ct);
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

public class DelBranchCmdHandler : IRequestHandler<DelBranchCmd>
{
    private readonly IUnitOfWork _uow;
    public DelBranchCmdHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(DelBranchCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Branch>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"BRANCH with id [{request.Id}] NOT FOUND."); }
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