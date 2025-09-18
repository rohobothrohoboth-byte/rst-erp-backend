using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Commands;

public class AddAddressCmd : IRequest<AddressListDto> { public AddAddressDto AddAddressDto { get; set; } = default!; }

public class ModAddressCmd : IRequest<AddressListDto> { public EditAddressDto EditAddressDto { get; set; } = default!; }

public class DelAddressCmd : IRequest { public Guid Id { get; set; } }

public class AddAddressCmdHandler : IRequestHandler<AddAddressCmd, AddressListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddAddressCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<AddressListDto> Handle(AddAddressCmd request, CancellationToken cancellationToken)
    {
        var data = new Address
        {
            RegionId = request.AddAddressDto.RegionId,
            AddressTypeId = request.AddAddressDto.AddressTypeId,
            Country = request.AddAddressDto.Country,
            Subcity = request.AddAddressDto.Subcity,
            Zone = request.AddAddressDto.Zone,
            Woreda = request.AddAddressDto.Woreda,
            Kebele = request.AddAddressDto.Kebele,
            HouseNo = request.AddAddressDto.HouseNo,
            Telephone = request.AddAddressDto.Telephone,
            PoBox = request.AddAddressDto.PoBox,
            Fax = request.AddAddressDto.Fax,
            Email = request.AddAddressDto.Email,
            Website = request.AddAddressDto.Website
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Address>().Add(data);
            await _unitOfWork.Commit();

            var nData = await _unitOfWork.Repository<Address>().GetById(data.Id);
            var res = new AddressListDto();
            if (nData == null) return res;

            res.Id = nData.Id;
            res.Region = "";
            res.AddressType = "";
            res.Country = nData.Country;
            res.Subcity = nData.Subcity;
            res.Zone = nData.Zone;
            res.Woreda = nData.Woreda;
            res.Kebele = nData.Kebele;
            res.HouseNo = nData.HouseNo;
            res.Telephone = nData.Telephone;
            res.PoBox = nData.PoBox;
            res.Fax = nData.Fax;
            res.Email = nData.Email;
            res.Website = nData.Website;
            res.IsDeleted = nData.IsDeleted;
            res.DateAdd = nData.DateAdd;
            res.DateMod = nData.DateMod;
            res.RowVersion = Convert.ToBase64String(nData.RowVersion);
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class ModAddressCmdHandler : IRequestHandler<ModAddressCmd, AddressListDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ModAddressCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<AddressListDto> Handle(ModAddressCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<Address>().GetById(request.EditAddressDto.Id);

        if (oldData == null)
        {
            throw new KeyNotFoundException($"ADDRESS with Id {request.EditAddressDto.Id} not found.");
        }

        oldData.RegionId = request.EditAddressDto.RegionId;
        oldData.AddressTypeId = request.EditAddressDto.AddressTypeId;
        oldData.Country = request.EditAddressDto.Country;
        oldData.Zone = request.EditAddressDto.Zone;
        oldData.Woreda = request.EditAddressDto.Woreda;
        oldData.Kebele = request.EditAddressDto.Kebele;
        oldData.HouseNo = request.EditAddressDto.HouseNo;
        oldData.Telephone = request.EditAddressDto.Telephone;
        oldData.PoBox = request.EditAddressDto.PoBox;
        oldData.Fax = request.EditAddressDto.Fax;
        oldData.Email = request.EditAddressDto.Email;
        oldData.Website = request.EditAddressDto.Website;

        await _unitOfWork.Begin();

        try
        {
            var data = await _unitOfWork.Repository<Address>().Update(oldData);
            await _unitOfWork.Commit();

            var result = new AddressListDto
            {
                Id = data.Id,
                Region = "",
                AddressType = "",
                Country = data.Country,
                Subcity = data.Subcity,
                Zone = data.Zone,
                Woreda = data.Woreda,
                Kebele = data.Kebele,
                HouseNo = data.HouseNo,
                Telephone = data.Telephone,
                PoBox = data.PoBox,
                Fax = data.Fax,
                Email = data.Email,
                Website = data.Website,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion),
            };

            return result;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class DelAddressCmdHandler : IRequestHandler<DelAddressCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelAddressCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelAddressCmd request, CancellationToken cancellationToken)
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