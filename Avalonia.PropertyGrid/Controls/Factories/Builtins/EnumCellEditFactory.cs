using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class EnumCellEditFactory : AbstractCellEditFactory
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
        /// <param name="rootPropertyGrid">The root property grid.</param>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
            if (!propertyDescriptor.PropertyType.IsEnum)
            {
                return null;
            }

            bool IsFlags = propertyDescriptor.PropertyType.IsDefined<FlagsAttribute>();
            //Enum value = propertyDescriptor.GetValue(target) as Enum;

            if (IsFlags)
            {
                var control = new CheckedListEdit();
                control.Items = Enum.GetValues(propertyDescriptor.PropertyType).Select(x => x.ToString()).ToArray();

                control.SelectedItemsChanged += (s, e) =>
                {
                    Enum selectedValue = ConvertSelectedItemToEnum(propertyDescriptor.PropertyType, control.SelectedItems);

                    if ((propertyDescriptor.GetValue(target) as Enum)?.ToString() != selectedValue?.ToString())
                    {
                        SetAndRaise(rootPropertyGrid, control, propertyDescriptor, target, selectedValue);
                    }
                };

                return control;
            }
            else
            {
                var control = new ComboBox();

                control.ItemsSource = Enum.GetValues(propertyDescriptor.PropertyType);
                control.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;

                control.SelectionChanged += (s, e) =>
                {
                    var item = control.SelectedItem;

                    if (item != null)
                    {
                        var v = Enum.Parse(propertyDescriptor.PropertyType, item.ToString()) as Enum;

                        if (v != propertyDescriptor.GetValue(target) as Enum)
                        {
                            SetAndRaise(rootPropertyGrid, control, propertyDescriptor, target, v);
                        }
                    }
                };

                return control;
            }
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="control">The control.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (!propertyDescriptor.PropertyType.IsEnum)
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is CheckedListEdit c)
            {
                Enum value = propertyDescriptor.GetValue(target) as Enum;
                Enum selectedValue = ConvertSelectedItemToEnum(propertyDescriptor.PropertyType, c.SelectedItems);

                if (value?.ToString() != selectedValue?.ToString())
                {
                    var old = c.EnableRaiseSelectedItemsChangedEvent;
                    c.EnableRaiseSelectedItemsChangedEvent = false;

                    try
                    {
                        c.SelectedItems = value.GetUniqueFlags().Select(x => x.ToString()).ToArray();
                    }
                    finally
                    {
                        c.EnableRaiseSelectedItemsChangedEvent = old;
                    }
                }

                return true;
            }
            else if (control is ComboBox cb)
            {
                Enum value = propertyDescriptor.GetValue(target) as Enum;
                cb.SelectedItem = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Converts the selected item to enum.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="items">The items.</param>
        /// <returns>Enum.</returns>
        private static Enum ConvertSelectedItemToEnum(Type enumType, object[] items)
        {
            if (items.Length == 0)
            {
                return default(Enum);
            }
            else if (items.Length == 1)
            {
                return (Enum)Enum.Parse(enumType, items[0].ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(items[0]);

                foreach (var i in items.Skip(1))
                {
                    sb.Append(", ");
                    sb.Append(i.ToString());
                }

                return (Enum)Enum.Parse(enumType, sb.ToString());
            }
        }
    }
}
