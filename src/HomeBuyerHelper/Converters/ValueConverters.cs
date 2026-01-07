using System.Globalization;

namespace HomeBuyerHelper.Converters;

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
            return Color.FromArgb("#F0EDF6"); // SurfaceVariant / selected
        }
        return Color.FromArgb("#FFFFFF"); // Surface / unselected
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
            return Color.FromArgb("#512BD4"); // Primary / selected
        }
        return Color.FromArgb("#E0E0E0"); // Gray300 / unselected
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
            return Color.FromArgb("#4CAF50"); // Success green
        if (score >= 5)
            return Color.FromArgb("#FF9800"); // Warning yellow/orange
        return Color.FromArgb("#F44336"); // Error red
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
