using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class ColorCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class ColorCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => base.ImportPriority - 100000;

        /// <summary>
        /// Determines whether [is available color type] [the specified property].
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns><c>true</c> if [is available color type] [the specified property]; otherwise, <c>false</c>.</returns>
        public bool IsAvailableColorType(PropertyDescriptor property)
        {
            var type = property.PropertyType;

            return type == typeof(Color) ||
                type == typeof(Avalonia.Media.Color) ||
                type == typeof(Avalonia.Media.HslColor) ||
                type == typeof(Avalonia.Media.HsvColor);
        }

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            
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
                    SetAndRaise(context, colorPicker, c);
                }
                else if(type == typeof(Avalonia.Media.Color))
                {
                    SetAndRaise(context, colorPicker, e.NewColor);
                }
                else if(type == typeof(Avalonia.Media.HslColor))
                {
                    SetAndRaise(context, colorPicker, e.NewColor.ToHsl());
                }
                else if(type == typeof(Avalonia.Media.HsvColor))
                {
                    SetAndRaise(context, colorPicker, e.NewColor.ToHsv());
                }
            };

            return colorPicker;
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