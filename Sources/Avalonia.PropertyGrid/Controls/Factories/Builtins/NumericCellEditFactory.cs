using System;
using System.ComponentModel.DataAnnotations;
using Avalonia.Controls;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using PropertyModels.Utils;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins;

/// <summary>
/// Class NumericCellEditFactory.
/// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
/// </summary>
/// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
public class NumericCellEditFactory : AbstractCellEditFactory
{
    /// <summary>
    /// Gets the import priority.
    /// The larger the value, the earlier the object will be processed
    /// </summary>
    /// <value>The import priority.</value>
    public override int ImportPriority => base.ImportPriority - 10000000;

    /// <summary>
    /// float equal test tolerance
    /// </summary>
#pragma warning disable CA2211
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once ConvertToConstant.Global
    public static float FloatEqualTolerance = 0.00001f;
#pragma warning restore CA2211
        
    /// <summary>
    /// double equal tolerance
    /// </summary>
#pragma warning disable CA2211
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once ConvertToConstant.Global
    public static double DoubleEqualTolerance = 0.00001;
#pragma warning restore CA2211
        
    /// <summary>
    /// decimal equal tolerance
    /// </summary>
#pragma warning disable CA2211
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once ConvertToConstant.Global
    public static decimal DecimalEqualTolerance = (decimal)0.00001;
#pragma warning restore CA2211
        

    /// <summary>
    /// Handles the new property.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Control.</returns>
    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        // var target = context.Target;

        if (!propertyDescriptor.PropertyType.IsNumericType())
        {
            return null;
        }

        var control = new NumericUpDown();

        var attr = propertyDescriptor.GetCustomAttribute<RangeAttribute>();

        if (attr != null)
        {
            var values = DecimalConvertUtils.GetDecimalRange(attr);
            control.Minimum = values.Min;
            control.Maximum = values.Max;
        }

        var formatAttr = propertyDescriptor.GetCustomAttribute<FormatStringAttribute>();

        if (propertyDescriptor.PropertyType == typeof(sbyte) ||
            propertyDescriptor.PropertyType == typeof(byte) ||
            propertyDescriptor.PropertyType == typeof(short) ||
            propertyDescriptor.PropertyType == typeof(ushort) ||
            propertyDescriptor.PropertyType == typeof(int) ||
            propertyDescriptor.PropertyType == typeof(uint) ||
            propertyDescriptor.PropertyType == typeof(long) ||
            propertyDescriptor.PropertyType == typeof(ulong)
           )
        {
            var incrementAttr = propertyDescriptor.GetCustomAttribute<IntegerIncrementAttribute>();

            control.Increment = incrementAttr?.Increment ?? 1;

            if (formatAttr != null)
            {
                control.FormatString = formatAttr.ToString()!;
            }
        }
        else
        {
            var precisionAttr = propertyDescriptor.GetCustomAttribute<FloatPrecisionAttribute>();
            if (precisionAttr != null)
            {
                control.Increment = precisionAttr.Increment;
                control.FormatString = precisionAttr.FormatString;
            }
            else
            {
                control.Increment = (decimal)0.01;
                control.FormatString = "{0:0.00}";

                if (formatAttr != null)
                {
                    control.FormatString = formatAttr.ToString()!;
                }
            }
        }

        control.ValueChanged += (s, e) =>
        {
            try
            {
                var value = Convert.ChangeType(control.Value, propertyDescriptor.PropertyType);
                SetAndRaise(context, control, value, context.GetValue());
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
        var propertyDescriptor = context.Property;
        var target = context.Target;
        var control = context.CellEdit!;

        if (!propertyDescriptor.PropertyType.IsNumericType())
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is NumericUpDown nup)
        {
            var value = propertyDescriptor.GetValue(target)!;

            nup.Value = DecimalConvertUtils.ConvertTo(value);

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
        if (control is NumericUpDown nup)
        {
            nup.IsReadOnly = readOnly;
                
            // Change appearance when read-only
            control.Opacity = readOnly ? 0.8 : // Lower opacity to indicate read-only
                1.0; // Reset opacity
        }
        else
        {
            base.HandleReadOnlyStateChanged(control, readOnly);
        }
    }

    /// <inheritdoc />
    protected override bool CheckIsPropertyChanged(PropertyCellContext context, object? value, object? oldValue)
    {
        if (context.Property.PropertyType == typeof(double) || 
            context.Property.PropertyType == typeof(float) ||
            context.Property.PropertyType == typeof(decimal))
        {
            if (oldValue == null && value == null)
            {
                return false;
            }

            if (context.Property.PropertyType == typeof(double))
            {
                var oValue = Convert.ToDouble(oldValue);
                var nValue = Convert.ToDouble(value);
                    
                var tolerance = context.Property.GetCustomAttribute<FloatingNumberEqualToleranceAttribute>()?.Tolerance ?? DoubleEqualTolerance;

                return Math.Abs(nValue - oValue) > tolerance;
            }
                
            if (context.Property.PropertyType == typeof(float))
            {
                var oValue = Convert.ToSingle(oldValue);
                var nValue = Convert.ToSingle(value);
                    
                var tolerance = context.Property.GetCustomAttribute<FloatingNumberEqualToleranceAttribute>()?.Tolerance ?? FloatEqualTolerance;

                return Math.Abs(nValue - oValue) > tolerance;
            }

            if (context.Property.PropertyType == typeof(decimal))
            {
                var oValue = Convert.ToDecimal(oldValue);
                var nValue = Convert.ToDecimal(value);
                var v = context.Property.GetCustomAttribute<FloatingNumberEqualToleranceAttribute>()?.Tolerance;
                var tolerance = v != null ? (decimal)(float)v : DecimalEqualTolerance;

                return Math.Abs(nValue - oValue) > tolerance;
            }
        }
            
        return base.CheckIsPropertyChanged(context, value, oldValue);
    }
}