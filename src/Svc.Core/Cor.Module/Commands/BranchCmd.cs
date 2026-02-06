using Cor.Module.Helpers;
using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;

namespace Cor.Module.Commands;

public class AddBranchCmd : IRequest<BranchListDto> { public AddBranchDto AddDto { get; set; } = default!; }
public class ModBranchCmd : IRequest<BranchListDto> { public EditBranchDto ModDto { get; set; } = default!; }
public class DelBranchCmd : IRequest { public Guid Id { get; set; } }

public class AddBranchCmdHandler : IRequestHandler<AddBranchCmd, BranchListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddBranchCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<BranchListDto> Handle(AddBranchCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var code = await new BraCode(_unitOfWork).GetBraCode();
            var bra = new Branch
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                Code = code,
                Location = request.AddDto.Location,
                BranchType = request.AddDto.BranchType,
                BranchStat = "0",
                OpenDate = request.AddDto.OpenDate,
                CompId = request.AddDto.CompId
            };
            await _unitOfWork.Repository<Branch>().Add(bra);
            await _unitOfWork.Commit();

            var res = new BranchListDto();
            var response = await _med.Send(new BranchByIdQry { Id = bra.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class ModBranchCmdHandler : IRequestHandler<ModBranchCmd, BranchListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ModBranchCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<BranchListDto> Handle(ModBranchCmd request, CancellationToken cancellationToken)
    {
        var oldBra = await _unitOfWork.Repository<Branch>().GetById(request.ModDto.Id);
        if (oldBra == null) { throw new DomainException($"BRANCH with id [{request.ModDto.Id}] NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldBra.Name = request.ModDto.Name;
            oldBra.NameAm = request.ModDto.NameAm;
            oldBra.Location = request.ModDto.Location;
            oldBra.BranchType = request.ModDto.BranchType;
            oldBra.BranchStat = request.ModDto.BranchStat;
            oldBra.OpenDate = request.ModDto.OpenDate;
            oldBra.CompId = request.ModDto.CompId;
            var nBra = await _unitOfWork.Repository<Branch>().Update(oldBra);
            await _unitOfWork.Commit();

            var res = new BranchListDto();
            var response = await _med.Send(new BranchByIdQry { Id = nBra.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class DelBranchCmdHandler : IRequestHandler<DelBranchCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelBranchCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelBranchCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<Branch>().GetById(request.Id);
            if (data == null) { throw new DomainException($"BRANCH with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<Branch>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}