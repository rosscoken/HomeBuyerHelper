using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Commute time value analysis per design spec section 2.5:
/// Monthly Value = (Round-Trip Minutes x Workdays/Month x Hourly Rate) / 60.
/// </summary>
public class CommuteValueService : ICommuteValueService
{
    public decimal CalculateMonthlyValue(int roundTripMinutes, int workdaysPerMonth, decimal hourlyRate)
    {
        if (roundTripMinutes <= 0 || workdaysPerMonth <= 0 || hourlyRate <= 0)
            return 0;

        return Math.Round(roundTripMinutes * workdaysPerMonth * hourlyRate / 60m, 2);
    }

    public CommuteAnalysis Analyze(Property property, UserPreferences preferences)
    {
        var primary = property.CommuteMinutesPrimary ?? 0;
        var secondary = property.CommuteMinutesSecondary ?? 0;
        var total = primary + secondary;

        if (total <= 0)
        {
            return new CommuteAnalysis
            {
                HasCommuteData = false,
                Zone = CommuteZone.Excellent
            };
        }

        var monthly = CalculateMonthlyValue(total, preferences.WorkdaysPerMonth, preferences.TimeValueHourlyRate);
        var hoursPerYear = Math.Round(total * preferences.WorkdaysPerMonth * 12m / 60m, 1);

        return new CommuteAnalysis
        {
            HasCommuteData = true,
            TotalRoundTripMinutes = total,
            MonthlyValue = monthly,
            AnnualValue = Math.Round(monthly * 12, 2),
            ThirtyYearValue = Math.Round(monthly * 12 * 30, 2),
            HoursPerYear = hoursPerYear,
            DaysPerYear = Math.Round(hoursPerYear / 8m, 1),
            Zone = GetZone(primary > 0 ? primary : total)
        };
    }

    private static CommuteZone GetZone(int roundTripMinutes) => roundTripMinutes switch
    {
        < 30 => CommuteZone.Excellent,
        <= 60 => CommuteZone.Moderate,
        <= 90 => CommuteZone.Long,
        _ => CommuteZone.VeryLong
    };
}
