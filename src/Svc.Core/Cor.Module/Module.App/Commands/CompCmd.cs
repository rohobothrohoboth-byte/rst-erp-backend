using MediatR;
using Module.App.Interfaces;
using Module.App.Queries;
using Module.Domain.DTOs;
using Module.Domain.Entities;

namespace Module.App.Commands;

public class AddCompCmd : IRequest<CompListDto> { public AddCompDto AddCompDto { get; set; } = default!; }

public class ModCompCmd : IRequest<CompListDto> { public EditCompDto EditCompDto { get; set; } = default!; }

public class DelCompCmd : IRequest { public Guid Id { get; set; } }

public class AddCompCmdHandler : IRequestHandler<AddCompCmd, CompListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddCompCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<CompListDto> Handle(AddCompCmd request, CancellationToken cancellationToken)
    {
        var com = new Company
        {
            Name = request.AddCompDto.Name,
            NameAm = request.AddCompDto.NameAm
        };

        await _unitOfWork.Begin();
        try
        {
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
        var oldComp = await _unitOfWork.Repository<Company>().GetById(request.EditCompDto.Id);

        if (oldComp == null)
        {
            throw new KeyNotFoundException($"COMPANY with Id {request.EditCompDto.Id} not found.");
        }

        oldComp.Name = request.EditCompDto.Name;
        oldComp.NameAm = request.EditCompDto.NameAm;

        await _unitOfWork.Begin();

        try
        {
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