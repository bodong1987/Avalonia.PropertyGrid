using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Utils;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

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
                control.Minimum = (decimal)(double)Convert.ChangeType(attr.Minimum, typeof(double));
                control.Maximum = (decimal)(double)Convert.ChangeType(attr.Maximum, typeof(double));
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

                if(formatAttr != null)
                {
                    control.FormatString = formatAttr.ToString()!;
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
                    SetAndRaise(context, control, value);
                }
                catch(Exception ex)
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
            }
            else
            {
                base.HandleReadOnlyStateChanged(control, readOnly);
            }
        }
    }
}
