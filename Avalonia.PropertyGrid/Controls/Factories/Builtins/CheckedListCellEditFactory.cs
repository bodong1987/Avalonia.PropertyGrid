using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls.Builtins;
using Avalonia.PropertyGrid.Services;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class CheckedListCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class CheckedListCellEditFactory : AbstractCellEditFactory
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

            if (!propertyDescriptor.PropertyType.IsImplementFrom<ICheckedList>() || propertyDescriptor.GetValue(target) == null)
            {
                return null;
            }

            if (propertyDescriptor.GetValue(target) is not ICheckedList list)
            {
                return null;
            }

            var control = new CheckedListEdit
            {
                Items = list.SourceItems
            };

            control.SelectedItemsChanged += (s, e) =>
            {
                var items = control.SelectedItems;
                var oldItems = list.Items;

                if(!CheckEquals(oldItems, items))
                {
                    var command = new GenericCancelableCommand(
                        string.Format(LocalizationService.Default["Change {0} selection from {1} to {2}"], context.Property.DisplayName, ArrayToString(oldItems), ArrayToString(items)),
                        () =>
                        {
                            list.SelectRange(items);
                            HandleRaiseEvent(control, context);

                            return true;
                        },
                        () =>
                        {
                            list.SelectRange(oldItems);
                            HandleRaiseEvent(control, context);

                            return true;
                        }
                        );

                    ExecuteCommand(command, context, list, list, oldItems);
                }
            };

            list.SelectionChanged += (s, e) =>
            {
                var cItems = control.Items;
                var lItems = list.Items;
                
                if(CheckEquals(cItems, lItems))
                {
                    return;
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

            if (!propertyDescriptor.PropertyType.IsImplementFrom<ICheckedList>())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is CheckedListEdit c)
            {
                if (propertyDescriptor.GetValue(target) is ICheckedList list)
                {
                    var old = c.EnableRaiseSelectedItemsChangedEvent;
                    c.EnableRaiseSelectedItemsChangedEvent = false;

                    try
                    {
                        c.SelectedItems = [];
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
