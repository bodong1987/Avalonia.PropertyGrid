using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridFilterPattern : MiniReactiveObject, IFilterPattern
    {
        private Regex? _cachedRegex;

        public string? FilterText
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        public bool UseRegex
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
        public bool IgnoreCase
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        } = true;

        /// <summary>
        /// Gets the quick filter.
        /// </summary>
        /// <value>The quick filter.</value>
        public ICheckedMaskModel? QuickFilter
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        public PropertyGridFilterPattern()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilterText) && UseRegex && FilterText.IsNotNullOrEmpty())
            {
                _cachedRegex = null;
                try
                {
                    _cachedRegex = new Regex(FilterText.Trim());
                }
                catch
                {
                    // ignored
                }
            }
        }

        public bool Match(PropertyDescriptor propertyDescriptor, object? context)
        {
            var displayName = LocalizationService.Default[propertyDescriptor.DisplayName];

            if (UseRegex && _cachedRegex != null)
            {
                return _cachedRegex.IsMatch(displayName);
            }

            return FilterText.IsNullOrEmpty() || displayName.Contains(FilterText, IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
        }
    }
}
