// Cor.CRM/Commands/CustomFieldCmd.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class CustomFieldDefinitionAddCmd : IRequest<CustomFieldDefinitionDto>
{
    public CreateCustomFieldDefinitionDto Dto { get; set; } = default!;
}

public class CustomFieldDefinitionModCmd : IRequest<CustomFieldDefinitionDto>
{
    public Guid Id { get; set; }
    public UpdateCustomFieldDefinitionDto Dto { get; set; } = default!;
}

public class CustomFieldDefinitionDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// ADD CUSTOM FIELD DEFINITION
// ============================================================

public class CustomFieldDefinitionAddHandler : IRequestHandler<CustomFieldDefinitionAddCmd, CustomFieldDefinitionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CustomFieldDefinitionAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CustomFieldDefinitionDto> Handle(CustomFieldDefinitionAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var definition = new CustomFieldDefinition
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                Label = request.Dto.Label ?? request.Dto.Name,
                Type = Enum.Parse<CustomFieldType>(request.Dto.Type),
                EntityType = Enum.Parse<CustomFieldEntity>(request.Dto.EntityType),
                IsRequired = request.Dto.IsRequired,
                DefaultValue = request.Dto.DefaultValue,
                OptionsJson = request.Dto.OptionsJson,
                ValidationRulesJson = request.Dto.ValidationRulesJson,
                DisplayOrder = request.Dto.DisplayOrder,
                Category = request.Dto.Category,
                HelpText = request.Dto.HelpText,
                IsSearchable = request.Dto.IsSearchable,
                IsFilterable = request.Dto.IsFilterable,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(definition, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Custom field definition created: {DefinitionId} - {DefinitionName}", definition.Id, definition.Name);

            return new CustomFieldDefinitionDto
            {
                Id = definition.Id,
                Name = definition.Name,
                Label = definition.Label,
                Type = definition.Type.ToString(),
                EntityType = definition.EntityType.ToString(),
                IsRequired = definition.IsRequired,
                IsActive = definition.IsActive,
                DefaultValue = definition.DefaultValue,
                OptionsJson = definition.OptionsJson,
                ValidationRulesJson = definition.ValidationRulesJson,
                DisplayOrder = definition.DisplayOrder,
                Category = definition.Category,
                HelpText = definition.HelpText,
                IsSearchable = definition.IsSearchable,
                IsFilterable = definition.IsFilterable,
                CreatedAt = definition.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// UPDATE CUSTOM FIELD DEFINITION
// ============================================================

public class CustomFieldDefinitionModHandler : IRequestHandler<CustomFieldDefinitionModCmd, CustomFieldDefinitionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CustomFieldDefinitionModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CustomFieldDefinitionDto> Handle(CustomFieldDefinitionModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var definition = await _uow.Set<CustomFieldDefinition>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (definition == null)
            {
                throw new DomainException($"Custom field definition with id [{request.Id}] NOT FOUND.");
            }

            if (request.Dto.Name != null)
                definition.Name = request.Dto.Name;
            if (request.Dto.Label != null)
                definition.Label = request.Dto.Label;
            if (request.Dto.Type != null)
                definition.Type = Enum.Parse<CustomFieldType>(request.Dto.Type);
            if (request.Dto.EntityType != null)
                definition.EntityType = Enum.Parse<CustomFieldEntity>(request.Dto.EntityType);
            if (request.Dto.IsRequired.HasValue)
                definition.IsRequired = request.Dto.IsRequired.Value;
            if (request.Dto.IsActive.HasValue)
                definition.IsActive = request.Dto.IsActive.Value;
            if (request.Dto.DefaultValue != null)
                definition.DefaultValue = request.Dto.DefaultValue;
            if (request.Dto.OptionsJson != null)
                definition.OptionsJson = request.Dto.OptionsJson;
            if (request.Dto.ValidationRulesJson != null)
                definition.ValidationRulesJson = request.Dto.ValidationRulesJson;
            if (request.Dto.DisplayOrder.HasValue)
                definition.DisplayOrder = request.Dto.DisplayOrder.Value;
            if (request.Dto.Category != null)
                definition.Category = request.Dto.Category;
            if (request.Dto.HelpText != null)
                definition.HelpText = request.Dto.HelpText;
            if (request.Dto.IsSearchable.HasValue)
                definition.IsSearchable = request.Dto.IsSearchable.Value;
            if (request.Dto.IsFilterable.HasValue)
                definition.IsFilterable = request.Dto.IsFilterable.Value;

            definition.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(definition);
            await _uow.Commit(ct);

            _logger.LogInformation("Custom field definition updated: {DefinitionId} - {DefinitionName}", definition.Id, definition.Name);

            return new CustomFieldDefinitionDto
            {
                Id = definition.Id,
                Name = definition.Name,
                Label = definition.Label,
                Type = definition.Type.ToString(),
                EntityType = definition.EntityType.ToString(),
                IsRequired = definition.IsRequired,
                IsActive = definition.IsActive,
                DefaultValue = definition.DefaultValue,
                OptionsJson = definition.OptionsJson,
                ValidationRulesJson = definition.ValidationRulesJson,
                DisplayOrder = definition.DisplayOrder,
                Category = definition.Category,
                HelpText = definition.HelpText,
                IsSearchable = definition.IsSearchable,
                IsFilterable = definition.IsFilterable,
                UpdatedAt = definition.UpdatedAt,
                CreatedAt = definition.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// DELETE CUSTOM FIELD DEFINITION
// ============================================================

public class CustomFieldDefinitionDelHandler : IRequestHandler<CustomFieldDefinitionDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CustomFieldDefinitionDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(CustomFieldDefinitionDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var definition = await _uow.Set<CustomFieldDefinition>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (definition == null)
            {
                throw new DomainException($"Custom field definition with id [{request.Id}] NOT FOUND.");
            }

            await _uow.Delete(definition);
            await _uow.Commit(ct);

            _logger.LogInformation("Custom field definition deleted: {DefinitionId} - {DefinitionName}", definition.Id, definition.Name);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}