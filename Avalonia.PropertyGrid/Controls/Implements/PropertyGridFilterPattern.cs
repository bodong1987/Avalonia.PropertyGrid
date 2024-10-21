using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using Avalonia.PropertyGrid.Services;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridFilterPattern : MiniReactiveObject, IFilterPattern
    {
        private string? _filterText;
        private Regex? _cachedRegex;
        private ICheckedMaskModel? _quickFilter;

        public string? FilterText
        {
            get => _filterText;
            set => this.RaiseAndSetIfChanged(ref _filterText, value);
        }

        private bool _useRegex;

        public bool UseRegex
        {
            get => _useRegex;
            set => this.RaiseAndSetIfChanged(ref _useRegex, value);
        }

        private bool _ignoreCase = true;
        public bool IgnoreCase
        {
            get => _ignoreCase;
            set => this.RaiseAndSetIfChanged( ref _ignoreCase, value);
        }

        /// <summary>
        /// Gets the quick filter.
        /// </summary>
        /// <value>The quick filter.</value>
        public ICheckedMaskModel? QuickFilter
        {
            get => _quickFilter;
            set => this.RaiseAndSetIfChanged(ref _quickFilter, value);
        }

        public PropertyGridFilterPattern()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FilterText) && _useRegex && FilterText.IsNotNullOrEmpty())
            {
                _cachedRegex = null;
                try
                {
                    _cachedRegex = new Regex(FilterText.Trim());
                }                
                catch
                {
                }
            }
        }

        public bool Match(PropertyDescriptor propertyDescriptor, object? context)
        {
            var displayName = LocalizationService.Default[propertyDescriptor.DisplayName];

            if(UseRegex && _cachedRegex != null)
            {
                return _cachedRegex.IsMatch(displayName);
            }

            if(FilterText.IsNullOrEmpty())
            {
                return true;
            }

            return displayName.Contains(FilterText, IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
        }
    }
}
