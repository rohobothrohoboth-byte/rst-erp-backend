// Cor.CRM/Commands/PropertyCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Helpers;
using MediatR;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

public class PropertyAddCmd : IRequest<PropertyDto>
{
    public CreatePropertyDto Dto { get; set; } = default!;
}

public class PropertyUpdateCmd : IRequest<PropertyDto>
{
    public Guid Id { get; set; }
    public UpdatePropertyDto Dto { get; set; } = default!;
}

public class PropertyDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class PropertyPublishCmd : IRequest<PropertyDto>
{
    public Guid Id { get; set; }
}

public class PropertyAddHandler : IRequestHandler<PropertyAddCmd, PropertyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public PropertyAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<PropertyDto> Handle(PropertyAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var property = new Property
            {
                Id = Guid.CreateVersion7(),
                Title = request.Dto.Title,
                Description = request.Dto.Description,
                Type = (PropertyType)request.Dto.Type,
                Address = request.Dto.Address,
                City = request.Dto.City,
                State = request.Dto.State,
                PostalCode = request.Dto.PostalCode,
                Country = request.Dto.Country,
                Latitude = request.Dto.Latitude,
                Longitude = request.Dto.Longitude,
                Bedrooms = request.Dto.Bedrooms,
                Bathrooms = request.Dto.Bathrooms,
                HalfBathrooms = request.Dto.HalfBathrooms,
                LandSize = request.Dto.LandSize,
                BuildingSize = request.Dto.BuildingSize,
                YearBuilt = request.Dto.YearBuilt,
                Price = request.Dto.Price,
                Status = (PropertyStatus)request.Dto.Status,
                OwnerId = request.Dto.OwnerId,
                ListingAgentId = request.Dto.ListingAgentId,
                FeaturesJson = request.Dto.Features != null && request.Dto.Features.Any()
                    ? JsonSerializer.Serialize(request.Dto.Features)
                    : null,
                MainImageUrl = request.Dto.MainImageUrl,
                ImagesJson = request.Dto.Images != null && request.Dto.Images.Any()
                    ? JsonSerializer.Serialize(request.Dto.Images)
                    : null,
                VirtualTourUrl = request.Dto.VirtualTourUrl,
                VideoUrl = request.Dto.VideoUrl,
                ListingDate = request.Dto.ListingDate ?? DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsFeatured = request.Dto.IsFeatured,
                IsPublished = request.Dto.IsPublished,
                MarketingDescription = request.Dto.MarketingDescription,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            await _uow.Add(property, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Property created: {PropertyId} - {Title}", property.Id, property.Title);

            return MapToDto(property);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private PropertyDto MapToDto(Property property)
    {
        var dto = new PropertyDto
        {
            Id = property.Id,
            Title = property.Title,
            Description = property.Description,
            Type = property.Type.ToString(),
            Address = property.Address,
            City = property.City,
            State = property.State,
            PostalCode = property.PostalCode,
            Country = property.Country,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            HalfBathrooms = property.HalfBathrooms,
            LandSize = property.LandSize,
            BuildingSize = property.BuildingSize,
            YearBuilt = property.YearBuilt,
            Price = property.Price,
            Status = property.Status.ToString(),
            OwnerId = property.OwnerId,
            ListingAgentId = property.ListingAgentId,
            MainImageUrl = property.MainImageUrl,
            VirtualTourUrl = property.VirtualTourUrl,
            VideoUrl = property.VideoUrl,
            ListingDate = property.ListingDate,
            SoldDate = property.SoldDate,
            IsFeatured = property.IsFeatured,
            IsPublished = property.IsPublished,
            ViewCount = property.ViewCount,
            InquiryCount = property.InquiryCount,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };

        // Parse Features
        if (!string.IsNullOrEmpty(property.FeaturesJson))
        {
            try
            {
                dto.Features = JsonSerializer.Deserialize<List<string>>(property.FeaturesJson);
            }
            catch { dto.Features = new List<string>(); }
        }

        // Parse Images
        if (!string.IsNullOrEmpty(property.ImagesJson))
        {
            try
            {
                dto.Images = JsonSerializer.Deserialize<List<string>>(property.ImagesJson);
            }
            catch { dto.Images = new List<string>(); }
        }

        return dto;
    }
}

public class PropertyUpdateHandler : IRequestHandler<PropertyUpdateCmd, PropertyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public PropertyUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<PropertyDto> Handle(PropertyUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var property = await _uow.Set<Property>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (property == null)
                throw new DomainException($"Property with id [{request.Id}] NOT FOUND.");

            // Update fields
            if (!string.IsNullOrEmpty(request.Dto.Title))
                property.Title = request.Dto.Title;

            if (request.Dto.Description != null)
                property.Description = request.Dto.Description;

            if (request.Dto.Type.HasValue)
                property.Type = (PropertyType)request.Dto.Type.Value;

            if (!string.IsNullOrEmpty(request.Dto.Address))
                property.Address = request.Dto.Address;

            if (request.Dto.City != null)
                property.City = request.Dto.City;

            if (request.Dto.State != null)
                property.State = request.Dto.State;

            if (request.Dto.PostalCode != null)
                property.PostalCode = request.Dto.PostalCode;

            if (request.Dto.Country != null)
                property.Country = request.Dto.Country;

            if (request.Dto.Latitude != null)
                property.Latitude = request.Dto.Latitude;

            if (request.Dto.Longitude != null)
                property.Longitude = request.Dto.Longitude;

            if (request.Dto.Bedrooms.HasValue)
                property.Bedrooms = request.Dto.Bedrooms;

            if (request.Dto.Bathrooms.HasValue)
                property.Bathrooms = request.Dto.Bathrooms;

            if (request.Dto.HalfBathrooms.HasValue)
                property.HalfBathrooms = request.Dto.HalfBathrooms;

            if (request.Dto.LandSize.HasValue)
                property.LandSize = request.Dto.LandSize;

            if (request.Dto.BuildingSize.HasValue)
                property.BuildingSize = request.Dto.BuildingSize;

            if (request.Dto.YearBuilt.HasValue)
                property.YearBuilt = request.Dto.YearBuilt;

            if (request.Dto.Price.HasValue)
                property.Price = request.Dto.Price.Value;

            if (request.Dto.Status.HasValue)
                property.Status = (PropertyStatus)request.Dto.Status.Value;

            if (request.Dto.OwnerId.HasValue)
                property.OwnerId = request.Dto.OwnerId;

            if (request.Dto.ListingAgentId.HasValue)
                property.ListingAgentId = request.Dto.ListingAgentId;

            if (request.Dto.Features != null)
                property.FeaturesJson = request.Dto.Features.Any()
                    ? JsonSerializer.Serialize(request.Dto.Features)
                    : null;

            if (request.Dto.MainImageUrl != null)
                property.MainImageUrl = request.Dto.MainImageUrl;

            if (request.Dto.Images != null)
                property.ImagesJson = request.Dto.Images.Any()
                    ? JsonSerializer.Serialize(request.Dto.Images)
                    : null;

            if (request.Dto.VirtualTourUrl != null)
                property.VirtualTourUrl = request.Dto.VirtualTourUrl;

            if (request.Dto.VideoUrl != null)
                property.VideoUrl = request.Dto.VideoUrl;

            if (request.Dto.ListingDate.HasValue)
                property.ListingDate = DateTimeHelper.EnsureUtc(request.Dto.ListingDate.Value);

            if (request.Dto.IsFeatured.HasValue)
                property.IsFeatured = request.Dto.IsFeatured.Value;

            if (request.Dto.IsPublished.HasValue)
                property.IsPublished = request.Dto.IsPublished.Value;

            if (request.Dto.MarketingDescription != null)
                property.MarketingDescription = request.Dto.MarketingDescription;

            property.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(property);
            await _uow.Commit(ct);

            _logger.LogInformation("Property updated: {PropertyId} - {Title}", property.Id, property.Title);

            return MapToDto(property);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private PropertyDto MapToDto(Property property)
    {
        var dto = new PropertyDto
        {
            Id = property.Id,
            Title = property.Title,
            Description = property.Description,
            Type = property.Type.ToString(),
            Address = property.Address,
            City = property.City,
            State = property.State,
            PostalCode = property.PostalCode,
            Country = property.Country,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            HalfBathrooms = property.HalfBathrooms,
            LandSize = property.LandSize,
            BuildingSize = property.BuildingSize,
            YearBuilt = property.YearBuilt,
            Price = property.Price,
            Status = property.Status.ToString(),
            OwnerId = property.OwnerId,
            ListingAgentId = property.ListingAgentId,
            MainImageUrl = property.MainImageUrl,
            VirtualTourUrl = property.VirtualTourUrl,
            VideoUrl = property.VideoUrl,
            ListingDate = property.ListingDate,
            SoldDate = property.SoldDate,
            IsFeatured = property.IsFeatured,
            IsPublished = property.IsPublished,
            ViewCount = property.ViewCount,
            InquiryCount = property.InquiryCount,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };

        if (!string.IsNullOrEmpty(property.FeaturesJson))
        {
            try
            {
                dto.Features = JsonSerializer.Deserialize<List<string>>(property.FeaturesJson);
            }
            catch { dto.Features = new List<string>(); }
        }

        if (!string.IsNullOrEmpty(property.ImagesJson))
        {
            try
            {
                dto.Images = JsonSerializer.Deserialize<List<string>>(property.ImagesJson);
            }
            catch { dto.Images = new List<string>(); }
        }

        return dto;
    }
}

public class PropertyDelHandler : IRequestHandler<PropertyDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public PropertyDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(PropertyDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var property = await _uow.Set<Property>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (property == null)
                throw new DomainException($"Property with id [{request.Id}] NOT FOUND.");

            property.IsDeleted = true;
            property.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(property);
            await _uow.Commit(ct);

            _logger.LogInformation("Property deleted: {PropertyId} - {Title}", property.Id, property.Title);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class PropertyPublishHandler : IRequestHandler<PropertyPublishCmd, PropertyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public PropertyPublishHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<PropertyDto> Handle(PropertyPublishCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var property = await _uow.Set<Property>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (property == null)
                throw new DomainException($"Property with id [{request.Id}] NOT FOUND.");

            property.IsPublished = true;
            property.ListingDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            property.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(property);
            await _uow.Commit(ct);

            _logger.LogInformation("Property published: {PropertyId} - {Title}", property.Id, property.Title);

            return MapToDto(property);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private PropertyDto MapToDto(Property property)
    {
        return new PropertyDto
        {
            Id = property.Id,
            Title = property.Title,
            Description = property.Description,
            Type = property.Type.ToString(),
            Address = property.Address,
            City = property.City,
            State = property.State,
            PostalCode = property.PostalCode,
            Country = property.Country,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            HalfBathrooms = property.HalfBathrooms,
            LandSize = property.LandSize,
            BuildingSize = property.BuildingSize,
            YearBuilt = property.YearBuilt,
            Price = property.Price,
            Status = property.Status.ToString(),
            OwnerId = property.OwnerId,
            ListingAgentId = property.ListingAgentId,
            MainImageUrl = property.MainImageUrl,
            VirtualTourUrl = property.VirtualTourUrl,
            VideoUrl = property.VideoUrl,
            ListingDate = property.ListingDate,
            SoldDate = property.SoldDate,
            IsFeatured = property.IsFeatured,
            IsPublished = property.IsPublished,
            ViewCount = property.ViewCount,
            InquiryCount = property.InquiryCount,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}