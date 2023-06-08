using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Model.Extensions;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class SelectableListCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 100000;

        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            if (!propertyDescriptor.PropertyType.IsImplementFrom<ISelectableList>() ||
                propertyDescriptor.GetValue(target) == null
                )
            {
                return null;
            }

            var control = new ComboBox();

            var list = (propertyDescriptor.GetValue(target) as ISelectableList);

            control.ItemsSource = list?.Values;
            control.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;

            control.SelectionChanged += (s, e) =>
            {
                var item = control.SelectedItem;

                if (list != null)
                {
                    list.SelectedValue = item;
                }
            };

            if(list != null)
            {
                list.SelectionChanged += (s, e) =>
                {
                    control.SelectedItem = list.SelectedValue;
                };
            }            

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (!propertyDescriptor.PropertyType.IsImplementFrom<ISelectableList>())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ComboBox cb)
            {
                var list = propertyDescriptor.GetValue(target) as ISelectableList;
                cb.ItemsSource = list?.Values;
                cb.SelectedItem = list?.SelectedValue;

                return true;
            }

            return false;
        }
    }
}
