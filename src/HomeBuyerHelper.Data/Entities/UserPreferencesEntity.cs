using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for UserPreferences table.
/// </summary>
[Table("UserPreferences")]
public class UserPreferencesEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public bool HasCompletedOnboarding { get; set; }

    public int BuyingGoal { get; set; }

    public int PropertyCountRange { get; set; }

    public int HouseholdSize { get; set; }

    public bool HasChildren { get; set; }

    public bool HasPets { get; set; }

    public int WorkArrangement { get; set; }

    public bool PrioritizesLocation { get; set; }

    public bool PrioritizesSize { get; set; }

    public bool PrioritizesCondition { get; set; }

    public bool PrioritizesPrice { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    public bool UseDarkMode { get; set; }

    public bool EnableNotifications { get; set; }

    public decimal DefaultDownPaymentPercent { get; set; }

    public decimal DefaultInterestRate { get; set; }

    public int DefaultMortgageTerm { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
