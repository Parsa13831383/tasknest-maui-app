using System.Globalization;

namespace TaskNest.Converters;

/// <summary>
/// Converts a boolean value to a Color.
/// TrueColor (parameter format: "TrueHex|FalseHex") or defaults to green/gray.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public Color TrueColor { get; set; } = Color.FromArgb("#16A34A");
    public Color FalseColor { get; set; } = Color.FromArgb("#6B7280");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? TrueColor : FalseColor;
        }

        return FalseColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
