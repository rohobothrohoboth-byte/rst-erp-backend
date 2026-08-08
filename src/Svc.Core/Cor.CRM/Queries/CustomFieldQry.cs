// Cor.CRM/Queries/CustomFieldQry.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;
namespace Cor.CRM.Queries;

// ============================================================
// QUERIES
// ============================================================

public class CustomFieldDefinitionsQry : IRequest<List<CustomFieldDefinitionDto>>
{
    public string? EntityType { get; set; }
}

public class CustomFieldDefinitionByIdQry : IRequest<CustomFieldDefinitionDto?>
{
    public Guid Id { get; set; }
}

// ============================================================
// GET ALL CUSTOM FIELD DEFINITIONS
// ============================================================

public class CustomFieldDefinitionsHandler : IRequestHandler<CustomFieldDefinitionsQry, List<CustomFieldDefinitionDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CustomFieldDefinitionsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<CustomFieldDefinitionDto>> Handle(CustomFieldDefinitionsQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"", ""Name"", ""Label"", ""Type"", ""EntityType"",
                    ""IsRequired"", ""IsActive"", ""DefaultValue"",
                    ""OptionsJson"", ""ValidationRulesJson"", ""DisplayOrder"",
                    ""Category"", ""HelpText"", ""IsSearchable"", ""IsFilterable"",
                    ""CreatedAt"", ""UpdatedAt""
                FROM ""CustomFieldDefinitions""
                WHERE ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.EntityType))
            {
                var entityType = Enum.Parse<CustomFieldEntity>(request.EntityType);
                sql += @" AND ""EntityType"" = @EntityType";
                parameters.Add("@EntityType", entityType);
            }

            sql += @" ORDER BY ""DisplayOrder"" ASC, ""CreatedAt"" ASC";

            var data = await _dapper.QueryAsync<CustomFieldDefinitionDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get custom field definitions");
            throw;
        }
    }
}

// ============================================================
// GET CUSTOM FIELD DEFINITION BY ID
// ============================================================

public class CustomFieldDefinitionByIdHandler : IRequestHandler<CustomFieldDefinitionByIdQry, CustomFieldDefinitionDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CustomFieldDefinitionByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<CustomFieldDefinitionDto?> Handle(CustomFieldDefinitionByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"", ""Name"", ""Label"", ""Type"", ""EntityType"",
                    ""IsRequired"", ""IsActive"", ""DefaultValue"",
                    ""OptionsJson"", ""ValidationRulesJson"", ""DisplayOrder"",
                    ""Category"", ""HelpText"", ""IsSearchable"", ""IsFilterable"",
                    ""CreatedAt"", ""UpdatedAt""
                FROM ""CustomFieldDefinitions""
                WHERE ""Id"" = @Id AND ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<CustomFieldDefinitionDto>(sql, parameters, ct);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get custom field definition by ID: {DefinitionId}", request.Id);
            throw;
        }
    }
}