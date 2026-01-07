namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a property being evaluated for purchase.
/// </summary>
public class Property
{
    public int Id { get; set; }

    /// <summary>
    /// User-friendly name for the property (e.g., "Blue house on Oak St").
    /// </summary>
    public required string Nickname { get; set; }

    /// <summary>
    /// Full street address of the property.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City where the property is located.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State/Province where the property is located.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// ZIP/Postal code.
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Listing price in USD.
    /// </summary>
    public decimal AskingPrice { get; set; }

    /// <summary>
    /// User's offer price, if different from asking.
    /// </summary>
    public decimal? OfferPrice { get; set; }

    /// <summary>
    /// Number of bedrooms.
    /// </summary>
    public int Bedrooms { get; set; }

    /// <summary>
    /// Number of bathrooms (supports half baths like 2.5).
    /// </summary>
    public decimal Bathrooms { get; set; }

    /// <summary>
    /// Total square footage of living space.
    /// </summary>
    public int SquareFeet { get; set; }

    /// <summary>
    /// Lot size in square feet.
    /// </summary>
    public int? LotSquareFeet { get; set; }

    /// <summary>
    /// Year the property was built.
    /// </summary>
    public int? YearBuilt { get; set; }

    /// <summary>
    /// Property type (Single Family, Condo, Townhouse, etc.).
    /// </summary>
    public PropertyType PropertyType { get; set; } = PropertyType.SingleFamily;

    /// <summary>
    /// Monthly HOA fee, if applicable.
    /// </summary>
    public decimal MonthlyHOA { get; set; }

    /// <summary>
    /// Annual property tax amount.
    /// </summary>
    public decimal? AnnualPropertyTax { get; set; }

    /// <summary>
    /// Estimated annual insurance cost.
    /// </summary>
    public decimal? AnnualInsurance { get; set; }

    /// <summary>
    /// Optional URL to the property listing.
    /// </summary>
    public string? ListingUrl { get; set; }

    /// <summary>
    /// User notes about the property.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this property is marked as a favorite.
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Whether this property has been archived/dismissed.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// When the property was added to the app.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the property was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Calculates the effective price (offer price if set, otherwise asking price).
    /// </summary>
    public decimal EffectivePrice => OfferPrice ?? AskingPrice;

    /// <summary>
    /// Calculates the price per square foot.
    /// </summary>
    public decimal PricePerSquareFoot => SquareFeet > 0 ? EffectivePrice / SquareFeet : 0;

    /// <summary>
    /// Calculates the estimated monthly property tax.
    /// </summary>
    public decimal MonthlyPropertyTax => (AnnualPropertyTax ?? 0) / 12;

    /// <summary>
    /// Calculates the estimated monthly insurance.
    /// </summary>
    public decimal MonthlyInsurance => (AnnualInsurance ?? 0) / 12;
}

/// <summary>
/// Types of properties that can be evaluated.
/// </summary>
public enum PropertyType
{
    SingleFamily,
    Condo,
    Townhouse,
    MultiFamily,
    Manufactured,
    Land,
    Other
}
