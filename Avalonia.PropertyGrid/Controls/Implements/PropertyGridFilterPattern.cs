using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridFilterPattern : ReactiveObject, IPropertyGridFilterPattern
    {
        string _FilterText;
        Regex _CachedRegex;

        public string FilterText
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

        public PropertyGridFilterPattern()
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FilterText) && _UseRegex)
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

        public bool Match(PropertyDescriptor propertyDescriptor, object context)
        {
            var displayName = propertyDescriptor.DisplayName;

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
