using Avalonia.Controls;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using PropertyModels.Extensions;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
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
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(PropertyCellContext context)
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
                control.Minimum = (Decimal)(double)Convert.ChangeType(attr.Minimum, typeof(double));
                control.Maximum = (Decimal)(double)Convert.ChangeType(attr.Maximum, typeof(double));
            }

            var formatAttr = propertyDescriptor.GetCustomAttribute<FormatStringAttribute>();

            if (propertyDescriptor.PropertyType == typeof(sbyte) ||
                propertyDescriptor.PropertyType == typeof(byte) ||
                propertyDescriptor.PropertyType == typeof(short) ||
                propertyDescriptor.PropertyType == typeof(ushort) ||
                propertyDescriptor.PropertyType == typeof(int) ||
                propertyDescriptor.PropertyType == typeof(uint) ||
                propertyDescriptor.PropertyType == typeof(Int64) ||
                propertyDescriptor.PropertyType == typeof(UInt64)
                )
            {
                var incrementAttr = propertyDescriptor.GetCustomAttribute<IntegerIncrementAttribute>();

                control.Increment = incrementAttr?.Increment ?? 1;

                if(formatAttr != null)
                {
                    control.FormatString = formatAttr.ToString();
                }
            }
            else
            {
                var precisionAttr = propertyDescriptor.GetCustomAttribute<FloatPrecisionAttribute>();
                if(precisionAttr != null)
                {
                    control.Increment = precisionAttr.Increment;
                    control.FormatString = precisionAttr.FormatString;
                }
                else
                {
                    control.Increment = (Decimal)0.01;
                    control.FormatString = "{0:0.00}";

                    if (formatAttr != null)
                    {
                        control.FormatString = formatAttr.ToString();
                    }
                }                
            }

            control.ValueChanged += (_, _) =>
            {
                try
                {
                    var value = Convert.ChangeType(control.Value, propertyDescriptor.PropertyType);
                    SetAndRaise(context, control, value);
                }
                catch(Exception ex)
                {
                    DataValidationErrors.SetErrors(control, new[] { ex.Message });
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
            var control = context.CellEdit;

            if (!propertyDescriptor.PropertyType.IsNumericType())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is NumericUpDown nup)
            {
                if(Decimal.TryParse(((double)Convert.ChangeType(propertyDescriptor.GetValue(target), typeof(double))).ToString(CultureInfo.InvariantCulture), out var d))
                {
                    nup.Value = d;
                }
                else
                {
                    nup.Value = null;
                }

                return true;
            }

            return false;
        }
    }
}
