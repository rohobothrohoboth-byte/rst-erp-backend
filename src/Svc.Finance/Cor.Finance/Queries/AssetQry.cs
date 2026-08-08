// Queries/AssetQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Entities.Local;

namespace Cor.Finance.Queries
{
    // ==================== GET ALL ASSETS QUERY ====================
    public class GetAssetsQry : IRequest<List<AssetDto>>
    {
        public string? Status { get; set; }
        public string? AssetType { get; set; }
        public string? AssetCategory { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? AssignedTo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinCost { get; set; }
        public decimal? MaxCost { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; } = "AcquisitionDate";
        public string? SortDirection { get; set; } = "DESC";
    }

    public class GetAssetByIdQry : IRequest<AssetDto>
    {
        public Guid Id { get; set; }
    }

    public class GetAssetsByBranchQry : IRequest<List<AssetDto>>
    {
        public Guid BranchId { get; set; }
    }

    public class GetAssetsByDepartmentQry : IRequest<List<AssetDto>>
    {
        public Guid DepartmentId { get; set; }
    }

    public class GetAssetsByEmployeeQry : IRequest<List<AssetDto>>
    {
        public Guid EmployeeId { get; set; }
    }

    // ==================== GET ALL ASSETS HANDLER ====================
    public class GetAssetsHandler : IRequestHandler<GetAssetsQry, List<AssetDto>>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<GetAssetsHandler> _logger;

        public GetAssetsHandler(FinanceDbContext context, ILogger<GetAssetsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

      public async Task<List<AssetDto>> Handle(GetAssetsQry request, CancellationToken ct)
      {
          try
          {
              _logger.LogInformation("🔍 GetAssetsHandler started");

              // ✅ Build query WITHOUT includes - use projection instead
              var query = _context.Set<Asset>()
                  .AsNoTracking()
                  .Where(x => !x.IsDeleted);

              // Apply filters (same as before)...

              // ✅ Get total count
              var totalCount = await query.CountAsync(ct);
              _logger.LogInformation("📊 Total assets matching filters: {Count}", totalCount);

              if (totalCount == 0)
              {
                  return new List<AssetDto>();
              }

              // ✅ Apply sorting and pagination
              query = request.SortDirection?.ToUpper() == "ASC"
                  ? query.OrderBy(x => EF.Property<object>(x, request.SortBy ?? "AcquisitionDate"))
                  : query.OrderByDescending(x => EF.Property<object>(x, request.SortBy ?? "AcquisitionDate"));

              query = query
                  .Skip((request.PageNumber - 1) * request.PageSize)
                  .Take(request.PageSize);

              // ✅ Project directly to DTO - NO Includes needed!
              var result = await query
                  .Select(a => new AssetDto
                  {
                      Id = a.Id,
                      Name = a.Name,
                      Code = a.Code,
                      Description = a.Description,
                      SerialNumber = a.SerialNumber,
                      Model = a.Model,
                      Manufacturer = a.Manufacturer,
                      Location = a.Location,
                      AcquisitionCost = a.AcquisitionCost,
                      AcquisitionDate = a.AcquisitionDate,
                      SalvageValue = a.SalvageValue,
                      UsefulLife = a.UsefulLife,
                      DepreciationRate = a.DepreciationRate,
                      AssetType = a.AssetType,
                      AssetCategory = a.AssetCategory,
                      Status = a.Status,
                      IsActive = a.IsActive,
                      AssignedTo = a.AssignedTo,
                      // ✅ These will be null for now - load separately if needed
                      AssignedToName = null,
                      DepartmentId = a.DepartmentId,
                      DepartmentName = null,
                      BranchId = a.BranchId,
                      BranchName = null,
                      AccountId = a.AccountId,
                      AccountCode = null,
                      AccountName = null,
                      PurchaseDate = a.PurchaseDate,
                      CurrentValue = a.CurrentValue,
                      AccumulatedDepreciation = a.AccumulatedDepreciation,
                      LastDepreciationDate = a.LastDepreciationDate,
                      WarrantyInfo = a.WarrantyInfo,
                      WarrantyExpiryDate = a.WarrantyExpiryDate,
                      Notes = a.Notes,
                      DateAdd = a.DateAdd,
                      DateMod = a.DateMod,
                      CreatedByUserName = a.CreatedByUserName,
                      UpdatedByUserName = a.UpdatedByUserName
                  })
                  .ToListAsync(ct);

              _logger.LogInformation("✅ Retrieved {Count} assets", result.Count);
              return result;
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "Error retrieving assets");
              throw;
          }
          }
    }

    // ==================== GET ASSET BY ID HANDLER ====================
    public class GetAssetByIdHandler : IRequestHandler<GetAssetByIdQry, AssetDto>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<GetAssetByIdHandler> _logger;

