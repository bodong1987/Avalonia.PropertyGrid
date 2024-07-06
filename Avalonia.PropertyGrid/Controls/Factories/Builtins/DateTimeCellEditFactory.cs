using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class DateTimeCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class DateTimeCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => base.ImportPriority - 1000000;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            var propertyType = propertyDescriptor.PropertyType;
            if(propertyType != typeof(DateTime) &&
                propertyType != typeof(DateTimeOffset) &&
                propertyType != typeof(DateTime?) &&
                propertyType != typeof(DateTimeOffset?))
            {
                return null;
            }

            DatePicker control = new DatePicker();
            control.SelectedDateChanged += (s, e) =>
            {
                if(propertyType ==  typeof(DateTime))
                {
                    if(control.SelectedDate !=null && control.SelectedDate.HasValue)
                    {
                        SetAndRaise(context, control, control.SelectedDate.Value.DateTime);
                    }                    
                }
                else if(propertyType == typeof(DateTimeOffset))
                {
                    if (control.SelectedDate != null && control.SelectedDate.HasValue)
                    {
                        SetAndRaise(context, control, control.SelectedDate.Value);
                    }                        
                }
                else if (propertyType == typeof(DateTime?))
                {
                    SetAndRaise(context, control, control.SelectedDate?.DateTime);
                }
                else if (propertyType == typeof(DateTimeOffset?))
                {
                    SetAndRaise(context, control, control.SelectedDate);
                }
            };

            return control;
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

            var propertyType = propertyDescriptor.PropertyType;
            if (propertyType != typeof(DateTime) &&
                propertyType != typeof(DateTimeOffset) &&
                propertyType != typeof(DateTime?) &&
                propertyType != typeof(DateTimeOffset?))
            {
                return false;
            }

            if (control is DatePicker dp)
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
