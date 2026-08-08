// CustomFieldDefinition.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Cor.CRM.Models.Entities;

public enum CustomFieldType
{
    Text = 1,
    Number = 2,
    Date = 3,
    DateTime = 4,
    Boolean = 5,
    Dropdown = 6,
    MultiSelect = 7,
    TextArea = 8,
    URL = 9,
    Email = 10,
    Phone = 11
}

public enum CustomFieldEntity
{
    Lead = 1,
    Customer = 2,
    Opportunity = 3,
    Task = 4,
    Activity = 5
}

public class CustomFieldDefinition : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Label { get; set; }

    public CustomFieldType Type { get; set; }
    public CustomFieldEntity EntityType { get; set; }

    public bool IsRequired { get; set; } = false;
    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? DefaultValue { get; set; }

    public string? OptionsJson { get; set; } // For dropdown/multiselect

    public string? ValidationRulesJson { get; set; }

    public int DisplayOrder { get; set; } = 0;

    [MaxLength(50)]
    public string? Category { get; set; }

    [MaxLength(500)]
    public string? HelpText { get; set; }

    public bool IsSearchable { get; set; } = false;
    public bool IsFilterable { get; set; } = false;
}