using System;
using System.Globalization;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.PropertyGrid.Utils;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class TrackableEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class TrackableEdit : RangeBase
    {
        /// <summary>
        /// The increment property
        /// </summary>
        public static readonly StyledProperty<double> IncrementProperty =
            AvaloniaProperty.Register<TrackableEdit, double>(nameof(Increment), 0.01);

        /// <summary>
        /// Gets or sets the increment.
        /// </summary>
        /// <value>The increment.</value>
        public double Increment
        {
            get => GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        /// <summary>
        /// The format string property
        /// </summary>
        public static readonly StyledProperty<string> FormatStringProperty = 
            AvaloniaProperty.Register<TrackableEdit, string>(nameof(FormatString), "{0:0.00}");

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString
        {
            get => GetValue(FormatStringProperty);
            set => SetValue(FormatStringProperty, value);
        }

        /// <summary>
        /// The Readonly property
        /// </summary>
        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<TrackableEdit, bool>(nameof(IsReadOnly));

        /// <summary>
        /// Gets or sets IsReadOnly state
        /// </summary>        
        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
    }

    /// <summary>
    /// Class DoubleToDecimalConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class DoubleToDecimalConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if((targetType == typeof(decimal) || targetType == typeof(decimal?)) && value != null)
            {
                return DecimalConvertUtils.ConvertTo(value);
            }

            return value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value != null)
            {
                try
                {
                    var v = System.Convert.ChangeType(value, targetType);

                    if(v != null)
                    {
                        return v;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return value;
        }
    }
}
