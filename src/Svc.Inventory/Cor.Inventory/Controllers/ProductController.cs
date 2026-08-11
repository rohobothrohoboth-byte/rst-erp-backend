using Asp.Versioning;
using Common;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("inv.products.list.view")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            return Ok(ApiResponse<List<ProductDto>>.Ok(products));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, ApiResponse<List<ProductDto>>.Fail("Failed to get products", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    [PerAuth("inv.products.list.view")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductDto>.Fail($"Product with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<ProductDto>.Ok(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {Id}", id);
            return StatusCode(500, ApiResponse<ProductDto>.Fail("Failed to get product", statusCode: 500));
        }
    }

    [HttpPost]
    [PerAuth("inv.products.list.add")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id, version = "1.0" },
                ApiResponse<ProductDto>.Ok(product, "Product created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, ApiResponse<ProductDto>.Fail("Failed to create product", statusCode: 500));
        }
    }

    [HttpPut]
    [PerAuth("inv.products.list.mod")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] UpdateProductDto dto)
    {
        try
        {
            var product = await _productService.UpdateAsync(dto);
            return Ok(ApiResponse<ProductDto>.Ok(product, "Product updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProductDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductDto>.Fail(ex.Message));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(ApiResponse<ProductDto>.Fail("The product was modified by another user", statusCode: 409));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            return StatusCode(500, ApiResponse<ProductDto>.Fail("Failed to update product", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [PerAuth("inv.products.list.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.Fail($"Product with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<object>.Ok(null, "Product deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail("Failed to delete product", statusCode: 500));
        }
    }
}
