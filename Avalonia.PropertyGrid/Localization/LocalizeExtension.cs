using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Controls;

namespace Avalonia.PropertyGrid.Localization
{
    /// <summary>
    /// Class LocalizeExtension.
    /// Implements the <see cref="MarkupExtension" />
    /// </summary>
    /// <seealso cref="MarkupExtension" />
    public class LocalizeExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public string Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizeExtension"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public LocalizeExtension(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>System.Object.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var keyToUse = Key;

            if (!string.IsNullOrWhiteSpace(Context))
            {
                keyToUse = $"{Context}/{Key}";
            }

            var binding = new ReflectionBindingExtension($"[{keyToUse}]")
            {
                Mode = BindingMode.OneWay,
                Source = Controls.PropertyGrid.LocalizationService,
            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}
