using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class ExpandableCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => int.MinValue + 10000;

        private bool IsExpandableType(PropertyDescriptor property)
        {
            if (!property.PropertyType.IsClass)
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

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor is MultiObjectPropertyDescriptor)
            {
                return null;
            }

            if (!IsExpandableType(propertyDescriptor))
            {
                return null;
            }

            // avoid recursive expansion
            var value = propertyDescriptor.GetValue(target);            
            if (context.Root.GetExpandableObjectCache().IsExists(value))
            {
                return null;
            }

            Border border = new Border();
            border.BorderBrush = Brushes.Gray;
            border.BorderThickness = new Thickness(0.5);
            border.CornerRadius = new CornerRadius(0, 0, 5, 5);
            border.Margin = new Thickness(0);
                        
            PropertyGrid propertyGrid = context.Root.ClonePropertyGrid() as PropertyGrid;
            propertyGrid.RootPropertyGrid = context.Root;

            Debug.Assert(propertyGrid.RootPropertyGrid != null);
            Debug.Assert(propertyGrid.RootPropertyGrid != propertyGrid);

            propertyGrid.ShowStyle = context.Root.ShowStyle;
            propertyGrid.AllowFilter = false;
            propertyGrid.AllowQuickFilter = false;
            propertyGrid.ShowTitle = false;

            border.Child = propertyGrid;

            // avoid recursive expansion
            propertyGrid.GetExpandableObjectCache().Merge(context.Root.GetExpandableObjectCache());
            propertyGrid.GetExpandableObjectCache().Add(value);

            return border;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

            if (propertyDescriptor is MultiObjectPropertyDescriptor)
            {
                return false;
            }

            if (!IsExpandableType(propertyDescriptor))
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

        public override PropertyVisibility? HandlePropagateVisibility(object target, PropertyDescriptor propertyDescriptor, Control control, IPropertyGridFilterContext filterContext)
        {
            if(!IsExpandableType(propertyDescriptor))
            {
                return null;
            }

            if (control is Border border && border.Child is PropertyGrid pg)
            {
                var category = FilterCategory.Default;
                category &= ~FilterCategory.Category;

                return pg.FilterCells(filterContext, category);
            }

            return null;
        }
    }
}
