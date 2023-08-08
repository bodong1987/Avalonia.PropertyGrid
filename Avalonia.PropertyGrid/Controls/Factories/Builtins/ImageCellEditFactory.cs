using Avalonia.Controls;
using PropertyModels.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class ImageCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class ImageCellEditFactory : AbstractCellEditFactory
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

            if (propertyDescriptor is MultiObjectPropertyDescriptor)
            {
                return null;
            }

            if (propertyDescriptor.PropertyType != typeof(Avalonia.Media.IImage))
            {
                return null;
            }

            Image control = new Image();
            control.VerticalAlignment = Layout.VerticalAlignment.Center;
            control.HorizontalAlignment = Layout.HorizontalAlignment.Center;            

            return control;
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

            if (propertyDescriptor.PropertyType != typeof(Avalonia.Media.IImage))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is Image imageControl)
            {
                object imageData = propertyDescriptor.GetValue(target);

                if (imageData == null)
                {
                    imageControl.Source = null;
                    return false;
                }

                if(imageData is Media.IImage iImage)
                {
                    imageControl.Source = iImage;
                }
            }

            return false;
        }
    }
}
