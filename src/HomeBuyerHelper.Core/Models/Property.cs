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

    // Score-related properties (populated when loading with scores)

    /// <summary>
    /// Overall weighted score (0-10 scale).
    /// </summary>
    public decimal OverallScore { get; set; }

    /// <summary>
    /// Rank among all properties (1 = best).
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// Number of criteria that have been scored.
    /// </summary>
    public int ScoredCriteriaCount { get; set; }

    /// <summary>
    /// Total number of criteria to score.
    /// </summary>
    public int TotalCriteriaCount { get; set; }

    /// <summary>
    /// Score completion percentage (0-100).
    /// </summary>
    public int ScoreCompletionPercent =>
        TotalCriteriaCount > 0 ? (int)(ScoredCriteriaCount * 100.0 / TotalCriteriaCount) : 0;

    /// <summary>
    /// Whether all criteria have been scored.
    /// </summary>
    public bool IsFullyScored => TotalCriteriaCount > 0 && ScoredCriteriaCount >= TotalCriteriaCount;

    /// <summary>
    /// Color for score display (green for 8+, yellow for 5-7, red for 1-4).
    /// </summary>
    public string ScoreColor =>
        OverallScore >= 8 ? "#4CAF50" :
        OverallScore >= 5 ? "#FF9800" :
        "#F44336";

    /// <summary>
    /// List of individual criterion scores.
    /// </summary>
    public List<PropertyScore> Scores { get; set; } = new();
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
