using MediatR;
using Module.App.Helpers;
using Module.App.Interfaces;
using Module.App.Queries;
using Module.Domain.DTOs;
using Module.Domain.Entities;

namespace Module.App.Commands;

public class AddBranchCmd : IRequest<BranchListDto> { public AddBranchDto AddBranchDto { get; set; } = default!; }

public class ModBranchCmd : IRequest<BranchListDto> { public EditBranchDto EditBranchDto { get; set; } = default!; }

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
                Name = request.AddBranchDto.Name,
                NameAm = request.AddBranchDto.NameAm,
                Code = code,
                Location = request.AddBranchDto.Location,
                BranchType = request.AddBranchDto.BranchType,
                BranchStat = "0",
                OpenDate = request.AddBranchDto.OpenDate,
                CompId = request.AddBranchDto.CompId
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
        var oldBra = await _unitOfWork.Repository<Branch>().GetById(request.EditBranchDto.Id);
        if (oldBra == null) { throw new KeyNotFoundException($"BRANCH with Id {request.EditBranchDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldBra.Name = request.EditBranchDto.Name;
            oldBra.NameAm = request.EditBranchDto.NameAm;
            oldBra.Location = request.EditBranchDto.Location;
            oldBra.BranchType = request.EditBranchDto.BranchType;
            oldBra.BranchStat = request.EditBranchDto.BranchStat;
            oldBra.OpenDate = request.EditBranchDto.OpenDate;
            oldBra.CompId = request.EditBranchDto.CompId;
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
