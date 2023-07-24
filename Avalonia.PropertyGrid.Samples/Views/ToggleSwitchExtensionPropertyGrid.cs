using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Views
{
    public class ToggleSwitchExtensionPropertyGrid : Controls.PropertyGrid
    {
        static ToggleSwitchExtensionPropertyGrid()
        {
            FactoryTemplates.AddFactory(new ToggleSwitchCellEditFactory());
        }
    }

    class ToggleSwitchCellEditFactory : AbstractCellEditFactory
    {
        // make this extend factor only effect on ToggleSwitchExtensionPropertyGrid
        public override bool Accept(object accessToken)
        {
            return accessToken is ToggleSwitchExtensionPropertyGrid;
        }

        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return null;
            }

            ToggleSwitch control = new ToggleSwitch();
            control.IsCheckedChanged += (s, e) =>
            {
                SetAndRaise(control, propertyDescriptor, target, control.IsChecked);
            };

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ToggleSwitch ts)
            {
                ts.IsChecked = (bool)propertyDescriptor.GetValue(target);

                return true;
            }

            return false;
        }
    }
}
