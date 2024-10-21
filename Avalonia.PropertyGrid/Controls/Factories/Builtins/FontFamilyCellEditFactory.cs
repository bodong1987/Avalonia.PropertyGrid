using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class FontFamilyCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class FontFamilyCellEditFactory : AbstractCellEditFactory
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

            if (propertyDescriptor.PropertyType != typeof(FontFamily))
            {
                return null;
            }

            ComboBox control = new ComboBox();
            control.ItemsSource = FontManager.Current.SystemFonts.ToArray();
            control.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;
            control.ItemTemplate = new FuncDataTemplate<FontFamily>((value, namescope) =>            
                new TextBlock
                {
                    [!TextBlock.TextProperty] = new Binding("Name"),
                    [!TextBlock.FontFamilyProperty] = new Binding()
                }
            );

            control.SelectionChanged += (s, e) =>
            {
                var item = control.SelectedItem;

                if (item is FontFamily ff)
                {                    
                    if (ff != propertyDescriptor.GetValue(target) as FontFamily)
                    {
                        SetAndRaise(context, control, ff);
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

            if (propertyDescriptor.PropertyType != typeof(FontFamily))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ComboBox cb)
            {
                var value = propertyDescriptor.GetValue(target) as FontFamily;
                cb.SelectedItem = value;
                return true;
            }

            return false;
        }
    }
}
