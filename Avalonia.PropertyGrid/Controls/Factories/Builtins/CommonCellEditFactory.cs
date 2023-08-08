using Avalonia.Controls;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using Avalonia.PropertyGrid.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class CommonCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class CommonCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => int.MinValue;

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

            var converter = TypeDescriptor.GetConverter(propertyDescriptor.PropertyType);

            TextBox control = new TextBox();

            control.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            control.FontFamily = FontUtils.DefaultFontFamily;
            control.IsEnabled = converter != null && 
                converter.CanConvertFrom(typeof(string)) && 
                converter.CanConvertTo(typeof(string));

            // set first ...
            // HandlePropertyChanged(target, propertyDescriptor, control);

            if (control.IsEnabled)
            {
                control.LostFocus += (s, e) =>
                {
                    string value = control.Text;

                    try
                    {
                        DataValidationErrors.ClearErrors(control);
                        var obj = converter.ConvertFrom(value);
                        SetAndRaise(context, control, obj);
                    }
                    catch (Exception ee)
                    {
                        DataValidationErrors.SetErrors(control, new string[] { ee.Message });
                    }
                };

                control.KeyUp += (s, e) =>
                {
                    if (e.Key == Input.Key.Enter)
                    {
                        string value = control.Text;

                        try
                        {
                            DataValidationErrors.ClearErrors(control);
                            var obj = converter.ConvertFrom(value);
                            SetAndRaise(context, control, obj);
                        }
                        catch (Exception ee)
                        {
                            DataValidationErrors.SetErrors(control, new string[] { ee.Message });
                        }
                    }
                };
            }

            return control;
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

            if (propertyDescriptor is MultiObjectPropertyDescriptor)
            {
                return false;
            }

            if (control is TextBox textBox)
            {
                var value = propertyDescriptor.GetValue(target);

                if(value != null)
                {
                    var converter = TypeDescriptor.GetConverter(propertyDescriptor.PropertyType);

                    if(converter != null && converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string)))
                    {
                        try
                        {
                            DataValidationErrors.ClearErrors(control);
                            textBox.Text = converter.ConvertTo(value, typeof(string)) as string;
                        }
                        catch (Exception ee)
                        {
                            DataValidationErrors.SetErrors(control, new string[] { ee.Message });
                        }
                    }
                    else
                    {
                        textBox.Text = value.ToString();
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
