// Cor.Finance/Queries/AccountTypeQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============================================================
// ACCOUNT TYPE QUERIES
// ============================================================

public class GetAllAccountTypesQry : IRequest<PaginatedResponse<AccountTypeDto>>
{
    public bool? IsActive { get; set; }
    public string? Category { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "Code";
    public string? SortOrder { get; set; } = "ASC";
}

public class GetAccountTypeByIdQry : IRequest<AccountTypeDto>
{
    public Guid Id { get; set; }
}

public class GetAccountTypeByCodeQry : IRequest<AccountTypeDto>
{
    public string Code { get; set; } = string.Empty;
}

public class GetAccountTypesByCategoryQry : IRequest<List<AccountTypeDto>>
{
    public string Category { get; set; } = string.Empty;
}

public class GetAccountTypeWithSubtypesQry : IRequest<AccountTypeWithSubtypesDto>
{
    public Guid Id { get; set; }
}

public class GetAllAccountTypesWithSubtypesQry : IRequest<List<AccountTypeWithSubtypesDto>>
{
}

// ============================================================
// ACCOUNT SUBTYPE QUERIES
// ============================================================

public class GetAllAccountSubtypesQry : IRequest<PaginatedResponse<AccountSubtypeDto>>
{
    public bool? IsActive { get; set; }
    public Guid? AccountTypeId { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "Code";
    public string? SortOrder { get; set; } = "ASC";
}

public class GetAccountSubtypesByTypeIdQry : IRequest<List<AccountSubtypeDto>>
{
    public Guid AccountTypeId { get; set; }
}

public class GetAccountSubtypesByTypeCodeQry : IRequest<List<AccountSubtypeDto>>
{
    public string TypeCode { get; set; } = string.Empty;
}

public class GetAccountSubtypeByIdQry : IRequest<AccountSubtypeDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// GET ALL ACCOUNT TYPES HANDLER
// ============================================================

public class GetAllAccountTypesHandler : IRequestHandler<GetAllAccountTypesQry, PaginatedResponse<AccountTypeDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllAccountTypesHandler> _logger;

    public GetAllAccountTypesHandler(FinanceDbContext context, ILogger<GetAllAccountTypesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<AccountTypeDto>> Handle(GetAllAccountTypesQry request, CancellationToken ct)
    {
        try
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Min(100, Math.Max(1, request.PageSize));

            var query = _context.AccountTypes
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (!string.IsNullOrEmpty(request.Category))
                query = query.Where(x => x.Category == request.Category);

            if (!string.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search) ||
                    (x.NameAm != null && x.NameAm.ToLower().Contains(search))
                );
            }

            var totalCount = await query.CountAsync(ct);

