using Avalonia.Controls;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class BooleanCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class BooleanCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => base.ImportPriority - 100000;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;

            if (propertyDescriptor.PropertyType != typeof(bool) && propertyDescriptor.PropertyType != typeof(bool?))
            {
                return null;
            }

            var control = new CheckBox();

            control.IsThreeState = propertyDescriptor.PropertyType == typeof(bool?) || propertyDescriptor is MultiObjectPropertyDescriptor;

            control.IsCheckedChanged += (s, e) =>
            {
                SetAndRaise(context, control, control.IsChecked);
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

            if (propertyDescriptor.PropertyType != typeof(bool) && propertyDescriptor.PropertyType != typeof(bool?))
            {
                return false;
            }

            Debug.Assert(control != null);

            ValidateProperty(control, propertyDescriptor, target);

            if (control is CheckBox ts)
            {
                if (ts.IsThreeState)
                {
                    var obj = propertyDescriptor.GetValue(target);

                    if( obj != null)
                    {
                        ts.IsChecked = (bool)obj;
                    }
                    else
                    {
                        ts.IsChecked = null;
                    }
                }
                else
                {
                    ts.IsChecked = (bool)propertyDescriptor.GetValue(target)!;
                }

                return true;
            }

            return false;
        }
    }
}