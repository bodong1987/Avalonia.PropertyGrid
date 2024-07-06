using Avalonia.Platform;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using PropertyModels.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Avalonia.PropertyGrid.Localization
{
    /// <summary>
    /// Class JsonAssetLocalizationService.
    /// Implements the <see cref="MiniReactiveObject" />
    /// Implements the <see cref="ILocalizationService" />
    /// </summary>
    /// <seealso cref="MiniReactiveObject" />
    /// <seealso cref="ILocalizationService" />
    public class AssemblyJsonAssetLocalizationService : MiniReactiveObject, ILocalizationService
    {
        /// <summary>
        /// The extra services
        /// </summary>
        private readonly List<ILocalizationService> _extraServices = [];

        /// <summary>
        /// Occurs when [on culture changed].
        /// </summary>
        public event EventHandler OnCultureChanged;

        /// <summary>
        /// Gets the available cultures.
        /// </summary>
        /// <value>The available cultures.</value>
        public ISelectableList<ICultureData> AvailableCultures => _assetCultureData;

        /// <summary>
        /// The asset culture data
        /// </summary>
        private readonly SelectableList<ICultureData> _assetCultureData = [];

        /// <summary>
        /// Gets or sets the culture data.
        /// </summary>
        /// <value>The culture data.</value>
        public ICultureData CultureData
        {
            get => _assetCultureData.SelectedValue;
            set
            {
                if(_assetCultureData.SelectedValue != value)
                {
                    _assetCultureData.SelectedValue = value;

                    RaisePropertyChanged(nameof(CultureData));
                }                
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string this[string key]
        {
            get
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach(var service in _extraServices)
                {
                    var value = service[key];

                    if(value.IsNotNullOrEmpty() && value != key)
                    {
                        return value;
                    }
                }
                                
                if (_assetCultureData.SelectedValue?[key] is { } text)
                {
                    return text;
                }

                return key;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyJsonAssetLocalizationService"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyJsonAssetLocalizationService(Assembly assembly) :
            this(new Uri($"avares://{assembly.GetName().Name}/Assets/Localizations"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyJsonAssetLocalizationService"/> class.
        /// </summary>
        /// <param name="assetDirectoryUri">The asset directory URI.</param>
        public AssemblyJsonAssetLocalizationService(Uri assetDirectoryUri)
        {
            var assets = AssetLoader.GetAssets(assetDirectoryUri, null);

            foreach(var asset in assets)
            {
                var assetCultureData = new AssetCultureData(asset);
             
                AvailableCultures.Add(assetCultureData);
            }

            _assetCultureData.SelectionChanged += OnSelectionChanged;

            SelectCulture(CultureInfo.CurrentCulture.Name);
        }

        /// <summary>
        /// Gets the cultures.
        /// </summary>
        /// <returns>ICultureData[].</returns>
        public ICultureData[] GetCultures()
        {
            var values = new Dictionary<string, ICultureData>();
            foreach(var i in _assetCultureData)
            {
                if(!values.ContainsKey(i.Culture.Name))
                {
                    values.Add(i.Culture.Name, i);
                }
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach(var i in _extraServices)
            {
                var extraCultures = i.GetCultures();

                foreach(var extra in extraCultures)
                {
                    if(!values.ContainsKey(extra.Culture.Name))
                    {
                        values.Add(extra.Culture.Name, extra);
                    }
                }
            }

            return values.Values.ToArray();
        }

        /// <summary>
        /// Selects the culture.
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        public void SelectCulture(string cultureName)
        {
            foreach(var i in _extraServices)
            {
                i.SelectCulture(cultureName);
            }

            var cultureData = _assetCultureData.ToList().Find(x => x.Culture.Name == cultureName) ?? _assetCultureData.ToList().Find(x => x.Culture.Name == "en-US");

            if(cultureData != null)
            {
                CultureData = cultureData;

                if(!cultureData.IsLoaded)
                {
                    cultureData.Reload();
                }
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {            
            // all bind in xaml can be refreshed
            RaisePropertyChanged("Item");
            RaisePropertyChanged("Item[]");

            // all bind in code can be refreshed
            OnCultureChanged?.Invoke(sender, e);
        }

        #region Extra Services
        /// <summary>
        /// Adds the extra service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void AddExtraService(ILocalizationService service)
        {
            _extraServices.Add(service);
        }

        /// <summary>
        /// Removes the extra service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void RemoveExtraService(ILocalizationService service)
        {
            _extraServices.Remove(service);
        }

        /// <summary>
        /// Gets the extra services.
        /// </summary>
        /// <returns>ILocalizationService[].</returns>
        public ILocalizationService[] GetExtraServices()
        {
            return _extraServices.ToArray();
        }
        #endregion
    }
}
