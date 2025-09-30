using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using MediatR;

namespace Cor.HRMM.Commands;

public class AddressAddCmd : IRequest<AddressListDto> { public AddressAddDto AddDto { get; set; } = default!; }

public class AddressModCmd : IRequest<AddressListDto> { public AddressModDto ModDto { get; set; } = default!; }

public class AddressDelCmd : IRequest { public Guid Id { get; set; } }

public class AddressAddCmdHandler : IRequestHandler<AddressAddCmd, AddressListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddressAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<AddressListDto> Handle(AddressAddCmd request, CancellationToken cancellationToken)
    {
        var data = new Address
        {
            RegionId = request.AddDto.RegionId,
            AddressTypeId = request.AddDto.AddressTypeId,
            Country = request.AddDto.Country,
            Subcity = request.AddDto.Subcity,
            Zone = request.AddDto.Zone,
            Woreda = request.AddDto.Woreda,
            Kebele = request.AddDto.Kebele,
            HouseNo = request.AddDto.HouseNo,
            Telephone = request.AddDto.Telephone,
            PoBox = request.AddDto.PoBox,
            Fax = request.AddDto.Fax,
            Email = request.AddDto.Email,
            Website = request.AddDto.Website
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Address>().Add(data);
            await _unitOfWork.Commit();

            var res = new AddressListDto();
            var response = await _med.Send(new AddressByIdQry { Id = data.Id }, cancellationToken);
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

public class AddressModCmdHandler : IRequestHandler<AddressModCmd, AddressListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddressModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<AddressListDto> Handle(AddressModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<Address>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"ADDRESS with Id {request.ModDto.Id} NOT FOUND."); }

        oldData.RegionId = request.ModDto.RegionId;
        oldData.AddressTypeId = request.ModDto.AddressTypeId;
        oldData.Country = request.ModDto.Country;
        oldData.Zone = request.ModDto.Zone;
        oldData.Woreda = request.ModDto.Woreda;
        oldData.Kebele = request.ModDto.Kebele;
        oldData.HouseNo = request.ModDto.HouseNo;
        oldData.Telephone = request.ModDto.Telephone;
        oldData.PoBox = request.ModDto.PoBox;
        oldData.Fax = request.ModDto.Fax;
        oldData.Email = request.ModDto.Email;
        oldData.Website = request.ModDto.Website;
        await _unitOfWork.Begin();

        try
        {
            var data = await _unitOfWork.Repository<Address>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new AddressListDto();
            var response = await _med.Send(new AddressByIdQry { Id = data.Id }, cancellationToken);
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

public class AddressDelCmdHandler : IRequestHandler<AddressDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddressDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(AddressDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Address>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}