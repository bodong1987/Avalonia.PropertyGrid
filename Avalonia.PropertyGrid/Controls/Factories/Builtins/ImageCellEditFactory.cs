using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class ImageCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 1000000;

        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
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

        Avalonia.Media.Imaging.Bitmap ConvertImageToAvaloniaBitmap(System.Drawing.Image image)
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

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (propertyDescriptor.PropertyType != typeof(Avalonia.Media.IImage) &&
                propertyDescriptor.PropertyType != typeof(System.Drawing.Image))
            {
                return false;
            }

            if(control is Image imageControl)
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
