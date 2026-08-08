using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Queries;

public class GetAllVendorEvaluationsQueryHandler
    : IRequestHandler<GetAllVendorEvaluationsQuery, List<VendorEvaluationDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllVendorEvaluationsQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetAllVendorEvaluationsQueryHandler(
        ProcurementDbContext context,
        ILogger<GetAllVendorEvaluationsQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<VendorEvaluationDto>> Handle(GetAllVendorEvaluationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"evaluations_all_{request.VendorId}_{request.Status}";
            var cached = await _cache.GetAsync<List<VendorEvaluationDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var query = _context.VendorEvaluations
                .Include(e => e.Vendor)
                .Where(e => !e.IsDeleted);

            if (request.VendorId.HasValue)
                query = query.Where(e => e.VendorId == request.VendorId.Value);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(e => e.Status == request.Status);

            if (request.FromDate.HasValue)
                query = query.Where(e => e.EvaluationDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(e => e.EvaluationDate <= request.ToDate.Value);

            var evaluations = await query
                .OrderByDescending(e => e.EvaluationDate)
                .Select(e => new VendorEvaluationDto
                {
                    Id = e.Id,
                    VendorId = e.VendorId,
                    VendorName = e.VendorName ?? e.Vendor!.Name,
                    VendorCode = e.VendorCode ?? e.Vendor!.Code,
                    OverallScore = e.OverallScore,
                    Category = e.Category,
                    EvaluationDate = e.EvaluationDate,
                    Evaluator = e.Evaluator,
                    Status = e.Status,
                    Criteria = JsonSerializer.Deserialize<List<EvaluationCriteriaDto>>(e.CriteriaJson ?? "[]") ?? new(),
                    Strengths = JsonSerializer.Deserialize<List<string>>(e.StrengthsJson ?? "[]") ?? new(),
                    Weaknesses = JsonSerializer.Deserialize<List<string>>(e.WeaknessesJson ?? "[]") ?? new(),
                    Recommendations = JsonSerializer.Deserialize<List<string>>(e.RecommendationsJson ?? "[]") ?? new(),
                    Notes = e.Notes,
                    DateAdd = e.DateAdd,
                    DateMod = e.DateMod
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, evaluations, TimeSpan.FromMinutes(15), cancellationToken);
            return evaluations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vendor evaluations");
            throw;
        }
    }
}

public class GetVendorEvaluationByIdQueryHandler
    : IRequestHandler<GetVendorEvaluationByIdQuery, VendorEvaluationDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetVendorEvaluationByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetVendorEvaluationByIdQueryHandler(
        ProcurementDbContext context,
        ILogger<GetVendorEvaluationByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorEvaluationDto> Handle(GetVendorEvaluationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"evaluation_{request.Id}";
            var cached = await _cache.GetAsync<VendorEvaluationDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var evaluation = await _context.VendorEvaluations
                .Include(e => e.Vendor)
                .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

            if (evaluation == null)
                throw new KeyNotFoundException($"Evaluation with ID '{request.Id}' not found");

            var dto = new VendorEvaluationDto
            {
                Id = evaluation.Id,
                VendorId = evaluation.VendorId,
                VendorName = evaluation.VendorName ?? evaluation.Vendor?.Name,
                VendorCode = evaluation.VendorCode ?? evaluation.Vendor?.Code,
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

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vendor evaluation {Id}", request.Id);
            throw;
        }
    }
}