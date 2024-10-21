using Avalonia.Controls;
using Avalonia.PropertyGrid.Localization;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using Avalonia.PropertyGrid.Utils;
using Avalonia.VisualTree;
using System.Linq;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class PathCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class PathCellEditFactory : AbstractCellEditFactory
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
        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor.PropertyType != typeof(string) || !propertyDescriptor.IsDefined<PathBrowsableAttribute>())
            {
                return null;
            }

            var attribute = propertyDescriptor.GetCustomAttribute<PathBrowsableAttribute>()!;
            var watermarkAttr = propertyDescriptor.GetCustomAttribute<WatermarkAttribute>();

            ButtonEdit control = new ButtonEdit();
            control.Text = propertyDescriptor.GetValue(target) as string;

            if (watermarkAttr != null)
            {
                // control.Watermark = LocalizationService.Default[watermarkAttr.Watermark];
                control.SetLocalizeBinding(ButtonEdit.WatermarkProperty, watermarkAttr.Watermark);
            }

            control.ButtonClick += async (s, e) =>
            {
                if (attribute.IsDirectorySelection && string.IsNullOrEmpty(attribute.InitialFileName))
                {
                    attribute.InitialFileName = control.Text;
                }

                var files = await PathBrowserUtils.ShowPathBrowserAsync((control.GetVisualRoot() as Window)!, attribute);

                if (files != null && files.Length > 0)
                {
                    var file = files.FirstOrDefault();

                    if (file != propertyDescriptor.GetValue(target) as string)
                    {
                        SetAndRaise(context, control, file);
                        control.Text = file;
                    }
                }
            };

            control.TextChanged += (s, e) =>
            {
                if (control.Text != propertyDescriptor.GetValue(target) as string)
                {
                    SetAndRaise(context, control, control.Text);
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

            if (propertyDescriptor.PropertyType != typeof(string) || !propertyDescriptor.IsDefined<PathBrowsableAttribute>())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ButtonEdit be)
            {
                be.Text = propertyDescriptor.GetValue(target) as string;

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
            if (control is ButtonEdit be)
            {
                be.IsReadOnly = readOnly;
            }
            else
            {
                base.HandleReadOnlyStateChanged(control, readOnly);
            }
        }
    }
}
