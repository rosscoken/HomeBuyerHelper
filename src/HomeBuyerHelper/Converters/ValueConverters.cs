using System.Globalization;

namespace HomeBuyerHelper.Converters;

/// <summary>
/// Theme-aware color lookup for converters, which can't use AppThemeBinding.
/// Every hex used through this helper must have a matching token in
/// Resources/Styles/Colors.xaml; update both together to avoid drift.
/// </summary>
internal static class ThemeColors
{
    internal static Color Pick(string light, string dark) =>
        Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb(dark)
            : Color.FromArgb(light);
}

/// <summary>
/// Inverts a boolean value.
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return value;
    }
}

/// <summary>
/// Returns true if the value is not null.
/// </summary>
public class IsNotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Compares value to parameter for equality.
/// </summary>
public class EqualToConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null && parameter == null)
            return true;
        if (value == null || parameter == null)
            return false;

        // Handle int comparison
        if (int.TryParse(parameter.ToString(), out var intParam) && value is int intValue)
            return intValue == intParam;

        return value.Equals(parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Returns true if value is greater than parameter.
/// </summary>
public class GreaterThanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue && int.TryParse(parameter?.ToString(), out var intParam))
            return intValue > intParam;
        if (value is decimal decimalValue && decimal.TryParse(parameter?.ToString(), out var decimalParam))
            return decimalValue > decimalParam;
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a favorite status to an icon source.
/// </summary>
public class FavoriteIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Using text since we don't have actual icons yet
        // In production, this would return image sources
        return value is true ? "heart_filled.png" : "heart_outline.png";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts percentage (0-100) to decimal (0-1) for ProgressBar.
/// </summary>
public class PercentToDecimalConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
            return (double)(decimalValue / 100);
        if (value is double doubleValue)
            return doubleValue / 100;
        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Shows "Suggested" text for system-suggested criteria.
/// </summary>
public class SystemSuggestedConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "(Suggested)" : "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts onboarding step to button text.
/// </summary>
public class OnboardingButtonTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int step)
            return step == 6 ? "Complete" : "Next";
        return "Next";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Returns appropriate command based on onboarding step.
/// Note: This is a simplified converter - in production, use DataTriggers or VisualStateManager.
/// </summary>
public class OnboardingButtonCommandConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // This converter is a placeholder - the actual implementation
        // would need access to the ViewModel commands
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts boolean to background color for selection states.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return ThemeColors.Pick("#EEEAFE", "#2A2350"); // SurfaceVariant / selected
        }
        return ThemeColors.Pick("#FFFFFF", "#1A1726"); // Surface / unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts boolean to stroke color for selection states.
/// </summary>
public class BoolToStrokeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return ThemeColors.Pick("#5B40E8", "#8B72FF"); // Primary / selected
        }
        return ThemeColors.Pick("#E6E3F2", "#3A3554"); // Gray300 / unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts boolean (IsFullyScored) to score button text.
/// </summary>
public class BoolToScoreButtonTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "Edit Scores" : "Score";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a score (0-10) to a color based on thresholds.
/// Green: 8-10, Yellow: 5-7, Red: 1-4
/// </summary>
public class ScoreToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        decimal score = value switch
        {
            decimal d => d,
            int i => i,
            double dbl => (decimal)dbl,
            _ => 0
        };

        if (score >= 8)
            return Color.FromArgb("#0E9F6E"); // Success green
        if (score >= 5)
            return Color.FromArgb("#D97706"); // Warning yellow/orange
        return Color.FromArgb("#E11D48"); // Error red
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts boolean (IsLastCriterion) to button text (Next/Finish).
/// </summary>
public class BoolToNextFinishConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "Finish" : "Next";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts boolean (IsSelectingProperties) to button text (Done/Select).
/// </summary>
public class BoolToEditSelectConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "Done" : "Select";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a numeric value to green (positive/zero) or red (negative).
/// Used for surplus/deficit display.
/// </summary>
public class PositiveNegativeColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isNegative = value switch
        {
            decimal d => d < 0,
            int i => i < 0,
            double db => db < 0,
            _ => false
        };
        return isNegative ? Color.FromArgb("#E11D48") : Color.FromArgb("#0E9F6E");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a warning flag to red (true) or gray (false) text color.
/// </summary>
public class BoolToAlertColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool warning && warning
            ? ThemeColors.Pick("#E11D48", "#FB7185") // Error
            : ThemeColors.Pick("#8E89A8", "#777291"); // TextDisabled
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Joins a list of strings with commas for display.
/// </summary>
public class ListToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<string> items)
            return string.Join(", ", items);
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Maps an affordability zone to its display color.
/// </summary>
public class AffordabilityZoneToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Core.Interfaces.AffordabilityZone zone)
        {
            return zone switch
            {
                Core.Interfaces.AffordabilityZone.Comfortable => Color.FromArgb("#0E9F6E"),
                Core.Interfaces.AffordabilityZone.Stretching => Color.FromArgb("#F59E0B"),
                Core.Interfaces.AffordabilityZone.Aggressive => Color.FromArgb("#D97706"),
                _ => Color.FromArgb("#E11D48")
            };
        }
        return Color.FromArgb("#8E89A8");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts the "buying is ahead" flag to a Buy/Rent label.
/// </summary>
public class BoolToBuyRentConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool buyAhead && buyAhead ? "Buy" : "Rent";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
