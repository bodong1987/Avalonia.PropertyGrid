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
    internal class PropertyGridSelectableListFactory : AbstractPropertyGridFactory
    {
        public override int ImportPriority => base.ImportPriority - 100000;

        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            if (!propertyDescriptor.PropertyType.IsImplementFrom<ISelectableList>())
            {
                return null;
            }

            var control = new ComboBox();

            control.Items = (propertyDescriptor.GetValue(target) as ISelectableList)?.Values;

            control.SelectionChanged += (s, e) =>
            {
                var item = control.SelectedItem;

                var list = (propertyDescriptor.GetValue(target) as ISelectableList);

                if (list != null)
                {
                    list.SelectedValue = item;
                }
            };

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
                cb.Items = list?.Values;
                cb.SelectedItem = list?.SelectedValue;
                return true;
            }

            return false;
        }
    }
}
