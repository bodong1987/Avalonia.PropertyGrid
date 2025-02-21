using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Color = System.Drawing.Color;

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
        public static bool IsAvailableColorType(PropertyDescriptor property)
        {
            var type = property.PropertyType;

            return type == typeof(Color) ||
                type == typeof(Media.Color) ||
                type == typeof(HslColor) ||
                type == typeof(HsvColor);
        }

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            //var target = context.Target;
            
            if (!IsAvailableColorType(propertyDescriptor))
            {
                return null;
            }

            var type = propertyDescriptor.PropertyType;

            var colorPicker = new ColorPicker
            {
                Palette = new MaterialHalfColorPalette(),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            colorPicker.ColorChanged += (s, e) =>
            {
                if(type == typeof(Color))
                {
                    var c = Color.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
                    SetAndRaise(context, colorPicker, c);
                }
                else if(type == typeof(Media.Color))
                {
                    SetAndRaise(context, colorPicker, e.NewColor);
                }
                else if(type == typeof(HslColor))
                {
                    SetAndRaise(context, colorPicker, e.NewColor.ToHsl());
                }
                else if(type == typeof(HsvColor))
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
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit!;

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
                    var color = (Color)propertyDescriptor.GetValue(target)!;
                    colorPicker.Color = Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                }
                else if (type == typeof(Media.Color))
                {
                    var color = (Media.Color)propertyDescriptor.GetValue(target)!;

                    colorPicker.Color = color;
                }
                else if (type == typeof(HslColor))
                {
                    var color = (HslColor)propertyDescriptor.GetValue(target)!;

                    colorPicker.Color = color.ToRgb();
                }
                else if (type == typeof(HsvColor))
                {
                    var color = (HsvColor)propertyDescriptor.GetValue(target)!;

                    colorPicker.Color = color.ToRgb();
                }

                return true;
            }

            return false;
        }
    }
}