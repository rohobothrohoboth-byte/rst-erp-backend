using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmContactAllQry : IRequest<List<EmContactListDto>> { public Guid Id { get; set; } }
public class EmContactByIdQry : IRequest<EmContactListDto?> { public Guid Id { get; set; } }

public class EmContactAllQryHandler : IRequestHandler<EmContactAllQry, List<EmContactListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILup _lup;
    private readonly IMediator _med;

    public EmContactAllQryHandler(IUnitOfWork unitOfWork, ILup lup, IMediator med)
    {
        _unitOfWork = unitOfWork;
        _lup = lup; 
        _med = med; 
    }

    public async Task<List<EmContactListDto>> Handle(EmContactAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EmergencyContact>().Find(e => e.EmployeeId == request.Id);
        var dataL = new List<EmContactListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var reL = await _lup.RelationList(cancellationToken);

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var re = reL!.FirstOrDefault(t => t.Id == data.RelationId);
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
                Relation = re != null ? re.Name : "NOT AVAILABLE",
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
    private readonly ILup _lup;
    private readonly IMediator _med;

    public EmContactByIdQryHandler(IUnitOfWork unitOfWork, ILup lup, IMediator med)
    {
        _unitOfWork = unitOfWork;
        _lup = lup;
        _med = med;
    }

    public async Task<EmContactListDto?> Handle(EmContactByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EmergencyContact>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var add = await _med.Send(new AddressNameByIdQry { Id = data.AddressId }, cancellationToken);
        var re = await _lup.Relation(data.RelationId, cancellationToken);

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
            Relation = re != null ? re.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}