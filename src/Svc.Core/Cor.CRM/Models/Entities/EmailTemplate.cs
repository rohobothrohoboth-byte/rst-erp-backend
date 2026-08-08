// EmailTemplate.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cor.CRM.Models.Entities;

public enum EmailTemplateCategory
{
    Welcome = 1,
    FollowUp = 2,
    Meeting = 3,
    Quote = 4,
    Invoice = 5,
    Newsletter = 6,
    Campaign = 7,
    Others = 8
}

public class EmailTemplate : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public string? HtmlBody { get; set; }

    public EmailTemplateCategory Category { get; set; }

    [MaxLength(50)]
    public string? Language { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;

    [MaxLength(500)]
    public string? Tags { get; set; }

    public int UseCount { get; set; } = 0;

    public string? VariablesJson { get; set; } // Available template variables
}