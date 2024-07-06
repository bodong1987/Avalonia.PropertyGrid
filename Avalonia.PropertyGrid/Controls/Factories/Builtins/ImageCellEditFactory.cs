﻿using Avalonia.Controls;
using PropertyModels.ComponentModel;

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
            // var target = context.Target;

            if (propertyDescriptor is MultiObjectPropertyDescriptor)
            {
                return null;
            }

            if (propertyDescriptor.PropertyType != typeof(Media.IImage))
            {
                return null;
            }

            var control = new Image
            {
                VerticalAlignment = Layout.VerticalAlignment.Center,
                HorizontalAlignment = Layout.HorizontalAlignment.Center
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

            if (propertyDescriptor.PropertyType != typeof(Media.IImage))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is Image imageControl)
            {
                var imageData = propertyDescriptor.GetValue(target);

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
