using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// Determines whether [is accept type] [the specified pd].
        /// </summary>
        /// <param name="pd">The pd.</param>
        /// <returns><c>true</c> if [is accept type] [the specified pd]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsAcceptType(PropertyDescriptor pd)
        {
            var type = GetElementType(pd);

            if(type == null)
            {
                return false;
            }

            if(pd.IsDefined<ObjectElementFactoryTypeAttribute>() || pd.PropertyType.IsDefined<ObjectElementFactoryTypeAttribute>())
            {
                return true;
            }

            return type != null && !type.IsAbstract;
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="pd">The pd.</param>
        /// <returns>Type.</returns>
        protected Type GetElementType(PropertyDescriptor pd)
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
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            if (!IsAcceptType(context.Property) || context.Property.GetValue(context.Target) == null)
            {
                return null;
            }

            ObjectElementFactoryTypeAttribute elementFactoryTypeAttr = context.Property.GetCustomAttribute<ObjectElementFactoryTypeAttribute>();

            if(elementFactoryTypeAttr == null)
            {
                elementFactoryTypeAttr = context.Property.PropertyType.GetCustomAttribute<ObjectElementFactoryTypeAttribute>();
            }

            BindingListEdit control = new BindingListEdit();
            control.Model.PropertyContext = context;
            control.Model.Collection = (this as ICellEditFactory).Collection;
            control.Model.ObjectElementFactory = elementFactoryTypeAttr?.CreateFactory();

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

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Handles the remove element.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="BindingListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleRemoveElement(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (value != null && e.Index >= 0 && e.Index < value.Count)
            {
                var oldElement = value[e.Index];

                GenericCancelableCommand command = new GenericCancelableCommand(
                    string.Format(LocalizationService.Default["Remove array element at {0}"], e.Index),
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

        /// <summary>
        /// Handles the clear elements.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="BindingListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleClearElements(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
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
                    LocalizationService.Default["Clear all elements of the array"],
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

        /// <summary>
        /// Handles the insert element.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="BindingListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleInsertElement(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                var NewElement = ObjectCreator.Create(GetElementType(context.Property));

                GenericCancelableCommand command = new GenericCancelableCommand(
                    string.Format(LocalizationService.Default["Insert a new element at {0}"], e.Index),
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

        /// <summary>
        /// Handles the new element.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="BindingListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleNewElement(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            var value = context.GetValue() as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                var NewElement = ObjectCreator.Create(GetElementType(context.Property));

                GenericCancelableCommand command = new GenericCancelableCommand(
                    LocalizationService.Default["Insert a new element at the end of the array"],
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

        /// <summary>
        /// Handles the element value changed.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The <see cref="BindingListRoutedEventArgs"/> instance containing the event data.</param>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        protected virtual void HandleElementValueChanged(object s, BindingListRoutedEventArgs e, PropertyCellContext context, BindingListEdit control)
        {
            // element has been changed
            // we just raise event, so property grid can refresh ui...
            context.Property.RaiseEvent(context.Target);
        }
    }
}
