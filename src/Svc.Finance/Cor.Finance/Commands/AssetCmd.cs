// Commands/AssetCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cor.Finance.Queries;
namespace Cor.Finance.Commands
{
    // ==================== CREATE ASSET COMMAND ====================
    public class CreateAssetCmd : IRequest<AssetDto>
    {
        public CreateAssetDto Dto { get; set; } = new();
    }

    public class CreateAssetHandler : IRequestHandler<CreateAssetCmd, AssetDto>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<CreateAssetHandler> _logger;

        public CreateAssetHandler(FinanceDbContext context, ILogger<CreateAssetHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AssetDto> Handle(CreateAssetCmd request, CancellationToken ct)
        {
            try
            {
                var dto = request.Dto;

                var asset = new Asset
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Code = dto.Code,
                    Description = dto.Description,
                    SerialNumber = dto.SerialNumber,
                    Model = dto.Model,
                    Manufacturer = dto.Manufacturer,
                    Location = dto.Location,
                    AcquisitionCost = dto.AcquisitionCost,
                    AcquisitionDate = dto.AcquisitionDate,
                    SalvageValue = dto.SalvageValue,
                    UsefulLife = dto.UsefulLife,
                    DepreciationRate = dto.DepreciationRate,
                    AssetType = dto.AssetType,
                    AssetCategory = dto.AssetCategory,
                    Status = dto.Status ?? "Active",
                    IsActive = true,
                    AssignedTo = dto.AssignedTo,
                    DepartmentId = dto.DepartmentId,
                    BranchId = dto.BranchId,
                    AccountId = dto.AccountId,
                    PurchaseDate = dto.PurchaseDate,
                    WarrantyInfo = dto.WarrantyInfo,
                    WarrantyExpiryDate = dto.WarrantyExpiryDate,
                    Notes = dto.Notes,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.Set<Asset>().Add(asset);
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("✅ Created asset: {AssetName} ({AssetId})", asset.Name, asset.Id);

                // Return the created asset
               var getAssetHandler = new GetAssetByIdHandler(_context,
                   new Logger<GetAssetByIdHandler>(new LoggerFactory()));
                return await getAssetHandler.Handle(new GetAssetByIdQry { Id = asset.Id }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset");
                throw;
            }
        }
    }

    // ==================== UPDATE ASSET COMMAND ====================
    public class UpdateAssetCmd : IRequest<AssetDto>
    {
        public UpdateAssetDto Dto { get; set; } = new();
    }

    public class UpdateAssetHandler : IRequestHandler<UpdateAssetCmd, AssetDto>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<UpdateAssetHandler> _logger;

        public UpdateAssetHandler(FinanceDbContext context, ILogger<UpdateAssetHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AssetDto> Handle(UpdateAssetCmd request, CancellationToken ct)
        {
            try
            {
                var dto = request.Dto;

                var asset = await _context.Set<Asset>()
                    .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct);

                if (asset == null)
                    throw new InvalidOperationException($"Asset with ID '{dto.Id}' not found");

                // Update fields
                asset.Name = dto.Name;
                asset.Code = dto.Code;
                asset.Description = dto.Description;
                asset.SerialNumber = dto.SerialNumber;
                asset.Model = dto.Model;
                asset.Manufacturer = dto.Manufacturer;
                asset.Location = dto.Location;
                asset.AcquisitionCost = dto.AcquisitionCost;
                asset.AcquisitionDate = dto.AcquisitionDate;
                asset.SalvageValue = dto.SalvageValue;
                asset.UsefulLife = dto.UsefulLife;
                asset.DepreciationRate = dto.DepreciationRate;
                asset.AssetType = dto.AssetType;
                asset.AssetCategory = dto.AssetCategory;
                asset.Status = dto.Status ?? asset.Status;
                asset.IsActive = dto.IsActive;
                asset.AssignedTo = dto.AssignedTo;
                asset.DepartmentId = dto.DepartmentId;
                asset.BranchId = dto.BranchId;
                asset.AccountId = dto.AccountId;
                asset.PurchaseDate = dto.PurchaseDate;
                asset.CurrentValue = dto.CurrentValue;
                asset.AccumulatedDepreciation = dto.AccumulatedDepreciation;
                asset.LastDepreciationDate = dto.LastDepreciationDate;
                asset.WarrantyInfo = dto.WarrantyInfo;
                asset.WarrantyExpiryDate = dto.WarrantyExpiryDate;
                asset.Notes = dto.Notes;
                asset.DateMod = DateTime.UtcNow;

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("✅ Updated asset: {AssetName} ({AssetId})", asset.Name, asset.Id);

                // Return the updated asset
              var getAssetHandler = new GetAssetByIdHandler(_context,
                  new Logger<GetAssetByIdHandler>(new LoggerFactory()));
                return await getAssetHandler.Handle(new GetAssetByIdQry { Id = asset.Id }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset {AssetId}", request.Dto.Id);
                throw;
            }
        }
    }

    // ==================== DELETE ASSET COMMAND ====================
    public class DeleteAssetCmd : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteAssetHandler : IRequestHandler<DeleteAssetCmd, bool>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<DeleteAssetHandler> _logger;

        public DeleteAssetHandler(FinanceDbContext context, ILogger<DeleteAssetHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteAssetCmd request, CancellationToken ct)
        {
            try
            {
                var asset = await _context.Set<Asset>()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

                if (asset == null)
                    return false;

                // Soft delete
                asset.IsDeleted = true;
                asset.DateMod = DateTime.UtcNow;
                asset.Status = "Disposed";

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("✅ Deleted asset: {AssetName} ({AssetId})", asset.Name, asset.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset {AssetId}", request.Id);
                throw;
            }
        }
    }

    // ==================== TOGGLE ASSET STATUS COMMAND ====================
    public class ToggleAssetStatusCmd : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ToggleAssetStatusHandler : IRequestHandler<ToggleAssetStatusCmd, bool>
    {
        private readonly FinanceDbContext _context;
        private readonly ILogger<ToggleAssetStatusHandler> _logger;

        public ToggleAssetStatusHandler(FinanceDbContext context, ILogger<ToggleAssetStatusHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(ToggleAssetStatusCmd request, CancellationToken ct)
        {
            try
            {
                var asset = await _context.Set<Asset>()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

                if (asset == null)
                    return false;

                asset.IsActive = !asset.IsActive;
                asset.Status = asset.IsActive ? "Active" : "Inactive";
                asset.DateMod = DateTime.UtcNow;

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("✅ Toggled asset status: {AssetName} ({AssetId}) -> {Status}",
                    asset.Name, asset.Id, asset.Status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling asset status {AssetId}", request.Id);
                throw;
            }
        }
    }
}