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

        public override Control HandleNewProperty(PropertyGrid parent, object target, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType != typeof(bool) && propertyDescriptor.PropertyType != typeof(bool?))
            {
                return null;
            }

            var control = new CheckBox();

            control.IsThreeState = propertyDescriptor.PropertyType == typeof(bool?);

            control.Checked += (s, e) => { propertyDescriptor.SetAndRaiseEvent(target, true); };
            control.Unchecked += (s, e) => { propertyDescriptor.SetAndRaiseEvent(target, false); };

            if (control.IsThreeState)
            {
                control.Indeterminate += (s, e) => { propertyDescriptor.SetAndRaiseEvent(target, null); };
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
