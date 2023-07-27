using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.Localization;
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
            CellEditFactoryService.Default.AddFactory(new ToggleSwitchCellEditFactory());
        }
    }

    class ToggleSwitchCellEditFactory : AbstractCellEditFactory
    {
        // make this extend factor only effect on ToggleSwitchExtensionPropertyGrid
        public override bool Accept(object accessToken)
        {
            return accessToken is ToggleSwitchExtensionPropertyGrid;
        }

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            
            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return null;
            }

            ToggleSwitch control = new ToggleSwitch();
            control.SetLocalizeBinding(ToggleSwitch.OnContentProperty, "On");
            control.SetLocalizeBinding(ToggleSwitch.OffContentProperty, "Off");

            control.IsCheckedChanged += (s, e) =>
            {
                SetAndRaise(context, control, control.IsChecked);
            };

            return control;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

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
