using Avalonia.Controls;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class TrackableCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class TrackableCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => base.ImportPriority - 99999;

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

            TrackableEdit control = new TrackableEdit();

            TrackableAttribute attr = context.Property.GetCustomAttribute<TrackableAttribute>()!;
            control.Minimum = attr.Minimum;
            control.Maximum = attr.Maximum;

            if (context.Property.PropertyType == typeof(sbyte) ||
                context.Property.PropertyType == typeof(byte) ||
                context.Property.PropertyType == typeof(short) ||
                context.Property.PropertyType == typeof(ushort) ||
                context.Property.PropertyType == typeof(int) ||
                context.Property.PropertyType == typeof(uint) ||
                context.Property.PropertyType == typeof(Int64) ||
                context.Property.PropertyType == typeof(UInt64)
                )
            {

                var incrementAttr = context.Property.GetCustomAttribute<IntegerIncrementAttribute>();

                if(incrementAttr != null)
                {
                    control.Increment = (int)incrementAttr.Increment;
                }
                else
                {
                    if((int)attr.Increment != 0)
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

            control.ValueChanged += (s, e) =>
            {
                try
                {                    
                    object value = Convert.ChangeType(e.NewValue, context.Property.PropertyType);
                    SetAndRaise(context, control, value);
                }
                catch (Exception ex)
                {
                    DataValidationErrors.SetErrors(control, new string[] { ex.Message });
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

            if(context.CellEdit is TrackableEdit te)
            {
                if(double.TryParse(context.GetValue()?.ToString(), out double d))
                {
                    te.Value = d;
                }
                else
                {
                    try
                    {
                        te.Value = (double)Convert.ChangeType(context.GetValue()!, typeof(double));
                    }
                    catch(Exception ex)
                    {
                        DataValidationErrors.SetErrors(context.CellEdit, new object[] { ex.Message });
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
            }
            else
            {
                base.HandleReadOnlyStateChanged(control, readOnly);
            }
        }
    }
}
