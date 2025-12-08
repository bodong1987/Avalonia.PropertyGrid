using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Services;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins;

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

        var list = propertyDescriptor.GetValue(target) as ISelectableList;

        var attr = propertyDescriptor.GetCustomAttribute<SingleSelectionModeAttribute>();

        if (attr == null || attr.Mode == SingleSelectionMode.ComboBox)
        {
            var control = new ComboBox
            {
                ItemsSource = list?.Values,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            control.SelectionChanged += (s, e) =>
            {
                var item = control.SelectedItem;

                if (list != null && (list.SelectedValue?.Equals(item) != true))
                {
                    var oldValue = list.SelectedValue;

                    var command = new GenericCancelableCommand(
                        string.Format(LocalizationService.Default["Change {0} selection from {1} to {2}"], context.Property.DisplayName, oldValue != null ? oldValue.ToString() : "null", item != null ? item.ToString() : "null"),
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

                    _ = ExecuteCommand(command, context, list, list, oldValue);

                    HandleRaiseEvent(context, control);
                }
            };

            if (list != null)
            {
                list.SelectionChanged += (s, e) =>
                {
                    if (!CheckEqual(control.SelectedItem, list.SelectedValue))
                    {
                        control.SelectedItem = list.SelectedValue;
                    }
                };
            }

            return control;
        }
        else
        {
            ICheckableListEdit control = attr.Mode == SingleSelectionMode.ToggleButtonGroup ? new ToggleButtonGroupListEdit
            {
                Items = list?.Values ?? [],
                ElementMinWidth = attr.ElementMinWidth,
                ElementMinHeight = attr.ElementMinHeight
            } : new RadioButtonListEdit
            {
                Items = list?.Values ?? [],
                ElementMinWidth = attr.ElementMinWidth,
                ElementMinHeight = attr.ElementMinHeight
            };

            if (propertyDescriptor.GetCustomAttribute<SelectableListDisplayModeAttribute>() is { } displayModeAttr)
            {
                control.DisplayMode = displayModeAttr.DisplayMode;
            }

            control.CheckChanged += (s, e) =>
            {
                var item = control.CheckedItem;

                if (list != null && (list.SelectedValue?.Equals(item) != true))
                {
                    var oldValue = list.SelectedValue;

                    var command = new GenericCancelableCommand(
                        string.Format(LocalizationService.Default["Change {0} selection from {1} to {2}"], context.Property.DisplayName, oldValue != null ? oldValue.ToString() : "null", item != null ? item.ToString() : "null"),
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

                    _ = ExecuteCommand(command, context, list, list, oldValue);

                    HandleRaiseEvent(context, (control as Control)!);
                }
            };

            if (list != null)
            {
                list.SelectionChanged += (s, e) =>
                {
                    if (!CheckEqual(control.CheckedItem, list.SelectedValue))
                    {
                        control.CheckedItem = list.SelectedValue;
                    }
                };
            }

            return control as Control;
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

        if (!propertyDescriptor.PropertyType.IsImplementFrom<ISelectableList>())
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is ComboBox cb)
        {
            var list = propertyDescriptor.GetValue(target) as ISelectableList;

            if (!CheckEquals(cb.ItemsSource as object[], list?.Values))
            {
                cb.ItemsSource = list?.Values;
            }

            if (!CheckEqual(cb.SelectedItem, list?.SelectedValue))
            {
                cb.SelectedItem = list?.SelectedValue;
            }

            return true;
        }

        if (control is ICheckableListEdit ce)
        {
            var list = propertyDescriptor.GetValue(target) as ISelectableList;

            if (!CheckEquals(ce.Items, list?.Values))
            {
                ce.Items = list?.Values ?? [];
            }

            if (!CheckEqual(ce.CheckedItem, list?.SelectedValue))
            {
                ce.CheckedItem = list?.SelectedValue;
            }

            return true;
        }

        return false;
    }
}