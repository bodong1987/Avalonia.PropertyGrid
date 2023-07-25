using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class TrackableCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 99999;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            if (!context.Property.PropertyType.IsNumericType() || !context.Property.IsDefined<TrackableAttribute>())
            {
                return null;
            }

            TrackableEdit control = new TrackableEdit();

            TrackableAttribute attr = context.Property.GetCustomAttribute<TrackableAttribute>();
            control.Minimum = attr.Minimum;
            control.Maximum = attr.Maximum;

            control.ValueChanged += (s, e) =>
            {
                SetAndRaise(context, control, e.NewValue);
            };

            return control;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            if (!context.Property.PropertyType.IsNumericType() || !context.Property.IsDefined<TrackableAttribute>())
            {
                return false;
            }

            if(context.CellEdit is TrackableEdit te)
            {
                if(double.TryParse(context.GetValue().ToString(), out double d))
                {
                    te.Value = d;
                }
                else
                {
                    try
                    {
                        te.Value = (double)Convert.ChangeType(context.GetValue(), typeof(double));
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
    }
}
