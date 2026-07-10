using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for the OfferScenarios table.
/// </summary>
[Table("OfferScenarios")]
public class OfferScenarioEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int PropertyId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public decimal OfferPrice { get; set; }

    public decimal? EscalationMaxPrice { get; set; }

    public decimal DownPaymentPercent { get; set; }

    public decimal InterestRate { get; set; }

    public int TermYears { get; set; }

    public decimal DiscountPoints { get; set; }

    public decimal SellerCredit { get; set; }

    public decimal LenderCredit { get; set; }

    public decimal EarnestMoney { get; set; }

    public bool WaiveInspection { get; set; }

    public bool WaiveFinancing { get; set; }

    public bool WaiveAppraisal { get; set; }

    public int ClosingDays { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
