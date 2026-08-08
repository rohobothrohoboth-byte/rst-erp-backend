// Models/DTOs/PaymentApprovalChainDto.cs
using System;
using System.Collections.Generic;

namespace Cor.Finance.Models.DTOs;

public class PaymentApprovalChainDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool IsActive { get; set; }
    public List<ApprovalStepDto> Steps { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class ApprovalStepDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? ApproverName { get; set; }
    public Guid? ApproverId { get; set; }
}

public class AddPaymentApprovalChainDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public List<AddApprovalStepDto> Steps { get; set; } = new();
}

public class EditPaymentApprovalChainDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool IsActive { get; set; }
    public List<EditApprovalStepDto> Steps { get; set; } = new();
}

public class AddApprovalStepDto
{
    public int Order { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? ApproverName { get; set; }
    public Guid? ApproverId { get; set; }
}

public class EditApprovalStepDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? ApproverName { get; set; }
    public Guid? ApproverId { get; set; }
}