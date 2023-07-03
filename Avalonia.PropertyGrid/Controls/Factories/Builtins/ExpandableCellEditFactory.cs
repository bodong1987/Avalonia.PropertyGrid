using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class ExpandableCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => int.MinValue + 10000;

        private bool IsExpandableType(PropertyDescriptor property)
        {
            if(!property.PropertyType.IsClass)
            {
                return false;
            }    

            var attr = property.GetCustomAttribute<TypeConverterAttribute>();

            if(attr != null && attr.GetConverterType().IsChildOf<ExpandableObjectConverter>())
            {
                return true;
            }

            attr = property.PropertyType.GetCustomAttribute<TypeConverterAttribute>();

            return attr != null && attr.GetConverterType().IsChildOf<ExpandableObjectConverter>();
        }

        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
            if(!IsExpandableType(propertyDescriptor))
            {
                return null;
            }

            return null;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            throw new NotImplementedException();
        }
    }
}
