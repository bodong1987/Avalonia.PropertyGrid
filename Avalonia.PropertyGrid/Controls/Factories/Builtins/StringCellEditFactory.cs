using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
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
    internal class StringCellEditFactory : AbstractCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority - 1000000;

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            
            if (propertyDescriptor.PropertyType != typeof(string))
            {
                return null;
            }

            bool IsPathBrowsable = propertyDescriptor.IsDefined<PathBrowsableAttribute>();
            WatermarkAttribute watermarkAttr = propertyDescriptor.GetCustomAttribute<WatermarkAttribute>();

            if (IsPathBrowsable)
            {
                PathBrowsableAttribute attribute = propertyDescriptor.GetCustomAttribute<PathBrowsableAttribute>();

                ButtonEdit control = new ButtonEdit();
                control.Text = propertyDescriptor.GetValue(target) as string;
                //control.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                // control.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;

                if(watermarkAttr!=null)
                {
                    control.Watermark = watermarkAttr.Watermask;
                }

                control.ButtonClick += async (s, e) =>
                {
                    var files = await PathBrowserUtils.ShowPathBrowserAsync(control.GetVisualRoot() as Window, attribute);

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
            else
            {
                TextBox control = new TextBox();
                control.Text = propertyDescriptor.GetValue(target) as string;
                control.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                control.FontFamily = FontUtils.DefaultFontFamily;

                if (watermarkAttr != null)
                {
                    control.Watermark = watermarkAttr.Watermask;
                }

                if (propertyDescriptor.GetCustomAttribute<PasswordPropertyTextAttribute>() is PasswordPropertyTextAttribute ppt && ppt.Password)
                {
                    control.PasswordChar = '*';
                }

                MultilineTextAttribute multilineAttr = propertyDescriptor.GetCustomAttribute<MultilineTextAttribute>();

                if(multilineAttr != null && multilineAttr.IsMultiline)
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
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

            if (propertyDescriptor.PropertyType != typeof(string))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ButtonEdit be)
            {
                be.Text = propertyDescriptor.GetValue(target) as string;

                return true;
            }
            else if (control is TextBox textBox)
            {
                textBox.Text = propertyDescriptor.GetValue(target) as string;
                return true;
            }

            return false;
        }
    }
}
