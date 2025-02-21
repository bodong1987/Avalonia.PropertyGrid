using System;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Utils;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

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
        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (!propertyDescriptor.PropertyType.IsEnum)
            {
                return null;
            }

            var isFlags = propertyDescriptor.PropertyType.IsDefined<FlagsAttribute>();

            if (isFlags)
            {
                var control = new CheckedListEdit
                {
                    Items = EnumUtils.GetEnumValues(propertyDescriptor.PropertyType, propertyDescriptor.Attributes.OfType<Attribute>())
                };

                control.SelectedItemsChanged += (s, e) =>
                {
                    var selectedValue = ConvertSelectedItemToEnum(propertyDescriptor.PropertyType, control.SelectedItems);

                    if ((propertyDescriptor.GetValue(target) as Enum)?.ToString() != selectedValue?.ToString())
                    {
                        SetAndRaise(context, control, selectedValue);
                    }
                };

                return control;
            }
            else
            {
                var control = new ComboBox
                {
                    ItemsSource = EnumUtils.GetEnumValues(propertyDescriptor.PropertyType, propertyDescriptor.Attributes.OfType<Attribute>()),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                control.SelectionChanged += (s, e) =>
                {
                    var item = control.SelectedItem;

                    if (item != null)
                    {
                        var v = (item as EnumValueWrapper)!.Value;

                        if (!Equals(v, propertyDescriptor.GetValue(target) as Enum))
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
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit!;

            if (!propertyDescriptor.PropertyType.IsEnum)
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is CheckedListEdit c)
            {
                var value = propertyDescriptor.GetValue(target) as Enum;
                var selectedValue = ConvertSelectedItemToEnum(propertyDescriptor.PropertyType, c.SelectedItems);

                if (value?.ToString() != selectedValue?.ToString())
                {
                    var old = c.EnableRaiseSelectedItemsChangedEvent;
                    c.EnableRaiseSelectedItemsChangedEvent = false;

                    try
                    {
                        c.SelectedItems = value!.GetUniqueFlagsExcluding().Select(x => new EnumValueWrapper(x)).ToArray();
                    }
                    finally
                    {
                        c.EnableRaiseSelectedItemsChangedEvent = old;
                    }
                }

                return true;
            }

            if (control is ComboBox cb)
            {
                var value = propertyDescriptor.GetValue(target) as Enum;
                cb.SelectedItem = new EnumValueWrapper(value!);
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
        private static Enum? ConvertSelectedItemToEnum(Type enumType, object[] items)
        {
            if (items.Length == 0)
            {
                return default;
            }

            if (items.Length == 1)
            {
                return (items[0] as EnumValueWrapper)?.Value;
            }

            var sb = new StringBuilder();
            sb.Append((items[0] as EnumValueWrapper)?.Value);

            foreach (var i in items.Skip(1))
            {
                sb.Append(", ");
                sb.Append((i as EnumValueWrapper)?.Value);
            }

            return (Enum)Enum.Parse(enumType, sb.ToString());
        }
    }
}