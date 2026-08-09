// Queries/AccountCategoryQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============================================================
// QUERIES
// ============================================================

public class GetAllAccountCategoriesQry : IRequest<PaginatedResponse<AccountCategoryDto>>
{
    public bool? IsActive { get; set; }
    public string? Type { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "Code";
    public string? SortOrder { get; set; } = "ASC";
}

public class GetAccountCategoryByIdQry : IRequest<AccountCategoryDto>
{
    public Guid Id { get; set; }
}

public class GetAccountCategoryByCodeQry : IRequest<AccountCategoryDto>
{
    public string Code { get; set; } = string.Empty;
}

public class GetAccountCategoryByTypeQry : IRequest<List<AccountCategoryDto>>
{
    public string Type { get; set; } = string.Empty;
}

public class GetAccountCategoryHierarchyQry : IRequest<List<AccountCategoryHierarchyDto>>
{
}

public class GetCategoryUsageQry : IRequest<CategoryUsageDto>
{
    public Guid Id { get; set; }
}

public class CanDeleteAccountCategoryQry : IRequest<CanDeleteResponse>
{
    public Guid Id { get; set; }
}

public class ExportAccountCategoriesQry : IRequest<List<AccountCategoryDto>>
{
    public string? Type { get; set; }
    public bool? IsActive { get; set; }
}

// ============================================================
// GET ALL ACCOUNT CATEGORIES HANDLER - FIXED ✅
// ============================================================

public class GetAllAccountCategoriesHandler : IRequestHandler<GetAllAccountCategoriesQry, PaginatedResponse<AccountCategoryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllAccountCategoriesHandler> _logger;

    public GetAllAccountCategoriesHandler(FinanceDbContext context, ILogger<GetAllAccountCategoriesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<AccountCategoryDto>> Handle(GetAllAccountCategoriesQry request, CancellationToken ct)
    {
        try
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Min(100, Math.Max(1, request.PageSize));

            var query = _context.AccountCategories
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Apply filters
            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (!string.IsNullOrEmpty(request.Type))
                query = query.Where(x => x.Type == request.Type);

            if (!string.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search) ||
                    (x.NameAm != null && x.NameAm.ToLower().Contains(search))
                );
            }

