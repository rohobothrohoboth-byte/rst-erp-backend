using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Commands;

public class CreateVendorEvaluationCommandHandler
    : IRequestHandler<CreateVendorEvaluationCommand, VendorEvaluationDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreateVendorEvaluationCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreateVendorEvaluationCommandHandler(
        ProcurementDbContext context,
        ILogger<CreateVendorEvaluationCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorEvaluationDto> Handle(CreateVendorEvaluationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating vendor evaluation");

            // Validate vendor exists
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == request.CreateDto.VendorId && !v.IsDeleted, cancellationToken);

            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with ID '{request.CreateDto.VendorId}' not found");

            // Calculate overall score
            var totalWeight = request.CreateDto.Criteria.Sum(c => c.Weight);
            var overallScore = totalWeight > 0
                ? (int)Math.Round(request.CreateDto.Criteria.Sum(c => (double)c.Score * c.Weight / totalWeight))
                : 0;

            var status = overallScore >= 80 ? "Excellent"
                : overallScore >= 60 ? "Good"
                : overallScore >= 40 ? "Average"
                : "Poor";

            var evaluation = new VendorEvaluation
            {
                Id = Guid.NewGuid(),
                VendorId = request.CreateDto.VendorId,
                VendorName = vendor.Name,
                VendorCode = vendor.Code,
                OverallScore = overallScore,
                Category = request.CreateDto.Category,
                EvaluationDate = request.CreateDto.EvaluationDate != DateTime.MinValue
                    ? request.CreateDto.EvaluationDate
                    : DateTime.UtcNow,
                Evaluator = request.CreateDto.Evaluator,
                Status = status,
                CriteriaJson = JsonSerializer.Serialize(request.CreateDto.Criteria),
                StrengthsJson = JsonSerializer.Serialize(request.CreateDto.Strengths),
                WeaknessesJson = JsonSerializer.Serialize(request.CreateDto.Weaknesses),
                RecommendationsJson = JsonSerializer.Serialize(request.CreateDto.Recommendations),
                Notes = request.CreateDto.Notes,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.VendorEvaluations.AddAsync(evaluation, cancellationToken);

            // Update vendor rating
            var allEvaluations = await _context.VendorEvaluations
                .Where(e => e.VendorId == request.CreateDto.VendorId && !e.IsDeleted)
                .ToListAsync(cancellationToken);

            if (allEvaluations.Any())
            {
                var avgScore = allEvaluations.Average(e => e.OverallScore);
                vendor.Rating = Math.Round((decimal)avgScore / 20, 2); // Convert to 5-star scale
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"evaluations_all", cancellationToken);
            await _cache.RemoveAsync($"evaluations_vendor_{request.CreateDto.VendorId}", cancellationToken);
            await _cache.RemoveAsync($"vendor_{request.CreateDto.VendorId}", cancellationToken);

            _logger.LogInformation($"Vendor evaluation created for {vendor.Name} with score {overallScore}%");

            return await MapToDto(evaluation, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vendor evaluation");
            throw;
        }
    }

    private async Task<VendorEvaluationDto> MapToDto(VendorEvaluation evaluation, CancellationToken ct)
    {
        return new VendorEvaluationDto
        {
            Id = evaluation.Id,
            VendorId = evaluation.VendorId,
            VendorName = evaluation.VendorName,
            VendorCode = evaluation.VendorCode,
            OverallScore = evaluation.OverallScore,
            Category = evaluation.Category,
            EvaluationDate = evaluation.EvaluationDate,
            Evaluator = evaluation.Evaluator,
            Status = evaluation.Status,
            Criteria = JsonSerializer.Deserialize<List<EvaluationCriteriaDto>>(evaluation.CriteriaJson ?? "[]") ?? new(),
            Strengths = JsonSerializer.Deserialize<List<string>>(evaluation.StrengthsJson ?? "[]") ?? new(),
            Weaknesses = JsonSerializer.Deserialize<List<string>>(evaluation.WeaknessesJson ?? "[]") ?? new(),
            Recommendations = JsonSerializer.Deserialize<List<string>>(evaluation.RecommendationsJson ?? "[]") ?? new(),
            Notes = evaluation.Notes,
            DateAdd = evaluation.DateAdd,
            DateMod = evaluation.DateMod
        };
    }
}

public class DeleteVendorEvaluationCommandHandler
    : IRequestHandler<DeleteVendorEvaluationCommand, bool>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<DeleteVendorEvaluationCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeleteVendorEvaluationCommandHandler(
        ProcurementDbContext context,
        ILogger<DeleteVendorEvaluationCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteVendorEvaluationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var evaluation = await _context.VendorEvaluations
                .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

            if (evaluation == null)
                return false;

            var vendorId = evaluation.VendorId;

            evaluation.IsDeleted = true;
            evaluation.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // Update vendor rating
            var allEvaluations = await _context.VendorEvaluations
                .Where(e => e.VendorId == vendorId && !e.IsDeleted)
                .ToListAsync(cancellationToken);

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken);

            if (vendor != null)
            {
                if (allEvaluations.Any())
                {
                    var avgScore = allEvaluations.Average(e => e.OverallScore);
                    vendor.Rating = Math.Round((decimal)avgScore / 20, 2);
                }
                else
                {
                    vendor.Rating = null;
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            await _cache.RemoveAsync($"evaluation_{request.Id}", cancellationToken);
            await _cache.RemoveAsync("evaluations_all", cancellationToken);
            await _cache.RemoveAsync($"evaluations_vendor_{vendorId}", cancellationToken);
            await _cache.RemoveAsync($"vendor_{vendorId}", cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vendor evaluation");
            throw;
        }
    }
}