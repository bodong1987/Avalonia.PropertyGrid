using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class BindingListCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 1000000;

        /// <summary>
        /// Determines whether [is accept type] [the specified pd].
        /// </summary>
        /// <param name="pd">The pd.</param>
        /// <returns><c>true</c> if [is accept type] [the specified pd]; otherwise, <c>false</c>.</returns>
        private bool IsAcceptType(PropertyDescriptor pd)
        {
            var type = GetElementType(pd);

            return type != null && !type.IsAbstract;
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="pd">The pd.</param>
        /// <returns>Type.</returns>
        private Type GetElementType(PropertyDescriptor pd)
        {
            if (pd.PropertyType.IsGenericType && pd.PropertyType.GetGenericTypeDefinition() == typeof(BindingList<>))
            {
                return pd.PropertyType.GetGenericArguments()[0];
            }

            return null;
        }

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            if (!IsAcceptType(context.Property) || context.Property.GetValue(context.Target) == null)
            {
                return null;
            }

            BindingListEdit control = new BindingListEdit();
            control.Model.PropertyContext = context;
            control.Model.Collection = (this as ICellEditFactory).Collection;            

            var attr = context.Property.GetCustomAttribute<EditableAttribute>();
            
            if((attr != null && !attr.AllowEdit) || context.Property.IsReadOnly)
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

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            if (!IsAcceptType(context.Property))
            {
                return false;
            }

            if (context.CellEdit is BindingListEdit ae)
            {
                var value = context.GetValue() as IBindingList;

                ae.DataList = value;

                return true;
            }

            return false;
        }

        private void HandleRemoveElement(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (value != null && e.Index >= 0 && e.Index < value.Count)
            {
                var oldElement = value[e.Index];

                GenericCancelableCommand command = new GenericCancelableCommand(
                    string.Format(PropertyGrid.LocalizationService["Remove array element at {0}"], e.Index),
                    () =>
                    {
                        if (value != null && e.Index >= 0 && e.Index < value.Count)
                        {
                            value.RemoveAt(e.Index);

                            HandleRaiseEvent(context.CellEdit, context);

                            return true;
                        }

                        return false;
                    },
                    () =>
                    {
                        if (e.Index < value.Count)
                        {
                            value.Insert(e.Index, oldElement);

                            HandleRaiseEvent(context.CellEdit, context);

                            return true;
                        }

                        return false;
                    },
                    () =>
                    {
                        return value != null && e.Index >= 0 && e.Index < value.Count;
                    },
                    () =>
                    {
                        return e.Index >= 0 && e.Index < value.Count;
                    }
                )
                { 
                    Tag = "Remove"
                };

                ExecuteCommand(command, context, value, value, oldElement);                
            }            
        }

        private void HandleClearElements(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                List<object> list = new List<object>();
                foreach(var obj in value)
                {
                    list.Add(obj);
                }

                GenericCancelableCommand command = new GenericCancelableCommand(
                    PropertyGrid.LocalizationService["Clear all elements of the array"],
                    () =>
                    {
                        value.Clear();

                        HandleRaiseEvent(context.CellEdit, context);

                        return true;
                    },
                    () =>
                    {
                        foreach (var l in list)
                        {
                            value.Add(l);
                        }

                        HandleRaiseEvent(context.CellEdit, context);

                        return true;
                    })
                {
                    Tag = "Clear"
                };

                ExecuteCommand(command, context, value, value, list);
            }
        }

        private void HandleInsertElement(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                var NewElement = ObjectCreator.Create(GetElementType(context.Property));

                GenericCancelableCommand command = new GenericCancelableCommand(
                    string.Format(PropertyGrid.LocalizationService["Insert a new element at {0}"], e.Index),
                    () =>
                    {
                        value.Insert(e.Index, NewElement);

                        HandleRaiseEvent(context.CellEdit, context);

                        return true;
                    },
                    () =>
                    {
                        value.RemoveAt(e.Index);

                        HandleRaiseEvent(context.CellEdit, context);

                        return true;
                    },
                    () =>
                    {
                        return e.Index >= 0 && e.Index <= value.Count;
                    },
                    () =>
                    {
                        return e.Index >= 0 && e.Index < value.Count;
                    }
                    )
                {
                    Tag = "Insert"
                };

                ExecuteCommand(command, context, value, value, NewElement);                
            }
        }

        private void HandleNewElement(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                var NewElement = ObjectCreator.Create(GetElementType(context.Property));

                GenericCancelableCommand command = new GenericCancelableCommand(
                    PropertyGrid.LocalizationService["Insert a new element at the end of the array"],
                    () =>
                    {
                        value.Add(NewElement);

                        HandleRaiseEvent(context.CellEdit, context);

                        return true;
                    },
                    () =>
                    {
                        value.RemoveAt(value.Count - 1);

                        HandleRaiseEvent(context.CellEdit, context);

                        return true;
                    },
                    null,
                    () => value.Count > 0
                    )
                {
                    Tag = "NewElement"
                };

                ExecuteCommand(command, context, value, value, NewElement);                
            }
        }

        private void HandleElementValueChanged(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            // element has been changed
            // we just raise event, so property grid can refresh ui...
            context.Property.RaiseEvent(context.Target);
        }
    }
}
