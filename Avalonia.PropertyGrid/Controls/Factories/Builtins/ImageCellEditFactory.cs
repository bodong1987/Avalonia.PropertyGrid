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

            if (propertyDescriptor.PropertyType != typeof(Avalonia.Media.IImage) &&
                propertyDescriptor.PropertyType != typeof(System.Drawing.Image))
            {
                return null;
            }

            Image control = new Image();
            control.VerticalAlignment = Layout.VerticalAlignment.Center;
            control.HorizontalAlignment = Layout.HorizontalAlignment.Center;            

            return control;
        }

        /// <summary>
        /// Converts the image to avalonia bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Avalonia.Media.Imaging.Bitmap.</returns>
        protected Avalonia.Media.Imaging.Bitmap ConvertImageToAvaloniaBitmap(System.Drawing.Image image)
        {
            if (image == null)
                return null;
            using (System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(image))
            {
                var bitmapdata = bitmapTmp.LockBits(new System.Drawing.Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                var bitmap1 = new Avalonia.Media.Imaging.Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
                    bitmapdata.Scan0,
                    new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
                    new Avalonia.Vector(96, 96),
                    bitmapdata.Stride
                    );

                bitmapTmp.UnlockBits(bitmapdata);

                return bitmap1;
            }   
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

            if (propertyDescriptor.PropertyType != typeof(Avalonia.Media.IImage) &&
                propertyDescriptor.PropertyType != typeof(System.Drawing.Image))
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
                else if(imageData is System.Drawing.Image iSysImage)
                {
                    imageControl.Source = ConvertImageToAvaloniaBitmap(iSysImage);
                }
            }

            return false;
        }
    }
}
