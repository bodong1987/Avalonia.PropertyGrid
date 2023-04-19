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
    internal class PropertyGridCheckedListFactory : AbstractPropertyGridFactory
    {
        public override int ImportPriority => base.ImportPriority - 100000;

        public override Control HandleNewProperty(PropertyGrid parent, object target, PropertyDescriptor propertyDescriptor)
        {
            if (!propertyDescriptor.PropertyType.IsImplementFrom<ICheckedList>())
            {
                return null;
            }

            ICheckedList list = propertyDescriptor.GetValue(target) as ICheckedList;

            if (list == null)
            {
                return null;
            }

            var control = new CheckListEdit();
            control.Items = list.SourceItems;

            control.SelectedItemsChanged += (s, e) =>
            {
                var items = control.SelectedItems;

                list.Clear();
                foreach (var item in items)
                {
                    list.SetChecked(item, true);

                    SetAndRaise(control, propertyDescriptor, target, list);
                }
            };

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (!propertyDescriptor.PropertyType.IsImplementFrom<ICheckedList>())
            {
                return false;
            }

            if (control is CheckListEdit c)
            {
                ICheckedList list = propertyDescriptor.GetValue(target) as ICheckedList;

                if (list != null)
                {
                    c.SelectedItems = new object[] { };
                    c.Items = list.SourceItems;
                    c.SelectedItems = list.Items;
                }

                return true;
            }

            return false;
        }
    }
}
