using Asp.Versioning;
using Helpers;
using Leave.App.Commands;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE POLICY MANAGEMENT - Complete policy configuration
/// </summary>
[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/Policy")]
[ApiVersion("1.0")]
public class LeavePolicyController : ControllerBase
{
    private readonly IMediator _med;

    public LeavePolicyController(IMediator med)
    {
        _med = med;
    }

    private IEnumerable<string> GetModelStateErrors()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
    }

    // ==================== LEAVE TYPE ====================

    [HttpGet("Type/All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLeaveTypes()
    {
        var response = await _med.Send(new LeaveTypeAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Type/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaveType(Guid id)
    {
        var response = await _med.Send(new LeaveTypeByIdQry { Id = id });
        if (response == null) { throw new DomainException("LEAVE TYPE not found."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Type/Names")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveTypeNames()
    {
        var response = await _med.Send(new LeaveTypeNameAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Type/Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateLeaveType([FromBody] LeaveTypeAddDto addDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new LeaveTypeAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave type created successfully."));
    }

    [HttpPut("Type/Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLeaveType(Guid id, [FromBody] LeaveTypeModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new LeaveTypeModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave type updated successfully."));
    }

    [HttpDelete("Type/Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteLeaveType(Guid id)
    {
        var command = new LeaveTypeDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Leave type deleted successfully."));
    }

    [HttpPatch("Type/Status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeLeaveTypeStatus([FromBody] StatChangeDto statDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new LeaveTypeStatCmd { StatDto = statDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave type status changed."));
    }

    // ==================== LEAVE POLICY ====================

    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPolicies()
    {
        var response = await _med.Send(new LeavePolicyAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivePolicy()
    {
        var response = await _med.Send(new ActiveLeavePolicyQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicy(Guid id)
    {
        var response = await _med.Send(new LeavePolicyByIdQry { Id = id });
        if (response == null) { throw new DomainException($"Policy with id [{id}] not found."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePolicy([FromBody] LeavePolicyAddDto addDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new LeavePolicyAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Policy created successfully."));
    }

    [HttpPut("Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePolicy(Guid id, [FromBody] LeavePolicyModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new LeavePolicyModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Policy updated successfully."));
    }

    [HttpDelete("Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePolicy(Guid id)
    {
        var command = new LeavePolicyDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Policy deleted successfully."));
    }

    [HttpPost("Assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignPolicy()
    {
        var response = await _med.Send(new PolicyAssignCmd());
        return Ok(ApiResponse<object>.Ok(response, "Policy assignment completed."));
    }

    // ==================== POLICY CONFIGURATION ====================

    [HttpGet("Config/Active/{policyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveConfig(Guid policyId)
    {
        var response = await _med.Send(new ActivePolicyConfigQry { Id = policyId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Config/All/{policyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllConfigs(Guid policyId)
    {
        var response = await _med.Send(new PolicyConfigByPolicyIdQry { Id = policyId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Config/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConfig(Guid id)
    {
        var response = await _med.Send(new LeavePolicyConfigByIdQry { Id = id });
        if (response == null) throw new DomainException($"Config with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Config/Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateConfig([FromBody] LeavePolicyConfigAddDto addDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new LeavePolicyConfigAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Config created successfully."));
    }

    [HttpPut("Config/Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateConfig(Guid id, [FromBody] LeavePolicyConfigModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new LeavePolicyConfigModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Config updated successfully."));
    }

    [HttpDelete("Config/Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteConfig(Guid id)
    {
        var command = new LeavePolicyConfigDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Config deleted successfully."));
    }

    // ==================== APPROVAL CHAIN ====================

    [HttpGet("Chain/Active/{policyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveChain(Guid policyId)
    {
        var response = await _med.Send(new ActiveLeaveAppChainQry { Id = policyId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Chain/All/{policyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllChains(Guid policyId)
    {
        var response = await _med.Send(new LeaveAppChainByPolicyIdQry { Id = policyId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Chain/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChain(Guid id)
    {
        var response = await _med.Send(new LeaveAppChainByIdQry { Id = id });
        if (response == null) throw new DomainException($"Chain with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Chain/Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateChain([FromBody] LeaveAppChainAddDto addDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new LeaveAppChainAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Chain created successfully."));
    }

    [HttpPut("Chain/Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateChain(Guid id, [FromBody] LeaveAppChainModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new LeaveAppChainModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Chain updated successfully."));
    }

    [HttpDelete("Chain/Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteChain(Guid id)
    {
        var command = new LeaveAppChainDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Chain deleted successfully."));
    }

    // ==================== CHAIN STEPS ====================

    [HttpGet("Chain/Step/All/{chainId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSteps(Guid chainId)
    {
        var response = await _med.Send(new AppStepByChainIdQry { Id = chainId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Chain/Step/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStep(Guid id)
    {
        var response = await _med.Send(new LeaveAppStepByIdQry { Id = id });
        if (response == null) throw new DomainException($"Step with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Chain/Step/Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateStep([FromBody] LeaveAppStepAddDto addDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new LeaveAppStepAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Step created successfully."));
    }

    [HttpPut("Chain/Step/Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStep(Guid id, [FromBody] LeaveAppStepModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new LeaveAppStepModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Step updated successfully."));
    }

    [HttpDelete("Chain/Step/Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteStep(Guid id)
    {
        var command = new LeaveAppStepDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Step deleted successfully."));
    }

    [HttpPatch("Chain/Step/Status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeStepStatus([FromBody] StatChangeDto statDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new LeaveAppChainStatCmd { StatDto = statDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Step status changed."));
    }

    // ==================== ASSIGNMENT RULES ====================

    [HttpGet("Rule/Active/{policyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveRules(Guid policyId)
    {
        var response = await _med.Send(new ActiveAssignmentRulesQry { Id = policyId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Rule/All/{policyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRules(Guid policyId)
    {
        var response = await _med.Send(new AssignmentRuleByPolicyIdQry { Id = policyId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Rule/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRule(Guid id)
    {
        var response = await _med.Send(new PolicyAssignmentRuleByIdQry { Id = id });
        if (response == null) throw new DomainException($"Rule with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Rule/Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRule([FromBody] PolicyAssignmentRuleAddDto addDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new PolicyAssignmentRuleAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Rule created successfully."));
    }

    [HttpPut("Rule/Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRule(Guid id, [FromBody] PolicyAssignmentRuleModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new PolicyAssignmentRuleModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Rule updated successfully."));
    }

    [HttpDelete("Rule/Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRule(Guid id)
    {
        var command = new PolicyAssignmentRuleDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Rule deleted successfully."));
    }

    // ==================== RULE CONDITIONS ====================

    [HttpGet("Rule/Condition/All/{ruleId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllConditions(Guid ruleId)
    {
        var response = await _med.Send(new PolicyRuleCondByRuleIdQry { Id = ruleId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Rule/Condition/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCondition(Guid id)
    {
        var response = await _med.Send(new PolicyRuleCondByIdQry { Id = id });
        if (response == null) throw new DomainException($"Condition with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Rule/Condition/Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCondition([FromBody] PolicyRuleCondAddDto addDto)
    {
        if (!ModelState.IsValid) throw new ValException(GetModelStateErrors());
        var command = new PolicyRuleCondAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Condition created successfully."));
    }

    [HttpPut("Rule/Condition/Update/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCondition(Guid id, [FromBody] PolicyRuleCondModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id) throw new ValException(GetModelStateErrors());
        var command = new PolicyRuleCondModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Condition updated successfully."));
    }

    [HttpDelete("Rule/Condition/Delete/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCondition(Guid id)
    {
        var command = new PolicyRuleCondDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Condition deleted successfully."));
    }
}