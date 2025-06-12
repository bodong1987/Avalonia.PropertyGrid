using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media;
using Color = System.Drawing.Color;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins;

/// <summary>
/// Class ColorCellEditFactory.
/// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
/// </summary>
/// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
public class ColorCellEditFactory : AbstractCellEditFactory
{
    /// <summary>
    /// Gets the import priority.
    /// The larger the value, the earlier the object will be processed
    /// </summary>
    /// <value>The import priority.</value>
    public override int ImportPriority => base.ImportPriority - 100000;

    /// <summary>
    /// Determines whether [is available color type] [the specified property].
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns><c>true</c> if [is available color type] [the specified property]; otherwise, <c>false</c>.</returns>
    public static bool IsAvailableColorType(PropertyDescriptor property)
    {
        var type = property.PropertyType;

        return type == typeof(Color) ||
               type == typeof(Media.Color) ||
               type == typeof(HslColor) ||
               type == typeof(HsvColor);
    }

    /// <summary>
    /// make child factory can override property changed testing
    /// </summary>
    /// <param name="context"></param>
    /// <param name="value"></param>
    /// <param name="oldValue"></param>
    /// <returns></returns>
    protected override bool CheckIsPropertyChanged(PropertyCellContext context, object? value, object? oldValue)
    {
        if (context.Property.PropertyType == typeof(Color))
        {
            var color1 = (Color)value!;
            var color2 = (Color)oldValue!;
                
            return color1.ToArgb() != color2.ToArgb();
        }
            
        return base.CheckIsPropertyChanged(context, value, oldValue);   
    }

    /// <summary>
    /// Handles the new property.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Control.</returns>
    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        //var target = context.Target;

        if (!IsAvailableColorType(propertyDescriptor))
        {
            return null;
        }

        var type = propertyDescriptor.PropertyType;

        var colorPicker = new PreviewableColorPicker();

        colorPicker.ColorChanged += (s, args) =>
        {
            var e = args as ColorChangedEventArgs;
            Debug.Assert(e != null);
            
            if (type == typeof(Color))
            {
                var c = Color.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
                var oldColor = Color.FromArgb(e.OldColor.A, e.OldColor.R, e.OldColor.G, e.OldColor.B);
                SetAndRaise(context, colorPicker, c,  oldColor);
            }
            else if (type == typeof(Media.Color))
            {
                SetAndRaise(context, colorPicker, e.NewColor, e.OldColor);
            }
            else if (type == typeof(HslColor))
            {
                SetAndRaise(context, colorPicker, e.NewColor.ToHsl(), e.OldColor.ToHsl());
            }
            else if (type == typeof(HsvColor))
            {
                SetAndRaise(context, colorPicker, e.NewColor.ToHsv(), e.OldColor.ToHsv());
            }
        };

        colorPicker.PreviewColorChanged += (s, args) =>
        {
            var e = args as ColorChangedEventArgs;
            Debug.Assert(e != null);
            
            if (type == typeof(Color))
            {
                var c = Color.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
                HandleSetValue(context, colorPicker, c);
            }
            else if (type == typeof(Media.Color))
            {
                HandleSetValue(context, colorPicker, e.NewColor);
            }
            else if (type == typeof(HslColor))
            {
                HandleSetValue(context, colorPicker, e.NewColor.ToHsl());
            }
            else if (type == typeof(HsvColor))
            {
                HandleSetValue(context, colorPicker, e.NewColor.ToHsv());
            }
        };

        return colorPicker;
    }

    /// <summary>
    /// Handles the property changed.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
    public override bool HandlePropertyChanged(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        var control = context.CellEdit!;

        if (!IsAvailableColorType(propertyDescriptor))
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is PreviewableColorPicker colorPicker)
        {
            colorPicker.Color = propertyDescriptor.GetValue(target) switch
            {
                Color color => Media.Color.FromArgb(color.A, color.R, color.G, color.B),
                Media.Color color => color,
                HslColor color => color.ToRgb(),
                HsvColor color => color.ToRgb(),
                _ => colorPicker.Color
            };

            return true;
        }

        return false;
    }
}