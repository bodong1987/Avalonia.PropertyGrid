using System;
using Avalonia.Controls;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins;

/// <summary>
/// Class TrackableCellEditFactory.
/// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.Builtins.NumericCellEditFactory" />
/// </summary>
/// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.Builtins.NumericCellEditFactory" />
public class TrackableCellEditFactory : NumericCellEditFactory
{
    /// <summary>
    /// Gets the import priority.
    /// The larger the value, the earlier the object will be processed
    /// </summary>
    /// <value>The import priority.</value>
    public override int ImportPriority => base.ImportPriority + 100;

    /// <summary>
    /// Handles the new property.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Control.</returns>
    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        if (!context.Property.PropertyType.IsNumericType() || !context.Property.IsDefined<TrackableAttribute>())
        {
            return null;
        }

        var control = new TrackableEdit();

        var attr = context.Property.GetCustomAttribute<TrackableAttribute>()!;
        control.Minimum = attr.Minimum;
        control.Maximum = attr.Maximum;
        control.AllowSpin = attr.AllowSpin;
        control.ShowButtonSpinner = attr.ShowButtonSpinner;

        if (context.Property.PropertyType == typeof(sbyte) ||
            context.Property.PropertyType == typeof(byte) ||
            context.Property.PropertyType == typeof(short) ||
            context.Property.PropertyType == typeof(ushort) ||
            context.Property.PropertyType == typeof(int) ||
            context.Property.PropertyType == typeof(uint) ||
            context.Property.PropertyType == typeof(long) ||
            context.Property.PropertyType == typeof(ulong)
           )
        {

            var incrementAttr = context.Property.GetCustomAttribute<IntegerIncrementAttribute>();

            if (incrementAttr != null)
            {
                control.Increment = incrementAttr.Increment;
            }
            else
            {
                if ((int)attr.Increment != 0)
                {
                    control.Increment = (int)attr.Increment;
                }
                else
                {
                    control.Increment = 1;
                }
            }

            control.FormatString = "{0:0}";
        }
        else
        {
            var precisionAttr = context.Property.GetCustomAttribute<FloatPrecisionAttribute>();
            if (precisionAttr != null)
            {
                control.Increment = (double)precisionAttr.Increment;
                control.FormatString = precisionAttr.FormatString;
            }
            else
            {
                control.Increment = attr.Increment;
                control.FormatString = attr.FormatString;
            }
        }

        control.RealValueChanged += (s, e) =>
        {
            try
            {
                var value = Convert.ChangeType(control.Value, context.Property.PropertyType);

                var args = (e as RealValueChangedEventArgs)!;
                    
                SetAndRaise(context, control, value,
                    args.Reason == RealValueChangedReason.DragEnd
                        ? Convert.ChangeType(args.OldValue, context.Property.PropertyType)
                        : context.GetValue());
            }
            catch (Exception ex)
            {
                DataValidationErrors.SetErrors(control, [ex.Message]);
            }
        };

        control.PreviewValueChanged += (s, e) =>
        {
            try
            {
                // just set value, don't generate a command
                var value = Convert.ChangeType(control.Value, context.Property.PropertyType);
                HandleSetValue(context, control, value);
            }
            catch (Exception ex)
            {
                DataValidationErrors.SetErrors(control, [ex.Message]);
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
        if (!context.Property.PropertyType.IsNumericType() || !context.Property.IsDefined<TrackableAttribute>())
        {
            return false;
        }

        if (context.CellEdit is TrackableEdit te)
        {
            if (double.TryParse(context.GetValue()?.ToString(), out var d))
            {
                te.Value = d;
            }
            else
            {
                try
                {
                    te.Value = (double)Convert.ChangeType(context.GetValue()!, typeof(double));
                }
                catch (Exception ex)
                {
                    DataValidationErrors.SetErrors(context.CellEdit, [ex.Message]);
                }
            }

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
        if (control is TrackableEdit te)
        {
            te.IsReadOnly = readOnly;
                
            // Change appearance when read-only
            control.Opacity = readOnly ? 0.8 : // Lower opacity to indicate read-only
                1.0; // Reset opacity
        }
        else
        {
            base.HandleReadOnlyStateChanged(control, readOnly);
        }
    }
}