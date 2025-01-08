using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class BindingListCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class BindingListCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => base.ImportPriority - 1000000;

        /// <summary>
        /// Determines whether [is acceptable type] [the specified pd].
        /// </summary>
        /// <param name="pd">The pd.</param>
        /// <returns><c>true</c> if [is acceptable type] [the specified pd]; otherwise, <c>false</c>.</returns>
        private static bool IsAcceptableType(PropertyDescriptor pd)
        {
            var type = GetElementType(pd);

            return type != null;
        }

        /// <summary>
        /// Determines whether [is editable type] [the specified pd].
        /// </summary>
        /// <param name="pd">The pd.</param>
        /// <returns><c>true</c> if [is editable type] [the specified pd]; otherwise, <c>false</c>.</returns>
        private static bool IsEditableType(PropertyDescriptor pd)
        {
            var type = GetElementType(pd);

            return type is { IsAbstract: false };
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="pd">The pd.</param>
        /// <returns>Type.</returns>
        private static Type? GetElementType(PropertyDescriptor pd)
        {
            if (pd.PropertyType.IsGenericType && pd.PropertyType.GetGenericTypeDefinition() == typeof(BindingList<>))
            {
                return pd.PropertyType.GetGenericArguments()[0];
            }

            return null;
        }

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            if (!IsAcceptableType(context.Property) || context.Property.GetValue(context.Target) == null)
            {
                return null;
            }

            var control = new ListEdit
            {
                Model =
                {
                    PropertyContext = context,
                    Collection = (this as ICellEditFactory).Collection,
                    IsReadOnly = context.IsReadOnly
                }
            };

            var attr = context.Property.GetCustomAttribute<EditableAttribute>();
            
            if(attr is { AllowEdit: false } || context.IsReadOnly || !IsEditableType(context.Property))
            {
                control.Model.IsEditable = false;
            }

            Debug.Assert(control.Model.Collection != null);

            control.NewElement += (s, e) => HandleNewElement(s, e, context, control);
            control.InsertElement += (s, e) => HandleInsertElement(s, e, context, control);
            control.RemoveElement += (s, e) => HandleRemoveElement(s, e, context, control);
            control.ClearElements += (s, e) => HandleClearElements(s, e, context, control);
            control.ElementValueChanged += (s, e) => HandleElementValueChanged(s, e, context, control);

            return control;
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            if (!IsAcceptableType(context.Property))
            {
                return false;
            }

            if (context.CellEdit is ListEdit ae)
            {
                var value = context.GetValue() as IBindingList;

                ae.DataList = value;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles readonly flag changed
        /// </summary>
        /// <param name="control">control.</param>
        /// <param name="readOnly">readonly flag</param>
        /// <returns>Control.</returns>
        public override void HandleReadOnlyStateChanged(Control control, bool readOnly)
        {
            if (control is ListEdit ae)
            {
                ae.Model.IsReadOnly = readOnly;
            }
            else
            {
                base.HandleReadOnlyStateChanged(control, readOnly);
            }
        }

        /// <summary>
        /// Handles the remove element.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="ListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleRemoveElement(object? s, ListRoutedEventArgs e, PropertyCellContext context, ListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (e.Index >= 0 && e.Index < value.Count)
            {
                var oldElement = value[e.Index];

                var command = new GenericCancelableCommand(
                    string.Format(LocalizationService.Default["Remove array element at {0}"], e.Index),
                    () =>
                    {
                        if (e.Index >= 0 && e.Index < value.Count)
                        {
                            value.RemoveAt(e.Index);

                            HandleRaiseEvent(context.CellEdit!, context);

                            return true;
                        }

                        return false;
                    },
                    () =>
                    {
                        if (e.Index < value.Count)
                        {
                            value.Insert(e.Index, oldElement);

                            HandleRaiseEvent(context.CellEdit!, context);

                            return true;
                        }

                        return false;
                    },
                    () => e.Index >= 0 && e.Index < value.Count,
                    () => e.Index >= 0 && e.Index < value.Count)
                { 
                    Tag = "Remove"
                };

                ExecuteCommand(command, context, value, value, oldElement);                
            }            
        }

        /// <summary>
        /// Handles the clear elements.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="ListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleClearElements(object? s, ListRoutedEventArgs e, PropertyCellContext context, ListEdit control)
        {
            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            List<object> list = [.. value];

            var command = new GenericCancelableCommand(
                LocalizationService.Default["Clear all elements of the array"],
                () =>
                {
                    value.Clear();

                    HandleRaiseEvent(context.CellEdit!, context);

                    return true;
                },
                () =>
                {
                    foreach (var l in list)
                    {
                        value.Add(l);
                    }

                    HandleRaiseEvent(context.CellEdit!, context);

                    return true;
                })
            {
                Tag = "Clear"
            };

            ExecuteCommand(command, context, value, value, list);
        }

        /// <summary>
        /// Handles the insert element.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="ListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleInsertElement(object? s, ListRoutedEventArgs e, PropertyCellContext context, ListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            var newElement = ObjectCreator.Create(GetElementType(context.Property)!);

            var command = new GenericCancelableCommand(
                string.Format(LocalizationService.Default["Insert a new element at {0}"], e.Index),
                () =>
                {
                    value.Insert(e.Index, newElement);

                    HandleRaiseEvent(context.CellEdit!, context);

                    return true;
                },
                () =>
                {
                    value.RemoveAt(e.Index);

                    HandleRaiseEvent(context.CellEdit!, context);

                    return true;
                },
                () => e.Index >= 0 && e.Index <= value.Count,
                () => e.Index >= 0 && e.Index < value.Count)
            {
                Tag = "Insert"
            };

            ExecuteCommand(command, context, value, value, newElement);     
        }

        /// <summary>
        /// Handles the new element.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="ListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleNewElement(object? s, ListRoutedEventArgs e, PropertyCellContext context, ListEdit control)
        {
            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            var newElement = ObjectCreator.Create(GetElementType(context.Property)!);

            var command = new GenericCancelableCommand(
                LocalizationService.Default["Insert a new element at the end of the array"],
                () =>
                {
                    value.Add(newElement);

                    HandleRaiseEvent(context.CellEdit!, context);

                    return true;
                },
                () =>
                {
                    value.RemoveAt(value.Count - 1);

                    HandleRaiseEvent(context.CellEdit!, context);

                    return true;
                },
                null,
                () => value.Count > 0
            )
            {
                Tag = "NewElement"
            };

            ExecuteCommand(command, context, value, value, newElement);    
        }

        /// <summary>
        /// Handles the element value changed.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="ListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleElementValueChanged(object? s, ListRoutedEventArgs e, PropertyCellContext context, ListEdit control)
        {
            // element has been changed
            // we just raise event, so property grid can refresh ui...
            context.Property.RaiseEvent(context.Target);
        }
    }
}
