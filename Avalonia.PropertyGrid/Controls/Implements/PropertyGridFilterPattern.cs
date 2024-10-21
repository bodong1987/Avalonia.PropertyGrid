using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using Avalonia.PropertyGrid.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridFilterPattern : MiniReactiveObject, IFilterPattern
    {
        string? _FilterText;
        Regex? _CachedRegex;
        ICheckedMaskModel? _QuickFilter;

        public string? FilterText
        {
            get => _FilterText;
            set => this.RaiseAndSetIfChanged(ref _FilterText, value);
        }

        bool _UseRegex = false;

        public bool UseRegex
        {
            get => _UseRegex;
            set => this.RaiseAndSetIfChanged(ref _UseRegex, value);
        }

        bool _IgnoreCase = true;
        public bool IgnoreCase
        {
            get => _IgnoreCase;
            set => this.RaiseAndSetIfChanged( ref _IgnoreCase, value);
        }

        /// <summary>
        /// Gets the quick filter.
        /// </summary>
        /// <value>The quick filter.</value>
        public ICheckedMaskModel? QuickFilter
        {
            get => this._QuickFilter;
            set => this.RaiseAndSetIfChanged(ref _QuickFilter, value);
        }

        public PropertyGridFilterPattern()
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FilterText) && _UseRegex && FilterText.IsNotNullOrEmpty())
            {
                _CachedRegex = null;
                try
                {
                    _CachedRegex = new Regex(FilterText.Trim());
                }                
                catch
                {
                }
            }
        }

        public bool Match(PropertyDescriptor propertyDescriptor, object? context)
        {
            var displayName = LocalizationService.Default[propertyDescriptor.DisplayName];

            if(UseRegex && _CachedRegex != null)
            {
                return _CachedRegex.IsMatch(displayName);
            }

            if(FilterText.IsNullOrEmpty())
            {
                return true;
            }

            return displayName.Contains(FilterText, IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
        }
    }
}
