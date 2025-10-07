using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Services;
using MediatR;

namespace Cor.HRMM.Queries;

public class AddressAllQry : IRequest<List<AddressListDto>> { }
public class AddressNameAllQry : IRequest<List<NameList>> { }
public class AddressByIdQry : IRequest<AddressListDto?> { public Guid Id { get; set; } }
public class AddressNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }

public class AddressAllQryHandler : IRequestHandler<AddressAllQry, List<AddressListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AddressAllQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<List<AddressListDto>> Handle(AddressAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Address>().GetAll();
        var dataL = new List<AddressListDto>();
        var regionL = await _lupClient.RegionList(cancellationToken);
        var aTypeL = await _lupClient.AddressTypeList(cancellationToken);

        foreach (var data in dbData)
        {
            var region = regionL!.FirstOrDefault(t => t.Id == data.RegionId);
            var aType = aTypeL!.FirstOrDefault(t => t.Id == data.AddressTypeId);
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

public class AddressNameAllQryHandler : IRequestHandler<AddressNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AddressNameAllQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<List<NameList>> Handle(AddressNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Address>().GetAll();
        var dataL = new List<NameList>();
        var regionL = await _lupClient.RegionList(cancellationToken);
        var aTypeL = await _lupClient.AddressTypeList(cancellationToken);

        foreach (var data in dbData)
        {
            var region = regionL!.FirstOrDefault(t => t.Id == data.RegionId);
            var aType = aTypeL!.FirstOrDefault(t => t.Id == data.AddressTypeId);
            var c = new NameList
            {
                Id = data.Id,
                Name = $"{aType!.Name}: {region!.Name} | {data.Zone}({data.Subcity} | {data.Woreda} | {data.Kebele})"
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class AddressByIdQryHandler : IRequestHandler<AddressByIdQry, AddressListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AddressByIdQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<AddressListDto?> Handle(AddressByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<Address>().GetById(request.Id);
        if (nData == null) { return null; }
        var region = await _lupClient.Region(nData.RegionId, cancellationToken);
        var aType = await _lupClient.AddressType(nData.AddressTypeId, cancellationToken);
        
        var c = new AddressListDto
        {
            Id = nData.Id,
            Region = region!.Name,
            AddressType = aType!.Name,
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

public class AddressNameByIdQryHandler : IRequestHandler<AddressNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AddressNameByIdQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<NameList?> Handle(AddressNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<Address>().GetById(request.Id);
        if (nData == null) { return null; }
        var region = await _lupClient.Region(nData.RegionId, cancellationToken);
        var aType = await _lupClient.AddressType(nData.AddressTypeId, cancellationToken);

        var c = new NameList
        {
            Id = nData.Id,
            Name = $"{aType!.Name}: {region!.Name} | {nData.Zone}({nData.Subcity} | {nData.Woreda} | {nData.Kebele})"
        };
        return c;
    }
}