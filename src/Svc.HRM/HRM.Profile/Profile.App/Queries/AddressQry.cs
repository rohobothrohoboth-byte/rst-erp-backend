using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class AddressAllQry : IRequest<List<AddressListDto>> { }
public class AddressByIdQry : IRequest<AddressListDto?> { public Guid Id { get; set; } }

public class AddressAllQryHandler : IRequestHandler<AddressAllQry, List<AddressListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddressAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<AddressListDto>> Handle(AddressAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Address>().GetAll();
        var dataL = new List<AddressListDto>();

        foreach (var data in dbData)
        {
            var c = new AddressListDto
            {
                Id = data.Id,
                AddressType = data.AddressType,
                AddressTypeStr = ((AddressType)Enum.Parse(typeof(AddressType), data.AddressType)).ToDisplayName(),
                Country = data.Country,
                Region = data.Region,
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
        if (nData == null) { return null; }
        
        var c = new AddressListDto
        {
            Id = nData.Id,
            AddressType = nData.AddressType,
            AddressTypeStr = ((AddressType)Enum.Parse(typeof(AddressType), nData.AddressType)).ToDisplayName(),
            Country = nData.Country,
            Region = nData.Region,
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