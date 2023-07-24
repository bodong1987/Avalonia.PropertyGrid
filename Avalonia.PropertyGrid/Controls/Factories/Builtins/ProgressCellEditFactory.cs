using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class ProgressCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 900000;

        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
            if(propertyDescriptor.PropertyType != typeof(double) || !propertyDescriptor.IsDefined<ProgressAttribute>())
            {
                return null;
            }

            ProgressBar control = new ProgressBar();

            var attr = propertyDescriptor.GetCustomAttribute<ProgressAttribute>();
            control.Minimum = attr.Minimum; 
            control.Maximum = attr.Maximum;
            control.ProgressTextFormat = attr.FormatString;
            control.ShowProgressText = attr.ShowProgressText;

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (propertyDescriptor.PropertyType != typeof(double) || !propertyDescriptor.IsDefined<ProgressAttribute>())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ProgressBar progressBar)
            {
                double value = (double)propertyDescriptor.GetValue(target);

                progressBar.Value = value;

                return true;
            }

            return false;
        }
    }
}
