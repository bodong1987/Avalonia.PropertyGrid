using Avalonia.Controls;
using PropertyModels.Collections;
using PropertyModels.Extensions;
using PropertyModels.ComponentModel;
using Avalonia.PropertyGrid.Services;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class SelectableListCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class SelectableListCellEditFactory : AbstractCellEditFactory
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

                if (list != null && (list.SelectedValue == null || !list.SelectedValue.Equals(item)))
                {
                    var oldValue = list.SelectedValue;

                    GenericCancelableCommand command = new GenericCancelableCommand(
                        string.Format(LocalizationService.Default["Change {0} selection from {1} to {2}"], context.Property.DisplayName, oldValue!=null?oldValue.ToString():"null", item!=null?item.ToString():"null"),
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

                    HandleRaiseEvent(control, context);
                }
            };

            if(list != null)
            {
                list.SelectionChanged += (s, e) =>
                {
                    if(!CheckEqual(control.SelectedItem, list.SelectedValue))
                    {
                        control.SelectedItem = list.SelectedValue;
                    }                    
                };
            }            

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

            if (!propertyDescriptor.PropertyType.IsImplementFrom<ISelectableList>())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ComboBox cb)
            {
                var list = propertyDescriptor.GetValue(target) as ISelectableList;

                if(!CheckEquals(cb.ItemsSource as object[], list?.Values))
                {
                    cb.ItemsSource = list?.Values;
                }
                
                if(!CheckEqual(cb.SelectedItem, list?.SelectedValue))
                {
                    cb.SelectedItem = list?.SelectedValue;
                }                

                return true;
            }

            return false;
        }
    }
}
