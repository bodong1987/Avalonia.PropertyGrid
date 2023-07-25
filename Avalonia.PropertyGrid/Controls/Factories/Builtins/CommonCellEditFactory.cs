using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    internal class CommonCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => int.MinValue;

        public override Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor)
        {
            if(propertyDescriptor is MultiObjectPropertyDescriptor)
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
                        SetAndRaise(rootPropertyGrid, control, propertyDescriptor, target, obj);
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
                            SetAndRaise(rootPropertyGrid, control, propertyDescriptor, target, obj);
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

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
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
