using Asp.Versioning;
using Common;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Persistence;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Controllers;

[ApiController]
[Route("api/inventory/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class BarcodeController : ControllerBase
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<BarcodeController> _logger;

    public BarcodeController(InventoryDbContext context, ILogger<BarcodeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("inv.products.barcode.view")]
    [ProducesResponseType(typeof(ApiResponse<List<BarcodeProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _context.Products.AsNoTracking()
                .Where(p => p.Barcode != null && p.Barcode != "")
                .OrderBy(p => p.Name)
                .Select(p => new BarcodeProductDto
                {
                    ProductId = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Barcode = p.Barcode
                })
                .ToListAsync();

            return Ok(ApiResponse<List<BarcodeProductDto>>.Ok(products));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products with barcodes");
            return StatusCode(500, ApiResponse<List<BarcodeProductDto>>.Fail("Failed to get products with barcodes", statusCode: 500));
        }
    }

    [HttpPost("generate/{productId}")]
    [PerAuth("inv.products.barcode.generate")]
    [ProducesResponseType(typeof(ApiResponse<BarcodeProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Generate(Guid productId, [FromBody] GenerateBarcodeDto? dto)
    {
        try
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                return NotFound(ApiResponse<BarcodeProductDto>.Fail($"Product with ID '{productId}' not found", statusCode: 404));

            var barcode = !string.IsNullOrWhiteSpace(dto?.Barcode)
                ? dto!.Barcode!
                : GenerateBarcode();

            product.Barcode = barcode;
            product.DateMod = DateTime.UtcNow;
            product.UpdateRowVersion();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Assigned barcode {Barcode} to product {ProductId}", barcode, productId);

            return Ok(ApiResponse<BarcodeProductDto>.Ok(new BarcodeProductDto
            {
                ProductId = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Barcode = product.Barcode
            }, "Barcode generated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating barcode for product {ProductId}", productId);
            return StatusCode(500, ApiResponse<BarcodeProductDto>.Fail("Failed to generate barcode", statusCode: 500));
        }
    }

    [HttpGet("scan/{barcode}")]
    [PerAuth("inv.products.barcode.scan")]
    [ProducesResponseType(typeof(ApiResponse<BarcodeProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Scan(string barcode)
    {
        try
        {
            var product = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (product == null)
                return NotFound(ApiResponse<BarcodeProductDto>.Fail($"No product found for barcode '{barcode}'", statusCode: 404));

            return Ok(ApiResponse<BarcodeProductDto>.Ok(new BarcodeProductDto
            {
                ProductId = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Barcode = product.Barcode
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning barcode {Barcode}", barcode);
            return StatusCode(500, ApiResponse<BarcodeProductDto>.Fail("Failed to scan barcode", statusCode: 500));
        }
    }

    private static string GenerateBarcode()
    {
        // 13-digit numeric barcode derived from a GUID hash for uniqueness.
        var value = (uint)Guid.NewGuid().GetHashCode();
        var suffix = value.ToString().PadLeft(12, '0');
        if (suffix.Length > 12)
            suffix = suffix.Substring(suffix.Length - 12);
        return "2" + suffix;
    }
}