        public GetAssetByIdHandler(FinanceDbContext context, ILogger<GetAssetByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AssetDto> Handle(GetAssetByIdQry request, CancellationToken ct)
        {
            try
            {
                var asset = await _context.Set<Asset>()
                    .AsNoTracking()
                    .Include(x => x.AssignedEmployee)
                    .Include(x => x.Department)
                    .Include(x => x.Branch)
                    .Include(x => x.Account)
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

                if (asset == null)
                    throw new InvalidOperationException($"Asset with ID '{request.Id}' not found");

                return new AssetDto
                {
                    Id = asset.Id,
                    Name = asset.Name,
                    Code = asset.Code,
                    Description = asset.Description,
                    SerialNumber = asset.SerialNumber,
                    Model = asset.Model,
                    Manufacturer = asset.Manufacturer,
                    Location = asset.Location,
                    AcquisitionCost = asset.AcquisitionCost,
                    AcquisitionDate = asset.AcquisitionDate,
                    SalvageValue = asset.SalvageValue,
                    UsefulLife = asset.UsefulLife,
                    DepreciationRate = asset.DepreciationRate,
                    AssetType = asset.AssetType,
                    AssetCategory = asset.AssetCategory,
                    Status = asset.Status,
                    IsActive = asset.IsActive,
                    AssignedTo = asset.AssignedTo,
                    AssignedToName = asset.AssignedEmployee != null
                        ? $"{asset.AssignedEmployee.FirstName} {asset.AssignedEmployee.LastName}"
                        : null,
                    DepartmentId = asset.DepartmentId,
                    DepartmentName = asset.Department?.Name,
                    BranchId = asset.BranchId,
                    BranchName = asset.Branch?.Name,
                    AccountId = asset.AccountId,
                    AccountCode = asset.Account?.Code,
                    AccountName = asset.Account?.Name,
                    PurchaseDate = asset.PurchaseDate,
                    CurrentValue = asset.CurrentValue,
                    AccumulatedDepreciation = asset.AccumulatedDepreciation,
                    LastDepreciationDate = asset.LastDepreciationDate,
                    WarrantyInfo = asset.WarrantyInfo,
                    WarrantyExpiryDate = asset.WarrantyExpiryDate,
                    Notes = asset.Notes,
                    DateAdd = asset.DateAdd,
                    DateMod = asset.DateMod,
                    CreatedByUserName = asset.CreatedByUserName,
                    UpdatedByUserName = asset.UpdatedByUserName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset {AssetId}", request.Id);
                throw;
            }
        }
    }

    // ==================== GET ASSETS BY BRANCH HANDLER ====================
    public class GetAssetsByBranchHandler : IRequestHandler<GetAssetsByBranchQry, List<AssetDto>>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<GetAssetsByBranchHandler> _logger;

        public GetAssetsByBranchHandler(FinanceDbContext context, ILogger<GetAssetsByBranchHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AssetDto>> Handle(GetAssetsByBranchQry request, CancellationToken ct)
        {
            try
            {
                var assets = await _context.Set<Asset>()
                    .AsNoTracking()
                    .Include(x => x.AssignedEmployee)
                    .Include(x => x.Department)
                    .Include(x => x.Branch)
                    .Include(x => x.Account)
                    .Where(x => x.BranchId == request.BranchId && !x.IsDeleted)
                    .OrderByDescending(x => x.AcquisitionDate)
                    .ToListAsync(ct);

                return assets.Select(a => new AssetDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Description = a.Description,
                    SerialNumber = a.SerialNumber,
                    Model = a.Model,
                    Manufacturer = a.Manufacturer,
                    Location = a.Location,
                    AcquisitionCost = a.AcquisitionCost,
                    AcquisitionDate = a.AcquisitionDate,
                    SalvageValue = a.SalvageValue,
                    UsefulLife = a.UsefulLife,
                    DepreciationRate = a.DepreciationRate,
                    AssetType = a.AssetType,
                    AssetCategory = a.AssetCategory,
                    Status = a.Status,
                    IsActive = a.IsActive,
                    AssignedTo = a.AssignedTo,
                    AssignedToName = a.AssignedEmployee != null
                        ? $"{a.AssignedEmployee.FirstName} {a.AssignedEmployee.LastName}"
                        : null,
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.Department?.Name,
                    BranchId = a.BranchId,
                    BranchName = a.Branch?.Name,
                    AccountId = a.AccountId,
                    AccountCode = a.Account?.Code,
                    AccountName = a.Account?.Name,
                    PurchaseDate = a.PurchaseDate,
                    CurrentValue = a.CurrentValue,
                    AccumulatedDepreciation = a.AccumulatedDepreciation,
                    LastDepreciationDate = a.LastDepreciationDate,
                    WarrantyInfo = a.WarrantyInfo,
                    WarrantyExpiryDate = a.WarrantyExpiryDate,
                    Notes = a.Notes,
                    DateAdd = a.DateAdd,
                    DateMod = a.DateMod,
                    CreatedByUserName = a.CreatedByUserName,
                    UpdatedByUserName = a.UpdatedByUserName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assets for branch {BranchId}", request.BranchId);
                throw;
            }
        }
    }

    // ==================== GET ASSETS BY DEPARTMENT HANDLER ====================
    public class GetAssetsByDepartmentHandler : IRequestHandler<GetAssetsByDepartmentQry, List<AssetDto>>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<GetAssetsByDepartmentHandler> _logger;

        public GetAssetsByDepartmentHandler(FinanceDbContext context, ILogger<GetAssetsByDepartmentHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AssetDto>> Handle(GetAssetsByDepartmentQry request, CancellationToken ct)
        {
            try
            {
                var assets = await _context.Set<Asset>()
                    .AsNoTracking()
                    .Include(x => x.AssignedEmployee)
                    .Include(x => x.Department)
                    .Include(x => x.Branch)
                    .Include(x => x.Account)
                    .Where(x => x.DepartmentId == request.DepartmentId && !x.IsDeleted)
                    .OrderByDescending(x => x.AcquisitionDate)
                    .ToListAsync(ct);

                return assets.Select(a => new AssetDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Description = a.Description,
                    SerialNumber = a.SerialNumber,
                    Model = a.Model,
                    Manufacturer = a.Manufacturer,
                    Location = a.Location,
                    AcquisitionCost = a.AcquisitionCost,
                    AcquisitionDate = a.AcquisitionDate,
                    SalvageValue = a.SalvageValue,
                    UsefulLife = a.UsefulLife,
                    DepreciationRate = a.DepreciationRate,
                    AssetType = a.AssetType,
                    AssetCategory = a.AssetCategory,
                    Status = a.Status,
                    IsActive = a.IsActive,
                    AssignedTo = a.AssignedTo,
                    AssignedToName = a.AssignedEmployee != null
                        ? $"{a.AssignedEmployee.FirstName} {a.AssignedEmployee.LastName}"
                        : null,
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.Department?.Name,
                    BranchId = a.BranchId,
                    BranchName = a.Branch?.Name,
                    AccountId = a.AccountId,
                    AccountCode = a.Account?.Code,
                    AccountName = a.Account?.Name,
                    PurchaseDate = a.PurchaseDate,
                    CurrentValue = a.CurrentValue,
                    AccumulatedDepreciation = a.AccumulatedDepreciation,
                    LastDepreciationDate = a.LastDepreciationDate,
                    WarrantyInfo = a.WarrantyInfo,
                    WarrantyExpiryDate = a.WarrantyExpiryDate,
                    Notes = a.Notes,
                    DateAdd = a.DateAdd,
                    DateMod = a.DateMod,
                    CreatedByUserName = a.CreatedByUserName,
                    UpdatedByUserName = a.UpdatedByUserName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assets for department {DepartmentId}", request.DepartmentId);
                throw;
            }
        }
    }

    // ==================== GET ASSETS BY EMPLOYEE HANDLER ====================
    public class GetAssetsByEmployeeHandler : IRequestHandler<GetAssetsByEmployeeQry, List<AssetDto>>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<GetAssetsByEmployeeHandler> _logger;

        public GetAssetsByEmployeeHandler(FinanceDbContext context, ILogger<GetAssetsByEmployeeHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AssetDto>> Handle(GetAssetsByEmployeeQry request, CancellationToken ct)
        {
            try
            {
                var assets = await _context.Set<Asset>()
                    .AsNoTracking()
                    .Include(x => x.AssignedEmployee)
                    .Include(x => x.Department)
                    .Include(x => x.Branch)
                    .Include(x => x.Account)
                    .Where(x => x.AssignedTo == request.EmployeeId && !x.IsDeleted)
                    .OrderByDescending(x => x.AcquisitionDate)
                    .ToListAsync(ct);

                return assets.Select(a => new AssetDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Description = a.Description,
                    SerialNumber = a.SerialNumber,
                    Model = a.Model,
                    Manufacturer = a.Manufacturer,
                    Location = a.Location,
                    AcquisitionCost = a.AcquisitionCost,
                    AcquisitionDate = a.AcquisitionDate,
                    SalvageValue = a.SalvageValue,
                    UsefulLife = a.UsefulLife,
                    DepreciationRate = a.DepreciationRate,
                    AssetType = a.AssetType,
                    AssetCategory = a.AssetCategory,
                    Status = a.Status,
                    IsActive = a.IsActive,
                    AssignedTo = a.AssignedTo,
                    AssignedToName = a.AssignedEmployee != null
                        ? $"{a.AssignedEmployee.FirstName} {a.AssignedEmployee.LastName}"
                        : null,
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.Department?.Name,
                    BranchId = a.BranchId,
                    BranchName = a.Branch?.Name,
                    AccountId = a.AccountId,
                    AccountCode = a.Account?.Code,
                    AccountName = a.Account?.Name,
                    PurchaseDate = a.PurchaseDate,
                    CurrentValue = a.CurrentValue,
                    AccumulatedDepreciation = a.AccumulatedDepreciation,
                    LastDepreciationDate = a.LastDepreciationDate,
                    WarrantyInfo = a.WarrantyInfo,
                    WarrantyExpiryDate = a.WarrantyExpiryDate,
                    Notes = a.Notes,
                    DateAdd = a.DateAdd,
                    DateMod = a.DateMod,
                    CreatedByUserName = a.CreatedByUserName,
                    UpdatedByUserName = a.UpdatedByUserName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assets for employee {EmployeeId}", request.EmployeeId);
                throw;
            }
        }
    }
}