using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class DateTimeCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 1000000;

        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            var propertyType = propertyDescriptor.PropertyType;
            if(propertyType != typeof(DateTime) &&
                propertyType != typeof(DateTimeOffset) &&
                propertyType != typeof(DateTime?) &&
                propertyType != typeof(DateTimeOffset?))
            {
                return null;
            }

//             if((propertyType != typeof(DateTime) ||
//                 propertyType != typeof(DateTimeOffset)) &&
//                 propertyDescriptor.GetValue(target) == null)
//             {
//                 // we can't support this case
//                 return null;
//             }

            DatePicker control = new DatePicker();
            control.SelectedDateChanged += (s, e) =>
            {
                if(propertyType ==  typeof(DateTime))
                {
                    if(control.SelectedDate !=null && control.SelectedDate.HasValue)
                    {
                        SetAndRaise(control, propertyDescriptor, target, control.SelectedDate.Value.DateTime);
                    }                    
                }
                else if(propertyType == typeof(DateTimeOffset))
                {
                    if (control.SelectedDate != null && control.SelectedDate.HasValue)
                    {
                        SetAndRaise(control, propertyDescriptor, target, control.SelectedDate.Value);
                    }                        
                }
                else if (propertyType == typeof(DateTime?))
                {
                    SetAndRaise(control, propertyDescriptor, target, control.SelectedDate?.DateTime);
                }
                else if (propertyType == typeof(DateTimeOffset?))
                {
                    SetAndRaise(control, propertyDescriptor, target, control.SelectedDate);
                }
            };

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            var propertyType = propertyDescriptor.PropertyType;
            if (propertyType != typeof(DateTime) &&
                propertyType != typeof(DateTimeOffset))
            {
                return false;
            }

            if(control is DatePicker dp)
            {
                if (propertyType == typeof(DateTime))
                {
                    var value = propertyDescriptor.GetValue(target);
                    dp.SelectedDate = value != null?new DateTimeOffset((DateTime)value):null;
                }
                else if(propertyType == typeof(DateTimeOffset))
                {
                    var value = propertyDescriptor.GetValue(target);
                    dp.SelectedDate = value != null ?(DateTimeOffset)value : null;
                }
                else if (propertyType == typeof(DateTime?))
                {
                    DateTime? dt = propertyDescriptor.GetValue(target) as DateTime?;
                    dp.SelectedDate = dt != null?new DateTimeOffset(dt.Value):null;
                }
                else if (propertyType == typeof(DateTimeOffset?))
                {
                    DateTimeOffset? dt = propertyDescriptor.GetValue(target) as DateTimeOffset?;
                    dp.SelectedDate = dt;
                }

                return true;
            }

            return false;
        }
    }
}
