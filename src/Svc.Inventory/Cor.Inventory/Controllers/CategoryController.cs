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
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("inv.products.category.view")]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse<List<CategoryDto>>.Ok(categories));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, ApiResponse<List<CategoryDto>>.Fail("Failed to get categories", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    [PerAuth("inv.products.category.view")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<CategoryDto>.Fail($"Category with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<CategoryDto>.Ok(category));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {Id}", id);
            return StatusCode(500, ApiResponse<CategoryDto>.Fail("Failed to get category", statusCode: 500));
        }
    }

    [HttpPost]
    [PerAuth("inv.products.category.add")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        try
        {
            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id, version = "1.0" },
                ApiResponse<CategoryDto>.Ok(category, "Category created"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, ApiResponse<CategoryDto>.Fail("Failed to create category", statusCode: 500));
        }
    }

    [HttpPut]
    [PerAuth("inv.products.category.mod")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryDto dto)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(dto);
            return Ok(ApiResponse<CategoryDto>.Ok(category, "Category updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CategoryDto>.Fail(ex.Message, statusCode: 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.Fail(ex.Message));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(ApiResponse<CategoryDto>.Fail("The category was modified by another user", statusCode: 409));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return StatusCode(500, ApiResponse<CategoryDto>.Fail("Failed to update category", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [PerAuth("inv.products.category.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.Fail($"Category with ID '{id}' not found", statusCode: 404));

            return Ok(ApiResponse<object>.Ok(null, "Category deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail("Failed to delete category", statusCode: 500));
        }
    }
}