            query = request.SortBy?.ToLower() switch
            {
                "code" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code),
                "name" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name),
                "category" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Category)
                    : query.OrderBy(x => x.Category),
                _ => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code)
            };

            var types = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AccountTypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Category = x.Category,
                    NormalBalance = x.NormalBalance,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    SubtypeCount = _context.AccountSubtypes.Count(s => s.AccountTypeId == x.Id && !s.IsDeleted)
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} account types (Page {Page}/{TotalPages})",
                types.Count, page, (int)Math.Ceiling((double)totalCount / pageSize));

            return new PaginatedResponse<AccountTypeDto>
            {
                Data = types,
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
            _logger.LogError(ex, "Error retrieving account types");
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT TYPE BY ID HANDLER
// ============================================================

public class GetAccountTypeByIdHandler : IRequestHandler<GetAccountTypeByIdQry, AccountTypeDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountTypeByIdHandler> _logger;

    public GetAccountTypeByIdHandler(FinanceDbContext context, ILogger<GetAccountTypeByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountTypeDto> Handle(GetAccountTypeByIdQry request, CancellationToken ct)
    {
        try
        {
            var accountType = await _context.AccountTypes
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new AccountTypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Category = x.Category,
                    NormalBalance = x.NormalBalance,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    SubtypeCount = _context.AccountSubtypes.Count(s => s.AccountTypeId == x.Id && !s.IsDeleted)
                })
                .FirstOrDefaultAsync(ct);

            if (accountType == null)
                throw new InvalidOperationException($"Account Type with ID '{request.Id}' not found");

            return accountType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account type by ID: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT TYPE BY CODE HANDLER
// ============================================================

public class GetAccountTypeByCodeHandler : IRequestHandler<GetAccountTypeByCodeQry, AccountTypeDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountTypeByCodeHandler> _logger;

    public GetAccountTypeByCodeHandler(FinanceDbContext context, ILogger<GetAccountTypeByCodeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountTypeDto> Handle(GetAccountTypeByCodeQry request, CancellationToken ct)
    {
        try
        {
            var accountType = await _context.AccountTypes
                .Where(x => x.Code == request.Code && !x.IsDeleted)
                .Select(x => new AccountTypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Category = x.Category,
                    NormalBalance = x.NormalBalance,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    SubtypeCount = _context.AccountSubtypes.Count(s => s.AccountTypeId == x.Id && !s.IsDeleted)
                })
                .FirstOrDefaultAsync(ct);

            if (accountType == null)
                throw new InvalidOperationException($"Account Type with code '{request.Code}' not found");

            return accountType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account type by code: {Code}", request.Code);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT TYPES BY CATEGORY HANDLER
// ============================================================

public class GetAccountTypesByCategoryHandler : IRequestHandler<GetAccountTypesByCategoryQry, List<AccountTypeDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountTypesByCategoryHandler> _logger;

    public GetAccountTypesByCategoryHandler(FinanceDbContext context, ILogger<GetAccountTypesByCategoryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountTypeDto>> Handle(GetAccountTypesByCategoryQry request, CancellationToken ct)
    {
        try
        {
            var types = await _context.AccountTypes
                .Where(x => x.Category == request.Category && !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new AccountTypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Category = x.Category,
                    NormalBalance = x.NormalBalance,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    SubtypeCount = _context.AccountSubtypes.Count(s => s.AccountTypeId == x.Id && !s.IsDeleted)
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} account types of category '{Category}'", types.Count, request.Category);
            return types;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account types by category: {Category}", request.Category);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT TYPE WITH SUBTYPES HANDLER
// ============================================================

public class GetAccountTypeWithSubtypesHandler : IRequestHandler<GetAccountTypeWithSubtypesQry, AccountTypeWithSubtypesDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountTypeWithSubtypesHandler> _logger;

    public GetAccountTypeWithSubtypesHandler(FinanceDbContext context, ILogger<GetAccountTypeWithSubtypesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountTypeWithSubtypesDto> Handle(GetAccountTypeWithSubtypesQry request, CancellationToken ct)
    {
        try
        {
            var accountType = await _context.AccountTypes
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new AccountTypeWithSubtypesDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Category = x.Category,
                    NormalBalance = x.NormalBalance,
                    IsActive = x.IsActive,
                    Subtypes = x.Subtypes
                        .Where(s => !s.IsDeleted)
                        .Select(s => new AccountSubtypeDto
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Name = s.Name,
                            Description = s.Description,
                            AccountTypeId = s.AccountTypeId,
                            IsActive = s.IsActive,
                            SortOrder = s.SortOrder,
                            DateAdd = s.DateAdd,
                            DateMod = s.DateMod
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (accountType == null)
                throw new InvalidOperationException($"Account Type with ID '{request.Id}' not found");

            return accountType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account type with subtypes: {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET ALL ACCOUNT TYPES WITH SUBTYPES HANDLER
// ============================================================

public class GetAllAccountTypesWithSubtypesHandler : IRequestHandler<GetAllAccountTypesWithSubtypesQry, List<AccountTypeWithSubtypesDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllAccountTypesWithSubtypesHandler> _logger;

    public GetAllAccountTypesWithSubtypesHandler(FinanceDbContext context, ILogger<GetAllAccountTypesWithSubtypesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountTypeWithSubtypesDto>> Handle(GetAllAccountTypesWithSubtypesQry request, CancellationToken ct)
    {
        try
        {
            var types = await _context.AccountTypes
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .Select(x => new AccountTypeWithSubtypesDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Category = x.Category,
                    NormalBalance = x.NormalBalance,
                    IsActive = x.IsActive,
                    Subtypes = x.Subtypes
                        .Where(s => !s.IsDeleted && s.IsActive)
                        .OrderBy(s => s.SortOrder)
                        .Select(s => new AccountSubtypeDto
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Name = s.Name,
                            Description = s.Description,
                            AccountTypeId = s.AccountTypeId,
                            IsActive = s.IsActive,
                            SortOrder = s.SortOrder,
                            DateAdd = s.DateAdd,
                            DateMod = s.DateMod
                        })
                        .ToList()
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} account types with subtypes", types.Count);
            return types;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all account types with subtypes");
            throw;
        }
    }
}

// ============================================================
// GET ALL ACCOUNT SUBTYPES HANDLER
// ============================================================

public class GetAllAccountSubtypesHandler : IRequestHandler<GetAllAccountSubtypesQry, PaginatedResponse<AccountSubtypeDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllAccountSubtypesHandler> _logger;

    public GetAllAccountSubtypesHandler(FinanceDbContext context, ILogger<GetAllAccountSubtypesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<AccountSubtypeDto>> Handle(GetAllAccountSubtypesQry request, CancellationToken ct)
    {
        try
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Min(100, Math.Max(1, request.PageSize));

            var query = _context.AccountSubtypes
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (request.AccountTypeId.HasValue)
                query = query.Where(x => x.AccountTypeId == request.AccountTypeId.Value);

            if (!string.IsNullOrEmpty(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search) ||
                    (x.NameAm != null && x.NameAm.ToLower().Contains(search))
                );
            }

            var totalCount = await query.CountAsync(ct);

            query = request.SortBy?.ToLower() switch
            {
                "code" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code),
                "name" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name),
                _ => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Code)
                    : query.OrderBy(x => x.Code)
            };

            var subtypes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AccountSubtypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountTypeId = x.AccountTypeId,
                    AccountTypeName = x.AccountType.Name,
                    AccountTypeCode = x.AccountType.Code,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} account subtypes (Page {Page}/{TotalPages})",
                subtypes.Count, page, (int)Math.Ceiling((double)totalCount / pageSize));

            return new PaginatedResponse<AccountSubtypeDto>
            {
                Data = subtypes,
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
            _logger.LogError(ex, "Error retrieving account subtypes");
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT SUBTYPES BY TYPE ID HANDLER
// ============================================================

public class GetAccountSubtypesByTypeIdHandler : IRequestHandler<GetAccountSubtypesByTypeIdQry, List<AccountSubtypeDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountSubtypesByTypeIdHandler> _logger;

    public GetAccountSubtypesByTypeIdHandler(FinanceDbContext context, ILogger<GetAccountSubtypesByTypeIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountSubtypeDto>> Handle(GetAccountSubtypesByTypeIdQry request, CancellationToken ct)
    {
        try
        {
            var subtypes = await _context.AccountSubtypes
                .Where(x => x.AccountTypeId == request.AccountTypeId && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .Select(x => new AccountSubtypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountTypeId = x.AccountTypeId,
                    AccountTypeName = x.AccountType.Name,
                    AccountTypeCode = x.AccountType.Code,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} subtypes for account type {TypeId}",
                subtypes.Count, request.AccountTypeId);
            return subtypes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subtypes by type ID: {TypeId}", request.AccountTypeId);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT SUBTYPES BY TYPE CODE HANDLER
// ============================================================

public class GetAccountSubtypesByTypeCodeHandler : IRequestHandler<GetAccountSubtypesByTypeCodeQry, List<AccountSubtypeDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountSubtypesByTypeCodeHandler> _logger;

    public GetAccountSubtypesByTypeCodeHandler(FinanceDbContext context, ILogger<GetAccountSubtypesByTypeCodeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AccountSubtypeDto>> Handle(GetAccountSubtypesByTypeCodeQry request, CancellationToken ct)
    {
        try
        {
            var subtypes = await _context.AccountSubtypes
                .Where(x => x.AccountType.Code == request.TypeCode && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .Select(x => new AccountSubtypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountTypeId = x.AccountTypeId,
                    AccountTypeName = x.AccountType.Name,
                    AccountTypeCode = x.AccountType.Code,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Retrieved {Count} subtypes for account type code '{TypeCode}'",
                subtypes.Count, request.TypeCode);
            return subtypes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subtypes by type code: {TypeCode}", request.TypeCode);
            throw;
        }
    }
}

// ============================================================
// GET ACCOUNT SUBTYPE BY ID HANDLER
// ============================================================

public class GetAccountSubtypeByIdHandler : IRequestHandler<GetAccountSubtypeByIdQry, AccountSubtypeDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAccountSubtypeByIdHandler> _logger;

    public GetAccountSubtypeByIdHandler(FinanceDbContext context, ILogger<GetAccountSubtypeByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AccountSubtypeDto> Handle(GetAccountSubtypeByIdQry request, CancellationToken ct)
    {
        try
        {
            var subtype = await _context.AccountSubtypes
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new AccountSubtypeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountTypeId = x.AccountTypeId,
                    AccountTypeName = x.AccountType.Name,
                    AccountTypeCode = x.AccountType.Code,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .FirstOrDefaultAsync(ct);

            if (subtype == null)
                throw new InvalidOperationException($"Account Subtype with ID '{request.Id}' not found");

            return subtype;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account subtype by ID: {Id}", request.Id);
            throw;
        }
    }
}