// Cor.CRM/Models/Entities/Property.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.CRM.Models.Entities.Local;
namespace Cor.CRM.Models.Entities;

public enum PropertyType
{
    Residential = 1,
    Commercial = 2,
    Land = 3,
    Industrial = 4,
    Agricultural = 5,
    MixedUse = 6
}

public enum PropertyStatus
{
    Active = 1,
    Pending = 2,
    Sold = 3,
    Rented = 4,
    OffMarket = 5,
    UnderConstruction = 6
}

public enum PropertyFeature
{
    Pool = 1,
    Garage = 2,
    Parking = 3,
    Garden = 4,
    Balcony = 5,
    Elevator = 6,
    Security = 7,
    Gym = 8,
    AirConditioning = 9,
    Heating = 10,
    Fireplace = 11,
    Basement = 12,
    Attic = 13,
    Waterfront = 14,
    MountainView = 15
}

public class Property : BaseEntity
{
    // Basic Info
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    // Property Type
    public PropertyType Type { get; set; }

    // Address
    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    // Coordinates
    [MaxLength(50)]
    public string? Latitude { get; set; }

    [MaxLength(50)]
    public string? Longitude { get; set; }

    // Property Details
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? HalfBathrooms { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? LandSize { get; set; } // Square footage of land

    [Column(TypeName = "decimal(12,2)")]
    public decimal? BuildingSize { get; set; } // Square footage of building

    public int? YearBuilt { get; set; }

    // Pricing
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PricePerSquareFoot { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ListedPrice { get; set; } // Original listing price

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SoldPrice { get; set; }

    // Status
    public PropertyStatus Status { get; set; } = PropertyStatus.Active;

    // Owner
    public Guid? OwnerId { get; set; } // Customer who owns the property
    [ForeignKey(nameof(OwnerId))]
    public virtual Customer? Owner { get; set; }

    // Listing Agent
    public Guid? ListingAgentId { get; set; }
    [ForeignKey(nameof(ListingAgentId))]
    public virtual LocalEmployee? ListingAgent { get; set; }

    // Buying Agent
    public Guid? BuyingAgentId { get; set; }
    [ForeignKey(nameof(BuyingAgentId))]
    public virtual LocalEmployee? BuyingAgent { get; set; }

    // Property Features (JSON)
    public string? FeaturesJson { get; set; } // Array of features

    // Media
    [MaxLength(500)]
    public string? MainImageUrl { get; set; }

    public string? ImagesJson { get; set; } // Array of image URLs

    [MaxLength(500)]
    public string? VirtualTourUrl { get; set; }

    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    // Documents
    public string? DocumentsJson { get; set; }

    // Listing Dates
    public DateTime? ListingDate { get; set; }
    public DateTime? SoldDate { get; set; }
    public DateTime? RentedDate { get; set; }
    public DateTime? OffMarketDate { get; set; }

    // Marketing
    [MaxLength(500)]
    public string? MarketingDescription { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }

    // View Count
    public int ViewCount { get; set; }
    public int InquiryCount { get; set; }

    // Navigation Properties
    public virtual ICollection<PropertyInquiry> Inquiries { get; set; } = new List<PropertyInquiry>();
    public virtual ICollection<PropertyViewing> Viewings { get; set; } = new List<PropertyViewing>();
    public virtual ICollection<RealEstateTransaction> Transactions { get; set; } = new List<RealEstateTransaction>();
}