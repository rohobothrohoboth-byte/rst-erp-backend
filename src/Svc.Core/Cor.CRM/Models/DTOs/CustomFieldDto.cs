// Cor.CRM/Models/DTOs/CustomFieldDto.cs

namespace Cor.CRM.Models.DTOs;

public class CustomFieldDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string Type { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsJson { get; set; }
    public string? ValidationRulesJson { get; set; }
    public int DisplayOrder { get; set; }
    public string? Category { get; set; }
    public string? HelpText { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCustomFieldDefinitionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string Type { get; set; } = "Text";
    public string EntityType { get; set; } = "Lead";
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsJson { get; set; }
    public string? ValidationRulesJson { get; set; }
    public int DisplayOrder { get; set; }
    public string? Category { get; set; }
    public string? HelpText { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
}

public class UpdateCustomFieldDefinitionDto
{
    public string? Name { get; set; }
    public string? Label { get; set; }
    public string? Type { get; set; }
    public string? EntityType { get; set; }
    public bool? IsRequired { get; set; }
    public bool? IsActive { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsJson { get; set; }
    public string? ValidationRulesJson { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Category { get; set; }
    public string? HelpText { get; set; }
    public bool? IsSearchable { get; set; }
    public bool? IsFilterable { get; set; }
}