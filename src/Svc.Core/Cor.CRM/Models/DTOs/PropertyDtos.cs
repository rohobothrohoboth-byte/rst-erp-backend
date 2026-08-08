// Cor.CRM/Models/DTOs/PropertyDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? HalfBathrooms { get; set; }
    public decimal? LandSize { get; set; }
    public decimal? BuildingSize { get; set; }
    public int? YearBuilt { get; set; }
    public decimal Price { get; set; }
    public decimal? PricePerSquareFoot { get; set; }
    public decimal? ListedPrice { get; set; }
    public decimal? SoldPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public Guid? ListingAgentId { get; set; }
    public string? ListingAgentName { get; set; }
    public Guid? BuyingAgentId { get; set; }
    public string? BuyingAgentName { get; set; }

    public string? VirtualTourUrl { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime? ListingDate { get; set; }
    public DateTime? SoldDate { get; set; }

     public List<string>? Features { get; set; }
        public string? FeaturesJson { get; set; }  // Raw JSON string from database
        public string? MainImageUrl { get; set; }
        public List<string>? Images { get; set; }
        public string? ImagesJson { get; set; }    // Raw JSON string from database

    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public int InquiryCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePropertyDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Type { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? HalfBathrooms { get; set; }
    public decimal? LandSize { get; set; }
    public decimal? BuildingSize { get; set; }
    public int? YearBuilt { get; set; }
    public decimal Price { get; set; }
    public int Status { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? ListingAgentId { get; set; }
    public List<int>? Features { get; set; }
    public string? MainImageUrl { get; set; }
    public List<string>? Images { get; set; }
    public string? VirtualTourUrl { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime? ListingDate { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public string? MarketingDescription { get; set; }
}

public class UpdatePropertyDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Type { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? HalfBathrooms { get; set; }
    public decimal? LandSize { get; set; }
    public decimal? BuildingSize { get; set; }
    public int? YearBuilt { get; set; }
    public decimal? Price { get; set; }
    public int? Status { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? ListingAgentId { get; set; }
    public List<int>? Features { get; set; }
    public string? MainImageUrl { get; set; }
    public List<string>? Images { get; set; }
    public string? VirtualTourUrl { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime? ListingDate { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsPublished { get; set; }
    public string? MarketingDescription { get; set; }
}

public class PropertyFilterDto
{
    public string? Search { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; }
    public int? MinBathrooms { get; set; }
    public int? MaxBathrooms { get; set; }
    public decimal? MinLandSize { get; set; }
    public decimal? MaxLandSize { get; set; }
    public decimal? MinBuildingSize { get; set; }
    public decimal? MaxBuildingSize { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public Guid? ListingAgentId { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsPublished { get; set; }
    public DateTime? ListedFrom { get; set; }
    public DateTime? ListedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}