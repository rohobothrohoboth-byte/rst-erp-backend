using Cor.Module.Helpers;
using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using MediatR;

namespace Cor.Module.Commands;

public class AddCompCmd : IRequest<CompListDto> { public AddCompDto AddDto { get; set; } = default!; }
public class ModCompCmd : IRequest<CompListDto> { public EditCompDto ModDto { get; set; } = default!; }
public class DelCompCmd : IRequest { public Guid Id { get; set; } }

public class AddCompCmdHandler : IRequestHandler<AddCompCmd, CompListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddCompCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<CompListDto> Handle(AddCompCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var com = new Company
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm
            };
            await _unitOfWork.Repository<Company>().Add(com);
            await _unitOfWork.Commit();

            var res = new CompListDto();
            var response = await _med.Send(new CompByIdQry { Id = com.Id }, cancellationToken);
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

public class ModCompCmdHandler : IRequestHandler<ModCompCmd, CompListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ModCompCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<CompListDto> Handle(ModCompCmd request, CancellationToken cancellationToken)
    {
        var oldComp = await _unitOfWork.Repository<Company>().GetById(request.ModDto.Id);
        if (oldComp == null) { throw new DomainException($"COMPANY with id [{request.ModDto.Id}] NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldComp.Name = request.ModDto.Name;
            oldComp.NameAm = request.ModDto.NameAm;
            var comp = await _unitOfWork.Repository<Company>().Update(oldComp);
            await _unitOfWork.Commit();

            var res = new CompListDto();
            var response = await _med.Send(new CompByIdQry { Id = comp.Id }, cancellationToken);
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

public class DelCompCmdHandler : IRequestHandler<DelCompCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelCompCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelCompCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var bra = await _unitOfWork.Repository<Branch>().Find(b => b.CompId == request.Id);
            if (bra.Any()) { throw new DomainException($"COMPANY with id [{request.Id}] Has branches, can not be deleted."); }

            var data = await _unitOfWork.Repository<Company>().GetById(request.Id);
            if (data == null) { throw new DomainException($"COMPANY with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<Company>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}