using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class PropertyGridBindingListFactory : AbstractPropertyGridFactory
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
            if (!IsAcceptType(propertyDescriptor))
            {
                return null;
            }

            BindingListEdit control = new BindingListEdit();
            control.Model.Collection = (this as IPropertyGridControlFactory).Collection;

            Debug.Assert(control.Model.Collection != null);

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
    }
}
