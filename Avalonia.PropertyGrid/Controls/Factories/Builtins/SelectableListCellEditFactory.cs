using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.ComponentModel;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class SelectableListCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 100000;

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            
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

                if (list != null && !list.SelectedValue.Equals(item))
                {
                    var oldValue = list.SelectedValue;

                    GenericCancelableCommand command = new GenericCancelableCommand(
                        string.Format(PropertyGrid.LocalizationService["Change {0} selection from {1} to {2}"], context.Property.DisplayName, oldValue!=null?oldValue.ToString():"null", item!=null?item.ToString():"null"),
                        () =>
                        {
                            list.SelectedValue = item;
                            return true;
                        },
                        () =>
                        {
                            list.SelectedValue = oldValue;
                            return true;
                        }
                        );

                    ExecuteCommand(command, context, list, list, oldValue);
                }
            };

            if(list != null)
            {
                list.SelectionChanged += (s, e) =>
                {
                    if(control.SelectedItem == null || !control.SelectedItem.Equals(list.SelectedValue))
                    {
                        control.SelectedItem = list.SelectedValue;
                    }                    
                };
            }            

            return control;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

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
