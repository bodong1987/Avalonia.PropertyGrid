using Avalonia.Controls;
using Avalonia.Data;
using PropertyModels.ComponentModel;
using Avalonia.PropertyGrid.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Localization
{
    /// <summary>
    /// Class LocalizationExtensions.
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Sets the localize binding.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="property">The property.</param>
        /// <param name="name">The name.</param>
        /// <param name="mode">The mode.</param>
        public static void SetLocalizeBinding(this Control control, AvaloniaProperty property, string name, BindingMode mode = BindingMode.Default)
        {
            var source = new LocalizedDataModel(name);
            Binding binding = new Binding
            {
                Source = source,
                Path = nameof(source.Value)
            };

            control.Bind(property, binding);
            control.DataContext = source;
        }
    }

    internal class LocalizedDataModel : ReactiveObject
    {
        public readonly string Name;

        public string Value
        {
            get
            {
                var localizeService = LocalizationService.Default;

                // in design mode, service maybe unavailable...
                if (localizeService != null)
                {
                    return localizeService[Name];
                }

                return Name;
            }
        }

        public LocalizedDataModel(string name)
        {
            Name = name;

            var localizeService = LocalizationService.Default;

            if (localizeService != null)
            {
                localizeService.OnCultureChanged += OnCultureChanged;
            }
        }

        private void OnCultureChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Value));
        }
    }
}
