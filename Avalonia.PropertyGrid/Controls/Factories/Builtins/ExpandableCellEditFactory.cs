using Avalonia.Controls;
using Avalonia.Media;
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

            // avoid recursive expansion
            var value = propertyDescriptor.GetValue(target);            
            if (rootPropertyGrid.GetExpandableObjectCache().IsExists(value))
            {
                return null;
            }

            Border border = new Border();
            border.BorderBrush = Brushes.Gray;
            border.BorderThickness = new Thickness(0.5);
            border.CornerRadius = new CornerRadius(0, 0, 5, 5);
            border.Margin = new Thickness(0);
                        
            PropertyGrid propertyGrid = rootPropertyGrid.ClonePropertyGrid() as PropertyGrid;

            propertyGrid.ShowStyle = rootPropertyGrid.ShowStyle;
            propertyGrid.AllowFilter = false;
            propertyGrid.ShowTitle = false;

            border.Child = propertyGrid;

            // avoid recursive expansion
            propertyGrid.GetExpandableObjectCache().Merge(rootPropertyGrid.GetExpandableObjectCache());
            propertyGrid.GetExpandableObjectCache().Add(value);

            return border;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if(!IsExpandableType(propertyDescriptor))
            {
                return false;
            }

            if(control is Border border && border.Child is PropertyGrid pg)
            {
                var value = propertyDescriptor.GetValue(target);

                pg.SelectedObject = value;

                return true;
            }

            return false;
        }
    }
}
