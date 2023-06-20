using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class ColorCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 100000;

        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType != typeof(Color))
            {
                return null;
            }

            ColorPicker colorPicker = new ColorPicker()
            {
                Palette = new MaterialHalfColorPalette(),
                HorizontalAlignment = Layout.HorizontalAlignment.Left
            };

            colorPicker.ColorChanged += (s, e) =>
            {
                Color c = Color.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
                SetAndRaise(colorPicker, propertyDescriptor, target, c);
            };

            return colorPicker;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (propertyDescriptor.PropertyType != typeof(Color))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ColorPicker colorPicker)
            {
                Color color = (Color)propertyDescriptor.GetValue(target);
                colorPicker.Color = Media.Color.FromArgb(color.A, color.R, color.G, color.B);

                return true;
            }

            return false;
        }
    }
}