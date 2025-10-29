using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class AddressNameAllQry : IRequest<List<NameList>> { }
public class AddressNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }


public class AddressNameAllQryHandler : IRequestHandler<AddressNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddressNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(AddressNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Address>().GetAll();
        var dataL = new List<NameList>();

        foreach (var data in dbData)
        {
            var c = new NameList
            {
                Id = data.Id,
                Name = $"{((AddressType)Enum.Parse(typeof(AddressType), data.AddressType)).ToDisplayName()}: {data.Region} | {data.Zone}({data.Subcity}) | {data.Woreda} | {data.Kebele})"
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class AddressNameByIdQryHandler : IRequestHandler<AddressNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddressNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(AddressNameByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Address>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new NameList
        {
            Id = data.Id,
            Name = $"{((AddressType)Enum.Parse(typeof(AddressType), data.AddressType)).ToDisplayName()}: {data.Region} | {data.Zone}({data.Subcity}) | {data.Woreda} | {data.Kebele})"
        };
        return c;
    }
}