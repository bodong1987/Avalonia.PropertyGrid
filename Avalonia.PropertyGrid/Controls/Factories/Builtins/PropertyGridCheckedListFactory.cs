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

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
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

            var control = new CheckedListEdit();
            control.Items = list.SourceItems;

            control.SelectedItemsChanged += (s, e) =>
            {
                var items = control.SelectedItems;

                list.SelectRange(items);

                SetAndRaise(control, propertyDescriptor, target, list);
            };

            list.SelectionChanged += (s, e) =>
            {
                var cItems = control.Items;
                var lItems = list.Items;
                if(cItems.Length == lItems.Length)
                {
                    bool Same = true;
                    for(int i=0; i<lItems.Length; i++)
                    {
                        if (!lItems[i].Equals(cItems[i]))
                        {
                            Same = false;
                            break;
                        }
                    }

                    if(Same)
                    {
                        return;
                    }
                }

                
                var old = control.EnableRaiseSelectedItemsChangedEvent;
                try
                {
                    control.EnableRaiseSelectedItemsChangedEvent = false;
                    control.SelectedItems = lItems;

                    ValidateProperty(control, propertyDescriptor, target);
                }
                finally
                {
                    control.EnableRaiseSelectedItemsChangedEvent = old;
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

            ValidateProperty(control, propertyDescriptor, target);

            if (control is CheckedListEdit c)
            {
                ICheckedList list = propertyDescriptor.GetValue(target) as ICheckedList;

                if (list != null)
                {
                    var old = c.EnableRaiseSelectedItemsChangedEvent;
                    c.EnableRaiseSelectedItemsChangedEvent = false;

                    try
                    {
                        c.SelectedItems = new object[] { };
                        c.Items = list.SourceItems;
                        c.SelectedItems = list.Items;
                    }
                    finally
                    {
                        c.EnableRaiseSelectedItemsChangedEvent = old;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
