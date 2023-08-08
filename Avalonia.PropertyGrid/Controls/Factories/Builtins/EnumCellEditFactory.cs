using Avalonia.Controls;
using PropertyModels.Extensions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Utils;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class EnumCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class EnumCellEditFactory : AbstractCellEditFactory
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
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (!propertyDescriptor.PropertyType.IsEnum)
            {
                return null;
            }

            bool IsFlags = propertyDescriptor.PropertyType.IsDefined<FlagsAttribute>();
            //Enum value = propertyDescriptor.GetValue(target) as Enum;

            if (IsFlags)
            {
                var control = new CheckedListEdit();
                control.Items = EnumUtils.GetEnumValues(propertyDescriptor.PropertyType);

                control.SelectedItemsChanged += (s, e) =>
                {
                    Enum selectedValue = ConvertSelectedItemToEnum(propertyDescriptor.PropertyType, control.SelectedItems);

                    if ((propertyDescriptor.GetValue(target) as Enum)?.ToString() != selectedValue?.ToString())
                    {
                        SetAndRaise(context, control, selectedValue);
                    }
                };

                return control;
            }
            else
            {
                var control = new ComboBox();

                control.ItemsSource = EnumUtils.GetEnumValues(propertyDescriptor.PropertyType);
                control.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;

                control.SelectionChanged += (s, e) =>
                {
                    var item = control.SelectedItem;

                    if (item != null)
                    {
                        var v = (item as EnumValueWrapper).Value;

                        if (v != propertyDescriptor.GetValue(target) as Enum)
                        {
                            SetAndRaise(context, control, v);
                        }
                    }
                };

                return control;
            }
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

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
                        c.SelectedItems = value.GetUniqueFlags().Select(x => new EnumValueWrapper(x as Enum)).ToArray();
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
                cb.SelectedItem = new EnumValueWrapper(value);
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
                return (items[0] as EnumValueWrapper)?.Value;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append((items[0] as EnumValueWrapper)?.Value?.ToString());

                foreach (var i in items.Skip(1))
                {
                    sb.Append(", ");
                    sb.Append((i as EnumValueWrapper)?.Value?.ToString());
                }

                return (Enum)Enum.Parse(enumType, sb.ToString());
            }
        }
    }
}
