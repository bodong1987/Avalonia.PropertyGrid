using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class PropertyGridBooleanFactory : AbstractPropertyGridFactory
    {
        public override int ImportPriority => base.ImportPriority - 100000;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType != typeof(bool) && propertyDescriptor.PropertyType != typeof(bool?))
            {
                return null;
            }

            var control = new CheckBox();

            control.IsThreeState = propertyDescriptor.PropertyType == typeof(bool?);

            control.Checked += (s, e) => { SetAndRaise(control, propertyDescriptor, target, true); };
            control.Unchecked += (s, e) => { SetAndRaise(control, propertyDescriptor, target, false); };

            if (control.IsThreeState)
            {
                control.Indeterminate += (s, e) => { SetAndRaise(control, propertyDescriptor, target, null); };
            }

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (propertyDescriptor.PropertyType != typeof(bool) && propertyDescriptor.PropertyType != typeof(bool?))
            {
                return false;
            }

            if (control is CheckBox ts)
            {
                if(ts.IsThreeState)
                {
                    ts.IsChecked = propertyDescriptor.GetValue(target) as bool?;
                }
                else
                {
                    ts.IsChecked = (bool)propertyDescriptor.GetValue(target);
                }                

                return true;
            }

            return false;
        }
    }
}
