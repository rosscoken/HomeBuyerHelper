using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for Property table.
/// </summary>
[Table("Properties")]
public class PropertyEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Nickname { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    public decimal AskingPrice { get; set; }

    public decimal? OfferPrice { get; set; }

    public int Bedrooms { get; set; }

    public decimal Bathrooms { get; set; }

    public int SquareFeet { get; set; }

    public int? LotSquareFeet { get; set; }

    public int? YearBuilt { get; set; }

    public int PropertyType { get; set; }

    public decimal MonthlyHOA { get; set; }

    public decimal? AnnualPropertyTax { get; set; }

    public decimal? AnnualInsurance { get; set; }

    [MaxLength(1000)]
    public string? ListingUrl { get; set; }

    public string? Notes { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
