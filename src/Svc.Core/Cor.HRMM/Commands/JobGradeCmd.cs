using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Cor.HRMM.Services;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Events;
using System.Data;

namespace Cor.HRMM.Commands;

public class JobGradeAddCmd : IRequest<JobGradeListDto> { public JobGradeAddDto AddDto { get; set; } = default!; }
public class JobGradeModCmd : IRequest<JobGradeListDto> { public JobGradeModDto ModDto { get; set; } = default!; }
public class JobGradeDelCmd : IRequest { public Guid Id { get; set; } }

public class JobGradeAddHandler : IRequestHandler<JobGradeAddCmd, JobGradeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<JobGradeAddHandler> _logger;

    public JobGradeAddHandler(
        IUnitOfWork unitOfWork,
        IMediator med,
        IEventPublisher eventPublisher,
        ILogger<JobGradeAddHandler> logger)
    {
        _uow = unitOfWork;
        _med = med;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<JobGradeListDto> Handle(JobGradeAddCmd request, CancellationToken ct)
    {
        JobGrade data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = new JobGrade
            {
                Id = Guid.CreateVersion7(),
                Name = request.AddDto.Name,
                StartSalary = request.AddDto.StartSalary,
                MaxSalary = request.AddDto.MaxSalary,
                IsDeleted = false,
                DateAdd = DateTime.UtcNow
            };
            await _uow.AddAsync(data, token);
        }, ct: ct);

        // Publish event after successful commit
        await _eventPublisher.PublishAsync("JobGrade", "CREATED", new JobGradeEventData
        {
            Id = data.Id,
            Name = data.Name,
            StartSalary = data.StartSalary,
            MaxSalary = data.MaxSalary,
            IsDeleted = false
        }, ct);

        _logger.LogInformation("JobGrade created and event published: {JobGradeId}", data.Id);

        var response = await _med.Send(new JobGradeByIdQry { Id = data.Id }, ct);
        return response ?? new JobGradeListDto();
    }
}

public class JobGradeModHandler : IRequestHandler<JobGradeModCmd, JobGradeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<JobGradeModHandler> _logger;

    public JobGradeModHandler(
        IUnitOfWork unitOfWork,
        IMediator med,
        IEventPublisher eventPublisher,
        ILogger<JobGradeModHandler> logger)
    {
        _uow = unitOfWork;
        _med = med;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<JobGradeListDto> Handle(JobGradeModCmd request, CancellationToken ct)
    {
        JobGrade? oldData = null!;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<JobGrade>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException($"JOB GRADE with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.Name = request.ModDto.Name;
            oldData.StartSalary = request.ModDto.StartSalary;
            oldData.MaxSalary = request.ModDto.MaxSalary;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            oldData.DateMod = DateTime.UtcNow;
            _uow.Update(oldData);
        }, ct: ct);

        // Publish event after successful commit
        await _eventPublisher.PublishAsync("JobGrade", "UPDATED", new JobGradeEventData
        {
            Id = oldData!.Id,
            Name = oldData.Name,
            StartSalary = oldData.StartSalary,
            MaxSalary = oldData.MaxSalary,
            IsDeleted = oldData.IsDeleted
        }, ct);

        _logger.LogInformation("JobGrade updated and event published: {JobGradeId}", oldData.Id);

        var response = await _med.Send(new JobGradeByIdQry { Id = request.ModDto.Id }, ct);
        return response ?? new JobGradeListDto();
    }
}

public class JobGradeDelHandler : IRequestHandler<JobGradeDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<JobGradeDelHandler> _logger;

    public JobGradeDelHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        ILogger<JobGradeDelHandler> logger)
    {
        _uow = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task Handle(JobGradeDelCmd request, CancellationToken ct)
    {
        JobGrade? data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = await _uow.Set<JobGrade>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException($"JOB GRADE with id [{request.Id}] NOT FOUND.");
            }

            data.IsDeleted = true;
            data.DateMod = DateTime.UtcNow;
            _uow.Update(data);
        }, ct: ct);

        // Publish event after successful commit
        await _eventPublisher.PublishAsync("JobGrade", "DELETED", new JobGradeEventData
        {
            Id = data!.Id,
            Name = data.Name,
            StartSalary = data.StartSalary,
            MaxSalary = data.MaxSalary,
            IsDeleted = true
        }, ct);

        _logger.LogInformation("JobGrade deleted and event published: {JobGradeId}", data.Id);
    }
}