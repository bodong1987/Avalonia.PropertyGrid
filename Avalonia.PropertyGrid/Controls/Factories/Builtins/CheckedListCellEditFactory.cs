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
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (!propertyDescriptor.PropertyType.IsImplementFrom<ICheckedList>() || propertyDescriptor.GetValue(target) == null)
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
                var oldItems = list.Items;

                if(!CheckEquals(oldItems, items))
                {
                    GenericCancelableCommand command = new GenericCancelableCommand(
                        string.Format(PropertyGrid.LocalizationService["Change {0} selection from {1} to {2}"], context.Property.DisplayName, ArrayToString(oldItems), ArrayToString(items)),
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
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

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
