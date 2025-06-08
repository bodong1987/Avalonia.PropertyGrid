using System;
using Avalonia.Data.Converters;

namespace Avalonia.PropertyGrid.Utils;

/// <summary>
/// A value converter that negates a boolean value.
/// </summary>
public class BooleanNegationConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to its negation.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A negated boolean value if the input is a boolean; otherwise, AvaloniaProperty.UnsetValue.
    /// </returns>
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return AvaloniaProperty.UnsetValue;
    }

    /// <summary>
    /// Converts a negated boolean value back to its original value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// The original boolean value if the input is a boolean; otherwise, AvaloniaProperty.UnsetValue.
    /// </returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }
        return AvaloniaProperty.UnsetValue;
    }
}