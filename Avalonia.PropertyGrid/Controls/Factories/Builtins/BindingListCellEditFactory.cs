using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            if (!IsAcceptType(propertyDescriptor) || propertyDescriptor.GetValue(target) == null)
            {
                return null;
            }

            BindingListEdit control = new BindingListEdit();
            control.Model.Collection = (this as ICellEditFactory).Collection;

            Debug.Assert(control.Model.Collection != null);

            control.NewElement += (s, e) => HandleNewElement(s, e, target, propertyDescriptor, control);
            control.InsertElement += (s, e) => HandleInsertElement(s, e, target, propertyDescriptor, control);
            control.RemoveElement += (s, e) => HandleRemoveElement(s, e, target, propertyDescriptor, control);
            control.ClearElements += (s, e) => HandleClearElements(s, e, target, propertyDescriptor, control);
            control.ElementValueChanged += (s, e) => HandleElementValueChanged(s, e, target, propertyDescriptor, control);

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (!IsAcceptType(propertyDescriptor))
            {
                return false;
            }

            if (control is BindingListEdit ae)
            {
                var value = propertyDescriptor.GetValue(target) as IBindingList;

                ae.DataList = value;

                return true;
            }

            return false;
        }

        private void HandleRemoveElement(object s, BindingListRoutedEventArgs e, object target, PropertyDescriptor propertyDescriptor, BindingListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = propertyDescriptor.GetValue(target) as IBindingList;

            Debug.Assert(value != null);

            if (value != null && e.Index >= 0 && e.Index < value.Count)
            {
                value.RemoveAt(e.Index);
            }
        }

        private void HandleClearElements(object s, BindingListRoutedEventArgs e, object target, PropertyDescriptor propertyDescriptor, BindingListEdit control)
        {
            var value = propertyDescriptor.GetValue(target) as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                value.Clear();
            }
        }

        private void HandleInsertElement(object s, BindingListRoutedEventArgs e, object target, PropertyDescriptor propertyDescriptor, BindingListEdit control)
        {
            Debug.Assert(e.Index != -1);

            var value = propertyDescriptor.GetValue(target) as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                value.Insert(e.Index, ObjectCreator.Create(GetElementType(propertyDescriptor)));
            }
        }

        private void HandleNewElement(object s, BindingListRoutedEventArgs e, object target, PropertyDescriptor propertyDescriptor, BindingListEdit control)
        {
            var value = propertyDescriptor.GetValue(target) as IBindingList;

            Debug.Assert(value != null);

            if (value != null)
            {
                value.Add(ObjectCreator.Create(GetElementType(propertyDescriptor)));
            }
        }

        private void HandleElementValueChanged(object s, BindingListRoutedEventArgs e, object target, PropertyDescriptor propertyDescriptor, BindingListEdit control)
        {
            propertyDescriptor.RaiseEvent(target);
        }
    }
}
