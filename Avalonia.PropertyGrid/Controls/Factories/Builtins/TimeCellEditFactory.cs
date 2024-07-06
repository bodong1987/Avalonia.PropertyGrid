using Avalonia.Controls;
using System;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class TimeCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class TimeCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => base.ImportPriority - 100000;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            //var target = context.Target;
            
            var propertyType = propertyDescriptor.PropertyType;

            if(propertyType != typeof(TimeSpan) && propertyType != typeof(TimeSpan?))
            {
                return null;
            }

            var control = new TimePicker
            {
                ClockIdentifier = "24HourClock"
            };

            control.SelectedTimeChanged += (_, _) =>
            {
                if (propertyType == typeof(TimeSpan?))
                {
                    SetAndRaise(context, control, control.SelectedTime);
                }
                else if (propertyType == typeof(TimeSpan) && control.SelectedTime is not null)
                {
                    SetAndRaise(context, control, control.SelectedTime.Value);
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
