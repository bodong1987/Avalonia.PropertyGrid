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

        public bool IsAvailableColorType(PropertyDescriptor property)
        {
            var type = property.PropertyType;

            return type == typeof(Color) ||
                type == typeof(Avalonia.Media.Color) ||
                type == typeof(Avalonia.Media.HslColor) ||
                type == typeof(Avalonia.Media.HsvColor);
        }

        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
            if (!IsAvailableColorType(propertyDescriptor))
            {
                return null;
            }

            var type = propertyDescriptor.PropertyType;

            ColorPicker colorPicker = new ColorPicker()
            {
                Palette = new MaterialHalfColorPalette(),
                HorizontalAlignment = Layout.HorizontalAlignment.Left
            };

            colorPicker.ColorChanged += (s, e) =>
            {
                if(type == typeof(Color))
                {
                    Color c = Color.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
                    SetAndRaise(colorPicker, propertyDescriptor, target, c);
                }
                else if(type == typeof(Avalonia.Media.Color))
                {
                    SetAndRaise(colorPicker, propertyDescriptor, target, e.NewColor);
                }
                else if(type == typeof(Avalonia.Media.HslColor))
                {
                    SetAndRaise(colorPicker, propertyDescriptor, target, e.NewColor.ToHsl());
                }
                else if(type == typeof(Avalonia.Media.HsvColor))
                {
                    SetAndRaise(colorPicker, propertyDescriptor, target, e.NewColor.ToHsv());
                }
            };

            return colorPicker;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (!IsAvailableColorType(propertyDescriptor))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            var type = propertyDescriptor.PropertyType;

            if (control is ColorPicker colorPicker)
            {
                if (type == typeof(Color))
                {
                    Color color = (Color)propertyDescriptor.GetValue(target);
                    colorPicker.Color = Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                }
                else if (type == typeof(Avalonia.Media.Color))
                {
                    Media.Color color = (Media.Color)propertyDescriptor.GetValue(target);

                    colorPicker.Color = color;
                }
                else if (type == typeof(Avalonia.Media.HslColor))
                {
                    Media.HslColor color = (Media.HslColor)propertyDescriptor.GetValue(target);

                    colorPicker.Color = color.ToRgb();
                }
                else if (type == typeof(Avalonia.Media.HsvColor))
                {
                    Media.HsvColor color = (Media.HsvColor)propertyDescriptor.GetValue(target);

                    colorPicker.Color = color.ToRgb();
                }

                return true;
            }

            return false;
        }
    }
}