using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class TimeCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 100000;

        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            var propertyType = propertyDescriptor.PropertyType;

            if(propertyType != typeof(TimeSpan) && propertyType != typeof(TimeSpan?))
            {
                return null;
            }

            TimePicker control = new TimePicker();
            control.ClockIdentifier = "24HourClock";

            control.SelectedTimeChanged += (s, e) =>
            {
                if (propertyType == typeof(TimeSpan?))
                {
                    SetAndRaise(control, propertyDescriptor, target, control.SelectedTime);
                }
                else if (propertyType == typeof(TimeSpan) && control.SelectedTime != null && control.SelectedTime.HasValue)
                {
                    SetAndRaise(control, propertyDescriptor, target, control.SelectedTime.Value);
                }
            };

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            var propertyType = propertyDescriptor.PropertyType;

            if (propertyType != typeof(TimeSpan) && propertyType != typeof(TimeSpan?))
            {
                return false;
            }

            if(control is TimePicker tp)
            {
                if(propertyType ==  typeof(TimeSpan?))
                {
                    tp.SelectedTime = propertyDescriptor.GetValue(target) as TimeSpan?;
                }
                else if(propertyType == typeof(TimeSpan))
                {
                    var value = propertyDescriptor.GetValue(target);

                    if(value != null)
                    {
                        tp.SelectedTime = (TimeSpan)value;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