            // ✅ FIX: Get total count first (single async operation)
            var totalCount = await query.CountAsync(ct);

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "code" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code),
                "name" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name),
                "type" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Type)
                    : query.OrderBy(x => x.Type),
                _ => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code)
            };

            // ✅ FIX: Get paginated data (single async operation)
            var categories = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AccountCategoryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Type = x.Type,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            // ✅ FIX: Get parent names in a separate single operation
            var parentIds = categories.Where(x => x.ParentId.HasValue).Select(x => x.ParentId!.Value).Distinct().ToList();
            if (parentIds.Any())
            {
                var parents = await _context.AccountCategories
                    .Where(x => parentIds.Contains(x.Id) && !x.IsDeleted)
                    .Select(x => new { x.Id, x.Name })
                    .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

                foreach (var cat in categories.Where(x => x.ParentId.HasValue))
                {
                    if (parents.TryGetValue(cat.ParentId!.Value, out var parentName))
                        cat.ParentName = parentName;
                }
            }

            _logger.LogInformation("✅ Retrieved {Count} account categories (Page {Page}/{TotalPages})",
                categories.Count, page, (int)Math.Ceiling((double)totalCount / pageSize));

            return new PaginatedResponse<AccountCategoryDto>
            {
                Data = categories,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = page > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account categories");
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT CATEGORY BY ID HANDLER
// ============================================================

public class GetAccountCategoryByIdHandler : IRequestHandler<GetAccountCategoryByIdQry, AccountCategoryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountCategoryByIdHandler> _logger;

    public GetAccountCategoryByIdHandler(FinanceDbContext context, ILogger<GetAccountCategoryByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountCategoryDto> Handle(GetAccountCategoryByIdQry request, CancellationToken ct)
    {
        try
        {
            var category = await _context.AccountCategories
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new AccountCategoryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Type = x.Type,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .FirstOrDefaultAsync(ct);

            if (category == null)
                throw new InvalidOperationException($"Account Category with ID '{request.Id}' not found");

            // Populate ParentName - single operation
            if (category.ParentId.HasValue)
            {
                var parent = await _context.AccountCategories
                    .Where(x => x.Id == category.ParentId.Value && !x.IsDeleted)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(ct);
                category.ParentName = parent;
            }

            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account category by ID: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT CATEGORY BY CODE HANDLER
// ============================================================

public class GetAccountCategoryByCodeHandler : IRequestHandler<GetAccountCategoryByCodeQry, AccountCategoryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountCategoryByCodeHandler> _logger;

    public GetAccountCategoryByCodeHandler(FinanceDbContext context, ILogger<GetAccountCategoryByCodeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountCategoryDto> Handle(GetAccountCategoryByCodeQry request, CancellationToken ct)
    {
        try
        {
            var category = await _context.AccountCategories
                .Where(x => x.Code == request.Code && !x.IsDeleted)
                .Select(x => new AccountCategoryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Type = x.Type,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .FirstOrDefaultAsync(ct);

            if (category == null)
                throw new InvalidOperationException($"Account Category with code '{request.Code}' not found");

            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account category by code: {Code}", request.Code);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT CATEGORY BY TYPE HANDLER
// ============================================================

public class GetAccountCategoryByTypeHandler : IRequestHandler<GetAccountCategoryByTypeQry, List<AccountCategoryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountCategoryByTypeHandler> _logger;

    public GetAccountCategoryByTypeHandler(FinanceDbContext context, ILogger<GetAccountCategoryByTypeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountCategoryDto>> Handle(GetAccountCategoryByTypeQry request, CancellationToken ct)
    {
        try
        {
            var categories = await _context.AccountCategories
                .Where(x => x.Type == request.Type && !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new AccountCategoryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Type = x.Type,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} account categories of type '{Type}'", categories.Count, request.Type);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account categories by type: {Type}", request.Type);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT CATEGORY HIERARCHY HANDLER
// ============================================================

public class GetAccountCategoryHierarchyHandler : IRequestHandler<GetAccountCategoryHierarchyQry, List<AccountCategoryHierarchyDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountCategoryHierarchyHandler> _logger;

    public GetAccountCategoryHierarchyHandler(FinanceDbContext context, ILogger<GetAccountCategoryHierarchyHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountCategoryHierarchyDto>> Handle(GetAccountCategoryHierarchyQry request, CancellationToken ct)
    {
        try
        {
            var categories = await _context.AccountCategories
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Code)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    x.Type,
                    x.ParentId
                })
                .ToListAsync(ct);

            var categoryDict = categories.ToDictionary(
                x => x.Id,
                x => new AccountCategoryHierarchyDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Type = x.Type,
                    Children = new List<AccountCategoryHierarchyDto>()
                });

            var rootCategories = new List<AccountCategoryHierarchyDto>();

            foreach (var cat in categoryDict.Values)
            {
                var entity = categories.First(c => c.Id == cat.Id);
                if (entity.ParentId.HasValue && categoryDict.ContainsKey(entity.ParentId.Value))
                {
                    categoryDict[entity.ParentId.Value].Children.Add(cat);
                }
                else
                {
                    rootCategories.Add(cat);
                }
            }

            // Sort children by code
            foreach (var cat in categoryDict.Values)
            {
                cat.Children = cat.Children.OrderBy(x => x.Code).ToList();
            }

            _logger.LogInformation("✅ Retrieved account category hierarchy with {Count} root nodes", rootCategories.Count);
            return rootCategories.OrderBy(x => x.Code).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account category hierarchy");
            throw;
        }
    }
}

// ============================================================
// GET CATEGORY USAGE HANDLER
// ============================================================

public class GetCategoryUsageHandler : IRequestHandler<GetCategoryUsageQry, CategoryUsageDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetCategoryUsageHandler> _logger;

    public GetCategoryUsageHandler(FinanceDbContext context, ILogger<GetCategoryUsageHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CategoryUsageDto> Handle(GetCategoryUsageQry request, CancellationToken ct)
    {
        try
        {
            var category = await _context.AccountCategories
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new { x.Code, x.Name })
                .FirstOrDefaultAsync(ct);

            if (category == null)
                throw new InvalidOperationException($"Account Category with ID '{request.Id}' not found");

            var accountCount = await _context.ChartOfAccounts
                .CountAsync(x => x.CategoryId == request.Id && !x.IsDeleted, ct);

            var hasChildren = await _context.AccountCategories
                .AnyAsync(x => x.ParentId == request.Id && !x.IsDeleted, ct);

            var canDelete = accountCount == 0 && !hasChildren;
            string? reason = null;

            if (!canDelete)
            {
                if (hasChildren && accountCount > 0)
                    reason = $"Category has {accountCount} accounts and child categories.";
                else if (hasChildren)
                    reason = "Category has child categories. Please delete or reassign child categories first.";
                else if (accountCount > 0)
                    reason = $"Category is used in {accountCount} account(s). Please remove or reassign these accounts first.";
            }

            return new CategoryUsageDto
            {
                CategoryId = request.Id,
                CategoryName = category.Name,
                AccountCount = accountCount,
                CanDelete = canDelete,
                Reason = reason
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category usage for ID: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// CAN DELETE ACCOUNT CATEGORY HANDLER
// ============================================================

public class CanDeleteAccountCategoryHandler : IRequestHandler<CanDeleteAccountCategoryQry, CanDeleteResponse>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<CanDeleteAccountCategoryHandler> _logger;

    public CanDeleteAccountCategoryHandler(FinanceDbContext context, ILogger<CanDeleteAccountCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CanDeleteResponse> Handle(CanDeleteAccountCategoryQry request, CancellationToken ct)
    {
        try
        {
            var accountCount = await _context.ChartOfAccounts
                .CountAsync(x => x.CategoryId == request.Id && !x.IsDeleted, ct);

            var hasChildren = await _context.AccountCategories
                .AnyAsync(x => x.ParentId == request.Id && !x.IsDeleted, ct);

            var canDelete = accountCount == 0 && !hasChildren;
            string? reason = null;

            if (!canDelete)
            {
                if (accountCount > 0 && hasChildren)
                    reason = $"Category has {accountCount} accounts and child categories";
                else if (hasChildren)
                    reason = "Category has child categories";
                else if (accountCount > 0)
                    reason = $"Category is used in {accountCount} accounts";
            }

            return new CanDeleteResponse
            {
                CanDelete = canDelete,
                Reason = reason,
                AccountCount = accountCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if category can be deleted: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// EXPORT ACCOUNT CATEGORIES HANDLER
// ============================================================

public class ExportAccountCategoriesHandler : IRequestHandler<ExportAccountCategoriesQry, List<AccountCategoryDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ExportAccountCategoriesHandler> _logger;

    public ExportAccountCategoriesHandler(FinanceDbContext context, ILogger<ExportAccountCategoriesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountCategoryDto>> Handle(ExportAccountCategoriesQry request, CancellationToken ct)
    {
        try
        {
            var query = _context.AccountCategories
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (!string.IsNullOrEmpty(request.Type))
                query = query.Where(x => x.Type == request.Type);

            var categories = await query
                .OrderBy(x => x.Code)
                .Select(x => new AccountCategoryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Type = x.Type,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Exported {Count} account categories", categories.Count);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting account categories");
            throw;
        }
    }
}