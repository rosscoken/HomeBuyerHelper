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
