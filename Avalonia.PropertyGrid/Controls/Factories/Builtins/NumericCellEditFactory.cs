using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.Controls.Embedding;
using Newtonsoft.Json.Linq;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class NumericCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 1000000;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            if (!propertyDescriptor.PropertyType.IsNumericType())
            {
                return null;
            }

            var control = new NumericUpDown();

            var attr = propertyDescriptor.GetCustomAttribute<RangeAttribute>();

            if (attr != null)
            {
                control.Minimum = (double)Convert.ChangeType(attr.Minimum, typeof(double));
                control.MaxHeight = (double)Convert.ChangeType(attr.Maximum, typeof(double));
            }

            if (propertyDescriptor.PropertyType == typeof(byte) ||
                propertyDescriptor.PropertyType == typeof(short) ||
                propertyDescriptor.PropertyType == typeof(ushort) ||
                propertyDescriptor.PropertyType == typeof(int) ||
                propertyDescriptor.PropertyType == typeof(uint) ||
                propertyDescriptor.PropertyType == typeof(Int64) ||
                propertyDescriptor.PropertyType == typeof(UInt64)
                )
            {
                control.Increment = 1;
            }
            else
            {
                control.Increment = 0.01;
                control.FormatString = "{0:0.00}";
            }

            control.ValueChanged += (s, e) =>
            {
                try
                {
                    object value = Convert.ChangeType(control.Value, propertyDescriptor.PropertyType);
                    SetAndRaise(control, propertyDescriptor, target, value);
                }
                catch(Exception ex)
                {
                    DataValidationErrors.SetErrors(control, new string[] { ex.Message });
                }
            };

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (!propertyDescriptor.PropertyType.IsNumericType())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is NumericUpDown nup)
            {
                nup.Value = (double)Convert.ChangeType(propertyDescriptor.GetValue(target), typeof(double));

                return true;
            }

            return false;
        }
    }
}
