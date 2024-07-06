using Avalonia;
using Avalonia.Platform;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using PropertyModels.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        readonly List<ILocalizationService> ExtraServices = new List<ILocalizationService>();

        /// <summary>
        /// Occurs when [on culture changed].
        /// </summary>
        public event EventHandler OnCultureChanged;

        /// <summary>
        /// Gets the available cultures.
        /// </summary>
        /// <value>The available cultures.</value>
        public ISelectableList<ICultureData> AvailableCultures => AssetCultureDatas;

        /// <summary>
        /// The asset culture datas
        /// </summary>
        readonly SelectableList<ICultureData> AssetCultureDatas = new SelectableList<ICultureData>();

        /// <summary>
        /// Gets or sets the culture data.
        /// </summary>
        /// <value>The culture data.</value>
        public ICultureData CultureData
        {
            get => AssetCultureDatas.SelectedValue;
            set
            {
                if(AssetCultureDatas.SelectedValue != value)
                {
                    AssetCultureDatas.SelectedValue = value;

                    this.RaisePropertyChanged(nameof(CultureData));
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
                foreach(var service in ExtraServices)
                {
                    string value = service[key];

                    if(value.IsNotNullOrEmpty() && value != key)
                    {
                        return value;
                    }
                }
                                
                if (AssetCultureDatas.SelectedValue != null && AssetCultureDatas.SelectedValue[key] is string text)
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
                AssetCultureData assetCultureData = new AssetCultureData(asset);
             
                AvailableCultures.Add(assetCultureData);
            }

            AssetCultureDatas.SelectionChanged += OnSelectionChanged;

            SelectCulture(CultureInfo.CurrentCulture.Name);
        }

        /// <summary>
        /// Gets the cultures.
        /// </summary>
        /// <returns>ICultureData[].</returns>
        public ICultureData[] GetCultures()
        {
            Dictionary<string, ICultureData> Values = new Dictionary<string, ICultureData>();
            foreach(var i in AssetCultureDatas)
            {
                if(!Values.ContainsKey(i.Culture.Name))
                {
                    Values.Add(i.Culture.Name, i);
                }
            }

            foreach(var i in ExtraServices)
            {
                var extraCultures = i.GetCultures();

                foreach(var extra in extraCultures)
                {
                    if(!Values.ContainsKey((string)extra.Culture.Name))
                    {
                        Values.Add(extra.Culture.Name, extra);
                    }
                }
            }

            return Values.Values.ToArray();
        }

        /// <summary>
        /// Selects the culture.
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        public void SelectCulture(string cultureName)
        {
            foreach(var i in ExtraServices)
            {
                i.SelectCulture(cultureName);
            }

            var cultureData = AssetCultureDatas.ToList().Find(x => x.Culture.Name == cultureName);

            if(cultureData == null)
            {
                cultureData = AssetCultureDatas.ToList().Find(x => x.Culture.Name == "en-US");
            }

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
            ExtraServices.Add(service);
        }

        /// <summary>
        /// Removes the extra service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void RemoveExtraService(ILocalizationService service)
        {
            ExtraServices.Remove(service);
        }

        /// <summary>
        /// Gets the extra services.
        /// </summary>
        /// <returns>ILocalizationService[].</returns>
        public ILocalizationService[] GetExtraServices()
        {
            return ExtraServices.ToArray();
        }
        #endregion
    }
}
