// Controllers/CustomerController.cs
using Common;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class CustomerController : BaseApiController
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator, ILogger<CustomerController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [PerAuth("fnm.ar.customer.view")]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _mediator.Send(new CustomerAllQry());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllCustomers");
        }
    }

    [HttpGet("{id}")]
    [PerAuth("fnm.ar.customer.view")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new CustomerByIdQry { Id = id });
            if (result == null)
                return NotFound(new { message = $"Customer with ID {id} not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetCustomerById", id);
        }
    }

    [HttpGet("code/{code}")]
    [PerAuth("fnm.ar.customer.view")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var result = await _mediator.Send(new CustomerByCodeQry { Code = code });
            if (result == null)
                return NotFound(new { message = $"Customer with code {code} not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetCustomerByCode", code);
        }
    }

    [HttpGet("type/{type}")]
    [PerAuth("fnm.ar.customer.view")]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByType(string type)
    {
        try
        {
            var result = await _mediator.Send(new CustomerByTypeQry { CustomerType = type });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetCustomerByType", type);
        }
    }

    [HttpGet("active")]
    [PerAuth("fnm.ar.customer.view")]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var result = await _mediator.Send(new CustomerActiveQry());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetActiveCustomers");
        }
    }

    [HttpGet("search")]
    [PerAuth("fnm.ar.customer.view")]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? term,
        [FromQuery] string? customerType,
        [FromQuery] bool? isActive)
    {
        try
        {
            var result = await _mediator.Send(new CustomerSearchQry
            {
                SearchTerm = term,
                CustomerType = customerType,
                IsActive = isActive
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "SearchCustomers");
        }
    }

    [HttpPost]
    [PerAuth("fnm.ar.customer.add")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto customer)
    {
        try
        {
            if (customer == null)
                return BadRequest(new { message = "Invalid customer data" });

            var result = await _mediator.Send(new CreateCustomerCmd { Customer = customer });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Customer created successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateCustomer");
        }
    }

    [HttpPut]
    [PerAuth("fnm.ar.customer.mod")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] CustomerUpdateDto customer)
    {
        try
        {
            if (customer == null || customer.Id == Guid.Empty)
                return BadRequest(new { message = "Invalid customer data" });

            var result = await _mediator.Send(new UpdateCustomerCmd { Customer = customer });
            return Ok(new
            {
                success = true,
                message = "Customer updated successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateCustomer", customer?.Id);
        }
    }

    [HttpDelete("{id}")]
    [PerAuth("fnm.ar.customer.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteCustomerCmd { Id = id });
            if (!result)
                return NotFound(new { message = $"Customer with ID {id} not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteCustomer", id);
        }
    }

    [HttpPost("{id}/toggle-status")]
    [PerAuth("fnm.ar.customer.view")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new ToggleCustomerStatusCmd { Id = id });
            return Ok(new
            {
                success = true,
                message = $"Customer status toggled to {(result.IsActive ? "Active" : "Inactive")}",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ToggleCustomerStatus", id);
        }
    }

    [HttpPost("bulk")]
    [PerAuth("fnm.ar.customer.add")]
    [ProducesResponseType(typeof(BulkOperationResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkCreate([FromBody] List<CustomerCreateDto> customers)
    {
        try
        {
            var result = await _mediator.Send(new BulkCreateCustomersCmd { Customers = customers });
            return Ok(new
            {
                success = true,
                message = $"Bulk create completed: {result.SuccessCount} succeeded, {result.FailedCount} failed",
                data = result
            });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "BulkCreateCustomers");
        }
    }
}