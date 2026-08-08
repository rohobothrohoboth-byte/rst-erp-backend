using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.HRMM.Commands;

public class BenefitSetAddCmd : IRequest<BenefitSetListDto> { public BenefitSetAddDto AddDto { get; set; } = default!; }
public class BenefitSetModCmd : IRequest<BenefitSetListDto> { public BenefitSetModDto ModDto { get; set; } = default!; }
public class BenefitSetDelCmd : IRequest { public Guid Id { get; set; } }

public class BenefitSetAddHandler : IRequestHandler<BenefitSetAddCmd, BenefitSetListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<BenefitSetAddHandler> _logger;

    public BenefitSetAddHandler(
        IUnitOfWork unitOfWork,
        ILogger<BenefitSetAddHandler> logger)
    {
        _uow = unitOfWork;
        _logger = logger;
    }

   public async Task<BenefitSetListDto> Handle(BenefitSetAddCmd request, CancellationToken ct)
   {
       BenefitSetting data = null!;

       await _uow.ExecuteAsync(async token =>
       {
           data = new BenefitSetting
           {
               Name = request.AddDto.Name,
               BenefitValue = request.AddDto.BenefitValue,
               Per = request.AddDto.Per
           };

           await _uow.AddAsync(data, token);
       }, ct: ct);

       return new BenefitSetListDto
       {
           Id = data.Id,
           Name = data.Name,
           BenefitValue = data.BenefitValue,
           Per = data.Per,
           DateAdd = data.DateAdd,
           DateMod = data.DateMod,
           IsDeleted = data.IsDeleted,
           RowVersion = data.xmin.ToString()
       };
   }}

// Similarly fix ModHandler to return the updated entity directly
public class BenefitSetModHandler : IRequestHandler<BenefitSetModCmd, BenefitSetListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<BenefitSetModHandler> _logger;

    public BenefitSetModHandler(
        IUnitOfWork unitOfWork,
        ILogger<BenefitSetModHandler> logger)
    {
        _uow = unitOfWork;
        _logger = logger;
    }

    public async Task<BenefitSetListDto> Handle(BenefitSetModCmd request, CancellationToken ct)
    {
        _logger.LogInformation("Updating BenefitSet: {Id}", request.ModDto.Id);

        BenefitSetting? oldData = null;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<BenefitSetting>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException(
                    $"BENEFIT SETTING with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.Name = request.ModDto.Name;
            oldData.BenefitValue = request.ModDto.BenefitValue;
            oldData.Per = request.ModDto.Per;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

            _uow.Update(oldData);
        }, ct: ct);

        return new BenefitSetListDto
        {
            Id = oldData!.Id,
            Name = oldData.Name,
            BenefitValue = oldData.BenefitValue,
            Per = oldData.Per,
            DateAdd = oldData.DateAdd,
            DateMod = oldData.DateMod,
            IsDeleted = oldData.IsDeleted,
            RowVersion = oldData.xmin.ToString()
        };
    }
}

public class BenefitSetDelHandler : IRequestHandler<BenefitSetDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<BenefitSetDelHandler> _logger;

    public BenefitSetDelHandler(
        IUnitOfWork unitOfWork,
        ILogger<BenefitSetDelHandler> logger)
    {
        _uow = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(BenefitSetDelCmd request, CancellationToken ct)
    {
        _logger.LogInformation("Deleting BenefitSet: {Id}", request.Id);

        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<BenefitSetting>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException(
                    $"BENEFIT SETTING with Id {request.Id} NOT FOUND.");
            }

            _uow.Delete(data);
        }, ct: ct);
    }
}
