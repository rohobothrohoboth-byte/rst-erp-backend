using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Services;
using MediatR;

namespace Cor.HRMM.Queries;

public class AllAddressQry : IRequest<List<AddressListDto>> { }

public class AddressByIdQry : IRequest<AddressListDto?> { public Guid Id { get; set; } }

public class AllAddressQryHandler : IRequestHandler<AllAddressQry, List<AddressListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AllAddressQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<List<AddressListDto>> Handle(AllAddressQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Address>().GetAll();
        var dataL = new List<AddressListDto>();
        foreach (var data in dbData)
        {
            var region = await _lupClient.GetRegion(data.RegionId);
            var aType = await _lupClient.GetAddressType(data.AddressTypeId);
            var c = new AddressListDto
            {
                Id = data.Id,
                Region = region!.Name,
                AddressType = aType!.Name,
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
            dataL.Add(c);
        }

        return dataL;
    }
}

public class AddressByIdQryHandler : IRequestHandler<AddressByIdQry, AddressListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddressByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<AddressListDto?> Handle(AddressByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<Address>().GetById(request.Id);
        if (nData == null)
        {
            return null;
        }
        
        var c = new AddressListDto
        {
            Id = nData.Id,
            Region = "",
            AddressType = "",
            Country = nData.Country,
            Subcity = nData.Subcity,
            Zone = nData.Zone,
            Woreda = nData.Woreda,
            Kebele = nData.Kebele,
            HouseNo = nData.HouseNo,
            Telephone = nData.Telephone,
            PoBox = nData.PoBox,
            Fax = nData.Fax,
            Email = nData.Email,
            Website = nData.Website,
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };
        return c;
    }
}