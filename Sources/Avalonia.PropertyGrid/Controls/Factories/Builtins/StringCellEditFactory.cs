using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.Utils;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins;

/// <summary>
/// Class StringCellEditFactory.
/// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
/// </summary>
/// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
public class StringCellEditFactory : AbstractCellEditFactory
{
    /// <summary>
    /// Gets the import priority.
    /// The larger the value, the earlier the object will be processed
    /// </summary>
    /// <value>The import priority.</value>
    public override int ImportPriority => base.ImportPriority - 1000000;

    /// <summary>
    /// Handles the new property.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Control.</returns>
    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;

        if (propertyDescriptor.PropertyType != typeof(string))
        {
            return null;
        }

        var watermarkAttr = propertyDescriptor.GetCustomAttribute<WatermarkAttribute>();

        var control = new TextBox
        {
            Text = propertyDescriptor.GetValue(target) as string,
            VerticalContentAlignment = VerticalAlignment.Center,
            FontFamily = FontUtils.DefaultFontFamily
        };

        if (watermarkAttr != null)
        {
            // control.Watermark = LocalizationService.Default[watermarkAttr.Watermark];
            control.SetLocalizeBinding(TextBox.WatermarkProperty, watermarkAttr.Watermark);
        }

        if (propertyDescriptor.GetCustomAttribute<PasswordPropertyTextAttribute>() is { Password: true })
        {
            control.PasswordChar = '*';
        }

        var multilineAttr = propertyDescriptor.GetCustomAttribute<MultilineTextAttribute>();

        if (multilineAttr is { IsMultiline: true })
        {
            control.TextWrapping = TextWrapping.Wrap;
            control.AcceptsReturn = true;
            control.AcceptsTab = true;
        }

        control.PropertyChanged += (s, e) =>
        {
            if (e.Property == TextBox.TextProperty)
            {
                if ((e.NewValue as string) != (propertyDescriptor.GetValue(target) as string))
                {
                    SetAndRaise(context, control, e.NewValue);
                }
            }
        };

        return control;
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

        if (propertyDescriptor.PropertyType != typeof(string))
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is TextBox textBox)
        {
            textBox.Text = propertyDescriptor.GetValue(target) as string;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Handles readonly flag changed
    /// </summary>
    /// <param name="control">control.</param>
    /// <param name="readOnly">readonly flag</param>
    /// <returns>Control.</returns>
    public override void HandleReadOnlyStateChanged(Control control, bool readOnly)
    {
        if (control is TextBox textBox)
        {
            textBox.IsReadOnly = readOnly;
                
            // Change appearance when read-only
            textBox.Opacity = readOnly ? 0.8 : // Lower opacity to indicate read-only
                1.0; // Reset opacity
        }
        else
        {
            base.HandleReadOnlyStateChanged(control, readOnly);
        }
    }
}