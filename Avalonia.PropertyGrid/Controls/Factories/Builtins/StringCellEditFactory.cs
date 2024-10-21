using Avalonia.Controls;
using Avalonia.PropertyGrid.Localization;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.Utils;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class StringCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class StringCellEditFactory : AbstractCellEditFactory
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
        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            
            if (propertyDescriptor.PropertyType != typeof(string))
            {
                return null;
            }

            var watermarkAttr = propertyDescriptor.GetCustomAttribute<WatermarkAttribute>();

            TextBox control = new TextBox();
            control.Text = propertyDescriptor.GetValue(target) as string;
            control.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            control.FontFamily = FontUtils.DefaultFontFamily;

            if (watermarkAttr != null)
            {
                // control.Watermark = LocalizationService.Default[watermarkAttr.Watermark];
                control.SetLocalizeBinding(TextBox.WatermarkProperty, watermarkAttr.Watermark);
            }

            if (propertyDescriptor.GetCustomAttribute<PasswordPropertyTextAttribute>() is PasswordPropertyTextAttribute ppt && ppt.Password)
            {
                control.PasswordChar = '*';
            }

            var multilineAttr = propertyDescriptor.GetCustomAttribute<MultilineTextAttribute>();

            if (multilineAttr != null && multilineAttr.IsMultiline)
            {
                control.TextWrapping = Media.TextWrapping.Wrap;
                control.AcceptsReturn = true;
                control.AcceptsTab = true;
            }

            control.PropertyChanged += (s, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                {
                    if (e.NewValue as string != propertyDescriptor.GetValue(target) as string)
                    {
                        SetAndRaise(context, control, e.NewValue);
                    }
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
            var control = context.CellEdit!;

            if (propertyDescriptor.PropertyType != typeof(string))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is TextBox textBox)
            {
                textBox.Text = propertyDescriptor.GetValue(target) as string;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles readonly flag changed
        /// </summary>
        /// <param name="control">control.</param>
        /// <param name="readOnly">readonly flag</param>
        /// <returns>Control.</returns>
        public override void HandleReadOnlyStateChanged(Control control, bool readOnly)
        {
            if(control is TextBox textBox)
            {
                textBox.IsReadOnly = readOnly;
            }
            else
            {
                base.HandleReadOnlyStateChanged(control, readOnly);
            }            
        }
    }
}
