using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmContactAllQry : IRequest<List<EmContactListDto>> { public Guid Id { get; set; } }
public class EmContactByIdQry : IRequest<EmContactListDto?> { public Guid Id { get; set; } }

public class EmContactAllQryHandler : IRequestHandler<EmContactAllQry, List<EmContactListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;
    private readonly ILupClient _gRPC;

    public EmContactAllQryHandler(IUnitOfWork unitOfWork, ILupClient gRPC, IMediator med)
    {
        _unitOfWork = unitOfWork;
        _med = med;
        _gRPC = gRPC;
    }

    public async Task<List<EmContactListDto>> Handle(EmContactAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmergencyContact>().Find(e => e.EmployeeId == request.Id);
        var dataL = new List<EmContactListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var rel = await _gRPC.GetRelList(cancellationToken);

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var reV = rel.Res.FirstOrDefault(r => r.Id == data.RelationId.ToString());
            var re = "NOT AVAILABLE";
            if (reV.Id != null)
            {
                re = reV.Name;
            }
            var add = await _med.Send(new AddressNameByIdQry { Id = data.AddressId }, cancellationToken);
            var c = new EmContactListDto
            {
                Id = data.Id,
                PersonId = data.PersonId,
                AddressId = data.AddressId,
                RelationId = data.RelationId,
                EmployeeId = data.EmployeeId,
                Gender = per!.Gender,
                Nationality = per.Nationality,
                ContactName = $"{per.FirstName} {per.MiddleName} {per.LastName}",
                ContactNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
                GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
                Address = add != null ? add.Name : "NOT AVAILABLE",
                Relation = re,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class EmContactByIdQryHandler : IRequestHandler<EmContactByIdQry, EmContactListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _gRPC;
    private readonly IMediator _med;

    public EmContactByIdQryHandler(IUnitOfWork unitOfWork, ILupClient gRPC, IMediator med)
    {
        _unitOfWork = unitOfWork;
        _gRPC = gRPC;
        _med = med;
    }

    public async Task<EmContactListDto?> Handle(EmContactByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmergencyContact>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var add = await _med.Send(new AddressNameByIdQry { Id = data.AddressId }, cancellationToken);
        var re = await _gRPC.GetRel(data.RelationId.ToString(), cancellationToken);

        var c = new EmContactListDto
        {
            Id = data.Id,
            PersonId = data.PersonId,
            AddressId = data.AddressId,
            RelationId = data.RelationId,
            EmployeeId = data.EmployeeId,
            Gender = per!.Gender,
            Nationality = per.Nationality,
            ContactName = $"{per.FirstName} {per.MiddleName} {per.LastName}",
            ContactNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
            GenderStr = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Address = add != null ? add.Name : "NOT AVAILABLE",
            Relation = re.Res.Name != null ? re.Res.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}